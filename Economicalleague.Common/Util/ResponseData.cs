using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace Economicalleague.Common.Util
{

    /// <summary>
    /// 数据列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseDataList<T>
    {
        /// <summary>
        /// 数据总数
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> DataList { get; set; }

    }

 

}
