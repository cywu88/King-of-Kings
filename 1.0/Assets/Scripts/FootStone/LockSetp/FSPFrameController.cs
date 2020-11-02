namespace FootStone
{
    public class FSPFrameController
    {

        //缓冲控制
        private int m_NewestFrameId; //接收消息时候最新的桢
        private int m_BuffSize = 0;  //需要缓存的帧数
        private bool m_IsInBuffing = false;  //是否再缓存状态
        private int m_ClientFrameRateMultiple = 2;   //客户端服务器的帧率比


        //加速控制
        private bool m_EnableSpeedUp = true;   //是否可以加速
        private int m_DefaultSpeed = 1;  //默认的数量
        private bool m_IsInSpeedUp = false; //是否加速ing


        //自动缓冲
        private bool m_EnableAutoBuff = true; //自动缓存,比如卡的时候需要缓存就是卡久一点。
        private int m_AutoBuffCnt = 0;
        private int m_AutoBuffInterval = 15; //缓存的周期
         


        //启动桢缓存
        public void Start()
        {
            SetParam();
        }

        public void Close()
        {
        }

        public void SetParam()
        {
            m_ClientFrameRateMultiple = FSPParam.clientFrameRateMultiple;
            m_BuffSize = FSPParam.frameBufferSize;
            m_EnableSpeedUp = FSPParam.enableSpeedUp;
            m_DefaultSpeed = FSPParam.defaultSpeed;
            m_EnableAutoBuff = FSPParam.enableAutoBuffer;
        }


        /// <summary>
        /// 获取当前要加速的帧率数
        /// </summary>
        public int GetFrameSpeed(int curFrameId)
        {
            int speed = 0;

            // 帧率差值0,1,2,3
            int newFrameNum = m_NewestFrameId - curFrameId;

            //不在缓存中
            if (!m_IsInBuffing)
            {
                //没有在缓冲中
                if (newFrameNum == 0)
                {
                    //需要缓冲一下
                    m_IsInBuffing = true;
                    m_AutoBuffCnt = m_AutoBuffInterval;
                }
                else
                {
                    //因为即将播去这么多帧
                    newFrameNum -= m_DefaultSpeed;
                    //剩下的可加速的帧数
                    int speedUpFrameNum = newFrameNum - m_BuffSize;
                    if (speedUpFrameNum >= m_ClientFrameRateMultiple)
                    {
                        //可以加速
                        if (m_EnableSpeedUp)
                        {
                            speed = speedUpFrameNum > 100 ? 32 : 2;
                        }
                        else
                        {
                            speed = m_DefaultSpeed;
                        }
                    }
                    else
                    {
                        //还达不到可加速的帧数
                        speed = m_DefaultSpeed;

                        //主动缓冲，当帧数过少时，
                        //与其每一帧都卡，不如先卡久一点，然后就不卡了

                        if (m_EnableAutoBuff)
                        {
                            m_AutoBuffCnt--;
                            if (m_AutoBuffCnt <= 0)
                            {
                                m_AutoBuffCnt = m_AutoBuffInterval;
                                if (speedUpFrameNum < m_ClientFrameRateMultiple - 1)
                                {
                                    speed = 0;
                                }
                            }
                        }
                    }
                } 
            }
            else
            {
                //正在缓冲中

                //剩下的可加速的帧数
                int speedUpFrameNum = newFrameNum - m_BuffSize;
                if (speedUpFrameNum > 0)
                {
                    //当缓冲的数量足够时，结束缓冲
                    m_IsInBuffing = false;
                }
            }
             
            return speed;
        }


        /// <summary>
        /// 接受最新帧
        /// </summary>
        public void AddNewFrameId(int frameId)
        {
            m_NewestFrameId = frameId;
        }
    }
}
