using System;
using System.Collections.Generic;
using System.Text;

namespace Economicalleague.Common.Cache
{
    public class CacheKey
    {
        public string Name { get; private set; }
        public TimeSpan SlidingExpiration { get; private set; }
        public string Suffix { get; protected set; }

        public CacheKey(string name, TimeSpan slidingExpiration)
        {
            this.Name = name;
            this.SlidingExpiration = slidingExpiration;
        }

        public CacheKey AppendSuffix(string suffix)
        {
            var cacheKey = new CacheKey(this.Name, this.SlidingExpiration);
            cacheKey.Suffix = suffix;
            return cacheKey;
        }

        public string UniqueKey
        {
            get
            {
                var uniqueKey = GenerateUniqueKey();
                return uniqueKey;
            }
        }

        private string GenerateUniqueKey()
        {
            return Prefix + this.Name + (String.IsNullOrEmpty(Suffix) ? "" : ":" + Suffix);
        }

        public static readonly string Prefix = "Economicalleague:";
    }
}
