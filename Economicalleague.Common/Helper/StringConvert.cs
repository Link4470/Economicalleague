using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Economicalleague.Common
{
    /// <summary>
    /// 字符串转换类
    /// </summary>
    public class StringConvert
    {
        /// <summary>
        /// 数字转换为中文(语音验证码使用)
        /// </summary>
        /// <param name="value">数字</param>
        /// <returns></returns>
        public static string NumToChinessStr(int value)
        {
            char[] chnGenText = new char[] { '零', '一', '二', '三', '四', '五', '六', '七', '八', '九' };
            string str = value.ToString();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                int num = int.Parse(str[i].ToString());
                //添加顿号让语速变慢
                sb.Append("、" + chnGenText[num].ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 格式化上次登录显示时间
        /// </summary>
        /// <param name="dt">上一次登录时间</param>
        /// <returns></returns>
        public static string DateFromNow(DateTime dt)
        {
            if (dt == null)
                return string.Empty;
            double totalDays = (DateTime.Now - dt).TotalDays;
            if (totalDays <= 1)
            {
                return "1天内来过";
            }
            else if (totalDays <= 2)
            {
                return "2天内来过";
            }
            else if (totalDays <= 7)
            {
                return "1周内来过";
            }
            else if (totalDays <= 14)
            {
                return "2周内来过";
            }
            else if (totalDays <= 21)
            {
                return "3周内来过";
            }
            else if (totalDays <= 30)
            {
                return "1月内来过";
            }
            return "1月前来过";
        }


        /// <summary>
        /// 开始、结束时间转换
        /// </summary>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public static string DateDurationToStr(DateTime begin, DateTime end)
        {
            if (begin.Year == end.Year)
            {
                return string.Format("{0}-{1}", begin.ToString("yyyy年MM月dd日"), end.ToString("MM月dd日"));
            }
            else
            {
                return string.Format("{0}-{1}", begin.ToString("yyyy年MM月dd日"), end.ToString("yyyy年MM月dd日"));
            }
        }

        /// <summary>
        /// 开始、结束时间转换
        /// </summary>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public static string ConvertToChinessNamePrefixStr(int sex)
        {
            if (sex == 1)
            {
                return "先生";
            }
            else if (sex == 2)
            {
                return "女士";
            }

            return string.Empty;
        }

        /// <summary>
        /// 判断两个数组是否存在交集
        /// </summary>
        /// <param name="strArrayFirst">第一个数组</param>
        /// <param name="strArraySecond">第二个数组</param>
        /// <returns></returns>
        public static bool HasIntersect(string[] strArrayFirst, string[] strArraySecond)
        {
            if (strArrayFirst.Length == 0 || strArraySecond.Length == 0)
                return false;
            return strArrayFirst.Intersect(strArraySecond).Count() > 0;
        }

        /// <summary>
        /// 获取汉字首字字母
        /// </summary>
        /// <param name="str">汉字</param>
        /// <returns>首字母</returns>
        public static string GetFirstPinyin(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            char firstChar = str[0];
            try
            {
                ChineseChar chineseChar = new ChineseChar(firstChar);
                string pinyin = chineseChar.Pinyins[0].ToString();
                return pinyin.Substring(0, 1);
            }
            catch
            {
                return firstChar.ToString();
            }
        }

        /// <summary>
        /// 转义字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns></returns>
        public static string Escape(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            var result = new StringBuilder(str);
            Dictionary<string, string> toReplace = new Dictionary<string, string>() {
                { "\t","   "},{ "\v", Environment.NewLine},
                { "\f", string.Empty},{ "\b", string.Empty},
                { "\a", string.Empty},{ "\0", string.Empty},{ "\\\\","\\"}
            };
            foreach (var item in toReplace)
            {
                result = result.Replace(item.Key, item.Value);
            }
            if (result.ToString().Contains("\r\n"))
            {
                result = result.Replace("\r\n", Environment.NewLine);
            }
            else if (result.ToString().Contains("\r"))
            {
                result = result.Replace("\r", Environment.NewLine);
            }
            else if (result.ToString().Contains("\n"))
            {
                result = result.Replace("\n", Environment.NewLine);
            }
            return result.ToString();
        }

        /// <summary>
        /// 流转换为字节数组
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }


        /// <summary>
        /// 字节数组转换为流
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>

        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// 判断是否为手机号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string str)
        {
            return Regex.IsMatch(str, @"^[1][3-8]\d{9}$");
        }

        /// <summary>
        /// Sha1加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns></returns>
        public static string EncryptToSHA1(string str)
        {
            byte[] cleanBytes = Encoding.Default.GetBytes(str);
            byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// 四舍5入
        /// </summary>
        /// <param name="str">初始值</param>
        /// <param name="pos">保留位数</param>
        /// <returns></returns>
        public static string FomatFloat(double str, int pos)
        {
            string formatStr = "0.";
            for (int i = 0; i < pos; i++)
            {
                formatStr += "#";
            }
            return (Math.Round(str * Math.Pow(10, pos)) / Math.Pow(10, pos)).ToString(formatStr);
        }

        /// <summary>
        /// 判断是否为中文
        /// </summary>
        /// <param name="CString"></param>
        /// <returns></returns>
        public static bool IsChinese(string str)
        {
            return Regex.IsMatch(str, @"^[\u4e00-\u9fa5]+$");
        }
        /// <summary>
        /// 根据生日获取年龄
        /// </summary>
        /// <param name="birthdate">生日</param>
        /// <returns></returns>
        public int GetAgeByBirthdate(DateTime birthdate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthdate.Year;
            if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
            {
                age--;
            }
            return age < 0 ? 0 : age;
        }
        /// <summary>
        /// 获取名字的最后两个字
        /// </summary>
        /// <param name="customerName">姓名</param>
        /// <returns></returns>
        public static string GetShortName(string customerName)
        {
            if (String.IsNullOrEmpty(customerName))

            {
                return "";
            }
            else
            {
                var NickArray = customerName.ToArray();
                if (NickArray.Length > 1)
                {
                    return NickArray[NickArray.Length - 2].ToString() + NickArray[NickArray.Length - 1].ToString();
                }
                else
                {
                    return NickArray[NickArray.Length - 1].ToString();
                }
            };
        }
    }
}
