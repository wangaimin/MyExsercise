using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wam.Utility.EntityBasic
{
    public class EntityBase
    {
        /// <summary>
        /// 创建者系统编号
        /// </summary>
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建者的显示名
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime InDate { get; set; }

        public string InDateStr
        {
            get
            {
                return InDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    InDate = dt;
                }
                else
                {
                    InDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// 最后更新者系统编号
        /// </summary>
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 最后更新者显示名
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        public string EditDateStr
        {
            get
            {
                if (EditDate.HasValue)
                {
                    return EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return "";
            }
            set
            {
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    EditDate = dt;
                }
                else
                {
                    EditDate = null;
                }
            }
        }


        /// <summary>
        /// 我是否能编辑
        /// </summary>
        public bool IsMyData { get; set; }

        /// <summary>
        /// 数据所属组织对应的CompanyCode
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
