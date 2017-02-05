using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wam.Utility.EntityBasic
{
  public  class AjaxResult
    {
        public AjaxResult() {
            Success = true;
        }
        public string Msg { get; set; }

        public object Data { get; set; }

        public bool Success { get; set;}
    }
}
