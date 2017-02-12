using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wam.Utility
{
    public abstract class parent
    {

        public parent()
        {
            name = "gene";

        }

        public string name;

      
        

        public string address {
            get;set;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public virtual void setAddress(string address)
        {
            this.address = address;
        }
    }
}
