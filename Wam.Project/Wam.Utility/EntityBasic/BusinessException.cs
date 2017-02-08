using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wam.Utility.EntityBasic
{
  public  class BusinessException:Exception
    {
        public int Code { get; private set; }

        /// <summary>
        /// Initializes a new instance of <c>BusinessException</c> class.
        /// </summary>
        public BusinessException() : base() { }
        /// <summary>
        /// Initializes a new instance of <c>BusinessException</c> class.
        /// </summary>
        /// <param name="message">The error message to be provided to the exception.</param>
        public BusinessException(string message, int code = 0) : base(message)
        {
            this.Code = code;
        }
    }
}
