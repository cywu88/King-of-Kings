using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public class NetConnector
    {
        protected internal readonly NetClientAdapter adapter;


        public NetConnector()
        {
            this.adapter = NetClientFactory.Instance.CreateAdapter(this);
        }


         

 


    }
}
