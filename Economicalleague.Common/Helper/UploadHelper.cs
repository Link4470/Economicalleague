using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Economicalleague.Common
{
    public class UploadHelper
    {
        public string SaveFile(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                string filename = file.FileName.Substring(0, file.FileName.LastIndexOf(".")) + ConvertDateTimeToInt(DateTime.Now);
                string suffix = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();

                try
                {
                    filename = filename + suffix;
                    string savePath = HttpContext.Current.Server.MapPath("~/UploadFile/Order/");
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    string filepath = savePath + filename;
                    file.SaveAs(filepath);
                    return filepath;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
    }
}
