using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Globalization;

namespace Economicalleague.Common
{
    public class Md5
    {
        public string Encrypt(string strPwd, string salt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string pwd = strPwd + salt;
            pwd = Reverse1(pwd);
            byte[] data = System.Text.Encoding.Default.GetBytes(pwd);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length - 1; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }
        private static string Reverse1(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="strPwd">需要加密的字符串</param>
        /// <returns></returns>
        public static string Encrypt(string strPwd)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new
                System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = Encoding.UTF8.GetBytes(strPwd);
            bs = md5.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            string password = s.ToString().ToLower();
            return password;
        }
    }

    /// <summary>
    /// Des加解密
    /// </summary>
    public class DesEncrypt
    {

        //DES默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        /// <summary>
        public static string EncryptDES(string encryptString, string encryptKey = "BZoneKey")
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return BytesToHex(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey = "BZoneKey")
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = HexToBytes(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        /// <summary>
        /// 字符串转byte
        /// </summary>
        /// <param name="Hex"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(string Hex)
        {
            int num = (int)Math.Round((double)(((double)Hex.Length) / 2));
            byte[] buffer = new byte[(num - 1) + 1];
            int num3 = num - 1;
            for (int i = 0; i <= num3; i++)
            {
                string s = Hex.Substring(i * 2, 2);
                buffer[i] = (byte)int.Parse(s, NumberStyles.HexNumber);
            }
            return buffer;
        }

        /// <summary>
        /// byte转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            int num2 = bytes.Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                builder.AppendFormat("{0:X2}", bytes[i]);
            }
            return builder.ToString();
        }
    }
}
