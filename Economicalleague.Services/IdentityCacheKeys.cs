using Economicalleague.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Services
{
   public class IdentityCacheKeys
    {
        public static readonly CacheKey UserToken = new CacheKey("UserToken", TimeSpan.FromDays(5));
    }
}
