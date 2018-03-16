using System.Runtime.Serialization;

namespace Economicalleague.Domain.FavoritItems
{
    /// <summary>
    /// 选品库的请求参数类
    /// </summary>
    public class TbkFavoritesRequest
    {
        [DataContract]
        public class PageRequest
        {
            [DataMember(Name = "page_no")]
            public int PageNo { get; set; }
            [DataMember(Name = "page_size")]
            public int PageSize { get; set; }
            /// <summary>
            /// 需要返回的字段列表，不能为空，字段名之间使用逗号分隔
            /// </summary>
            [DataMember(Name = "fields")]
            public int[] FieIds { get; set; }
            /// <summary>
            /// 默认值-1；选品库类型，1：普通选品组，2：高佣选品组，-1，同时输出所有类型的选品组
            /// </summary>
            [DataMember(Name="type")]
            public string Type { get; set; }
        }
    }
}