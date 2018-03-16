using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Services.TaoBao
{
    public class TaoBaoConfig
    {
        public static string AppKey
        {
            get
            {
                string appKeyValue = ConfigurationManager.AppSettings["TaoBaoAppKey"];
                if (!string.IsNullOrEmpty(appKeyValue))
                {
                    return appKeyValue;
                }
                else
                {
                    throw new ConfigurationErrorsException("TaoBaoAppKey未配置");
                }
            }
        }

        public static string AppSecret
        {
            get
            {
                string appSecretValue = ConfigurationManager.AppSettings["TaoBaoAppSecret"];
                if (!string.IsNullOrEmpty(appSecretValue))
                {
                    return appSecretValue;
                }
                else
                {
                    throw new ConfigurationErrorsException("TaoBaoAppSecret未配置");
                }
            }
        }
        public static string Url
        {
            get
            {
                string url = ConfigurationManager.AppSettings["TaoBaoUrl"];
                if (!string.IsNullOrEmpty(url))
                {
                    return url;
                }
                else
                {
                    throw new ConfigurationErrorsException("TaoBaoUrl未配置");
                }
            }
        }

        public static string UId
        {
            get
            {
                string uid = ConfigurationManager.AppSettings["UId"];
                if (!string.IsNullOrEmpty(uid))
                {
                    return uid;
                }
                else
                {
                    throw new ConfigurationErrorsException("推广者ID未配置");
                }
            }
        }

        public static string TaoBaoSiteId
        {
            get
            {
                string siteid = ConfigurationManager.AppSettings["TaoBaoSiteId"];
                if (!string.IsNullOrEmpty(siteid))
                {
                    return siteid;
                }
                else
                {
                    throw new ConfigurationErrorsException("网站ID未配置");
                }
            }
        }



        public static string AdzoneName
        {
            get
            {
                string name = ConfigurationManager.AppSettings["AdzoneName"];
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
                else
                {
                    throw new ConfigurationErrorsException("广告位名称未配置");
                }
            }
        }

        public static string PlatformFee
        {
            get
            {
                string platformfee = ConfigurationManager.AppSettings["PlatformFee"];
                if (!string.IsNullOrEmpty(platformfee))
                {
                    return platformfee;
                }
                else
                {
                    throw new ConfigurationErrorsException("平台服务费未配置");
                }
            }
        }
    }
}
