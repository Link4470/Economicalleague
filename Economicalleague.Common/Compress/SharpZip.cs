using System.IO;
using ICSharpCode.SharpZipLib.GZip;

namespace Economicalleague.Common.Compress
{
    /// <summary>
    /// SharpZip
    /// </summary>
    public class SharpZip : ICompress
    {
        public byte[] Compress(byte[] buffer, long index, long count)
        {
            byte[] arrBuffer = null;
            MemoryStream ms = new MemoryStream();
            GZipOutputStream objGzip = new GZipOutputStream(ms);
            #region copy
            //const int BUFFER_SIZE = 1024 * 10;
            //byte[] arrBuffer = new byte[BUFFER_SIZE];
            //int nGetedCount = 0;
            //do
            //{
            //    nGetedCount = ms.Read(arrBuffer, 0, BUFFER_SIZE);
            //    objGzip.Write(arrBuffer, 0, nGetedCount);
            //} while (nGetedCount > 0);
            #endregion
            objGzip.Write(buffer, 0, buffer.Length);
            //objGzip.SetLevel(level);
            objGzip.Finish();
            arrBuffer = ms.ToArray();
            ms.Close();
            objGzip.Close();

            return arrBuffer;
        }

        public byte[] DeCompress(byte[] buffer, long index, long count)
        {
            int nIndex = (int)index;
            int nCount = (int)count;
            MemoryStream ms = new MemoryStream(buffer, nIndex, nCount);
            var gis = new GZipInputStream(ms);
            MemoryStream ret = new MemoryStream();
            int buffersize = 2048;
            var arrBuffer = new byte[buffersize];
            while (buffersize > 0)
            {
                buffersize = gis.Read(arrBuffer, 0, buffersize);
                ret.Write(arrBuffer, 0, buffersize);
            }
            arrBuffer = ret.ToArray();
            ret.Close();
            gis.Close();
            ms.Close();

            return arrBuffer;
        }
    }

}