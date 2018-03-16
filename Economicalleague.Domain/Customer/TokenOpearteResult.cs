using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Domain.Customer
{
    /// <summary>
    /// Token操作结果
    /// </summary>
    public class TokenOpearteResult
    {
        public string token { get; set; }
        public bool isok { get; set; }
        public int code { get; set; }
        public object data { get; set; }
        public string result { get; set; }

    }
}
