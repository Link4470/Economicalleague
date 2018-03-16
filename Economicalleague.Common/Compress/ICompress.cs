using System;

namespace Economicalleague.Common.Compress
{
    /// <summary>
    /// 压缩接口
    /// </summary>
    public interface ICompress
    {
        byte[] Compress(byte[] buffer, long index, long count);
        byte[] DeCompress(byte[] buffer, long index, long count);
    }

}