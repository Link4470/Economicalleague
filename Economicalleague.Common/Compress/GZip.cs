using System;
using System.IO;
using System.IO.Compression;

namespace Economicalleague.Common.Compress
{
    /// <summary>
    /// Gzip
    /// </summary>
    public class Gzip : ICompress
    {
        public byte[] Compress(byte[] buffer, long index, long count)
        {
            int nIndex = (int) index;
            int nCount = (int) count;
            MemoryStream inputStream = new MemoryStream(buffer, nIndex, nCount);
            MemoryStream outputStream = new MemoryStream();
            GZipStream compressionStream = new GZipStream(outputStream, CompressionMode.Compress);

            inputStream.CopyTo(compressionStream);
            compressionStream.Close();
            outputStream.Close();
            inputStream.Close();

            return outputStream.ToArray();
        }

        public byte[] DeCompress(byte[] buffer, long index, long count)
        {
            int nIndex = (int)index;
            int nCount = (int)count;
            MemoryStream inputStream = new MemoryStream(buffer, nIndex, nCount);
            MemoryStream outputStream = new MemoryStream();
            GZipStream compressionStream = new GZipStream(inputStream, CompressionMode.Decompress);
            compressionStream.CopyTo(outputStream);
            compressionStream.Close();
            outputStream.Close();
            inputStream.Close();

            return outputStream.ToArray();
        }
    }

}