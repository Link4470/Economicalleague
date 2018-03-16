using System.Runtime.Serialization;

namespace Economicalleague.Domain
{
    /// <summary>
    /// 公共参数
    /// </summary>
    [DataContract]
    public class PublicParams
    {
        /// <summary>
        /// API接口名称 （必须）
        /// </summary>
        [DataMember(Name = "method")]
        public string Method { get; set; }

        /// <summary>
        /// TOP分配给应用的AppKey（必须）
        /// </summary>
        [DataMember(Name = "app_key")]
        public string AppKey { get; set; }

        /// <summary>
        /// 用户登录授权成功后，TOP颁发给应用的授权信息
        /// </summary>
        [DataMember(Name = "session")]
        public string Session { get; set; }

        /// <summary>
        /// 时间戳 （必须）
        /// </summary>
        [DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// 响应格式。默认为xml格式，可选值：xml，json。
        /// </summary>
        [DataMember(Name = "format")]
        public string Format { get; set; }

        /// <summary>
        /// API协议版本，可选值：2.0。 （必须）
        /// </summary>
        [DataMember(Name = "v")]
        public string V { get; set; }

        /// <summary>
        /// 合作伙伴ID
        /// </summary>
        [DataMember(Name = "partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 被调用的目标AppKey，仅当被调用的API为第三方ISV提供时有效。
        /// </summary>
        [DataMember(Name = "target_app_key")]
        public string TargetAppKey { get; set; }

        /// <summary>
        /// 是否采用精简JSON返回格式，仅当format=json时有效，默认值为：false。
        /// </summary>
        [DataMember(Name = "simplify")]
        public bool Simplify { get; set; }

        /// <summary>
        /// 签名的摘要算法，可选值为：hmac，md5。 （必须）
        /// </summary>
        [DataMember(Name = "sign_method")]
        public string SignMethod { get; set; }

        /// <summary>
        /// API输入参数签名结果，签名算法参照下面的介绍。 （必须）
        /// </summary>
        [DataMember(Name = "sign")]
        public string Sign { get; set; }

    }
}