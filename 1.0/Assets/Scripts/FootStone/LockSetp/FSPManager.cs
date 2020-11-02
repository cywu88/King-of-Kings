using System.Collections.Generic;
using UnityEngine;

namespace FootStone
{
    public class FSPManager
    {
        private FSPFrameController m_FrameCtrl;
        private Dictionary<int, FSPFrame> m_FrameBuffer;   // 帧号 + 若干消息列表
         
        private bool m_IsRunning = false;
        private bool m_isPause;
         
        private int m_CurrentFrameIndex = 0;
        private int m_ClientLockedFrame = 0;               // 客户端收到的服务器最新帧率
        private int m_executeSpeedTmp;


        public void Start()
        {
            m_FrameBuffer = new Dictionary<int, FSPFrame>();
            m_FrameCtrl = new FSPFrameController();
            m_FrameCtrl.Start();

            m_IsRunning = true;
            m_isPause = false;

            m_CurrentFrameIndex = 0;
        }

        public void Stop()
        {
            if (m_FrameCtrl != null)
            {
                m_FrameCtrl.Close();
                m_FrameCtrl = null;
            }

            m_FrameBuffer.Clear();
            m_IsRunning = false;
            m_isPause = false;
            m_ClientLockedFrame = 0;
        }


        /// <summary>
        /// 由外界驱动（固定桢驱动）
        /// </summary>
        public void FixedUpdate()
        {
            if (!m_IsRunning || m_isPause)
            {
                return;
            }

            UpdateSpeedController();

            while (m_executeSpeedTmp > 0)
            {
                if (CanExecuteCurFrame())
                {
                    m_CurrentFrameIndex++;

                    FSPFrame frame = null;
                    m_FrameBuffer.TryGetValue(m_CurrentFrameIndex, out frame);
                    ExecuteFrame(m_CurrentFrameIndex, frame);

                    m_executeSpeedTmp--;
                } 
            } 
        }
 
        /// <summary>
        /// 控制帧运行速度
        /// </summary>
        private void UpdateSpeedController()
        {
            m_executeSpeedTmp = m_FrameCtrl.GetFrameSpeed(m_CurrentFrameIndex);

        }


        /// <summary>
        /// 可以执行当前帧
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteCurFrame()
        {
            return m_CurrentFrameIndex < m_ClientLockedFrame;
        }

        /// <summary>
        /// 执行每一帧的消息列表
        /// </summary>
        private void ExecuteFrame(int frameId, FSPFrame frame)
        {
            //优先处理流程VKey
            if (frame != null && frame.vkeys != null)
            {
                for (int i = 0; i < frame.vkeys.Count; i++)
                {
                    //处理完了消息？  感觉这里可以加个dispatcher转发
                    NetMessage cmd = frame.vkeys[i];
                    cmd.OnRecv();
                }
            }

            //执行桢消息
            GameManager.Instance.EnterFrame(frameId);
        }


        /// <summary>
        /// 接受帧消息入口
        /// 每一条消息包含FSPFrame列表
        /// FSPFrame：帧号 + 具体消息内容
        /// </summary>
        public void AddServerFrameMsg(NetMessage msg)
        {
            //TODO 将网络消息转为帧消息
            var frame = new FSPFrame();
            AddServerFrameUnit(frame);

        }

        public void AddServerFrameUnit(FSPFrame frame)
        {
            m_ClientLockedFrame = frame.frameId + FSPParam.clientFrameRateMultiple - 1;

            if (!m_FrameBuffer.ContainsKey(frame.frameId))
            {
                m_FrameBuffer.Add(frame.frameId, frame);
                m_FrameCtrl.AddNewFrameId(frame.frameId);
            }
            else
            {
                Debug.LogErrorFormat("m_FrameBuffer.ContainsKey AddServerFrameUnit frameId={0}", frame.frameId);
            }
        }


    }

}
