using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Economicalleague.Infrastructure.Response
{
    public enum ErrCode
    {
        /// <summary>
        /// 接口调用成功
        /// </summary>
        [Description("接口调用成功")]
        Sucess = 10000,

        /// <summary>
        /// 接口调用失败
        /// </summary>
        [Description("接口调用失败")]
        Failure = 10001,

        /// <summary>
        /// 无权限
        /// </summary>
        [Description("无权限")]
        PermissionDenied = 10002,

        /// <summary>
        /// 关联关系错误
        /// </summary>
        [Description("关联关系错误")]
        RelativeError = 10003,

        /// <summary>
        /// 数据已存在
        /// </summary>
        [Description("数据已存在")]
        DataIsExist = 10004,

        /// <summary>
        /// 数据不存在
        /// </summary>
        [Description("数据不存在")]
        DataIsnotExist = 10005,

        /// <summary>
        /// 数据查询错误
        /// </summary>
        [Description("数据查询错误")]
        QueryError = 10006,

        /// <summary>
        /// 数据插入错误
        /// </summary>
        [Description("数据插入错误")]
        InsertError = 10007,

        /// <summary>
        /// 数据删除错误
        /// </summary>
        [Description("数据删除错误")]
        DeleteError = 10008,

        /// <summary>
        /// 数据修改错误
        /// </summary>
        [Description("数据修改错误")]
        UpdateError = 10009,

        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        ParameterError = 10010,

        /// <summary>
        /// 内部错误
        /// </summary>
        [Description("服务忙，请稍候重试")]
        InnerError = 10011,


        /// <summary>
        /// 账号已存在
        /// </summary>
        [Description("账号已注册，请直接登录")]
        customerNameAlreadyExist = 10012,



        /// <summary>
        /// 账号或密码错误
        /// </summary>
        [Description("账号或密码错误")]
        ErrUsernameOrPwd = 10013,

        /// <summary>
        /// 旧密码输入错误
        /// </summary>
        [Description("旧密码输入错误")]
        ErrorOldPassword = 10014,

        /// <summary>
        /// 短信验证码错误
        /// </summary>
        [Description("短信验证码错误")]
        InvalidSmsCode = 10015,

        /// <summary>
        /// 验证码错误
        /// </summary>
        [Description("验证码错误")]
        InvalidCheckCode = 10016,

        /// <summary>
        /// 手机号码不合法
        /// </summary>
        [Description("手机号码不合法")]
        InvalidPhoneNum = 10017,

        /// <summary>
        /// 用户名或者密码不合法
        /// </summary>
        [Description("用户名或者密码不合法")]
        InvalidUsernameOrPwd = 10018,

        /// <summary>
        /// 输入的参数不能为空
        /// </summary>
        [Description("输入的参数不能为空")]
        ParametersIsNotAllowedEmpty = 10019,

        /// <summary>
        /// 输入的手机号码不能为空
        /// </summary>
        [Description("输入的账号码不能为空")]
        customerNameIsNotAllowedEmpty = 10020,

        /// <summary>
        /// 发送验证码失败
        /// </summary>
        [Description("发送验证码失败")]
        SendCodeFailed = 10021,

        /// <summary>
        /// 发送验证码成功
        /// </summary>
        [Description("发送验证码成功")]
        SendCodeSuccess = 10022,

        /// <summary>
        /// 输入的验证码信息不能为空
        /// </summary>
        [Description("输入的验证码信息不能为空")]
        CodeIsNotAllowedEmpty = 10023,

        /// <summary>
        /// 验证码过期,返回重发
        /// </summary>
        [Description("验证码过期,返回重发")]
        CodeIsOutDuration = 10024,

        /// <summary>
        /// 请输入正确的验证码
        /// </summary>
        [Description("请输入正确的验证码")]
        InvalidCode = 10025,

        /// <summary>
        /// 帐号已存在
        /// </summary>
        [Description("帐号已存在")]
        AccountAlreadyExist = 10026,

        /// <summary>
        /// 输入的密码信息不能为空
        /// </summary>
        [Description("输入的密码信息不能为空")]
        PasswordIsNotAllowedEmpty = 10027,

        /// <summary>
        /// 账号未注册
        /// </summary>
        [Description("账号未注册")]
        AccountNotExist = 10028,

        /// <summary>
        /// 输入的Token信息不能为空
        /// </summary>
        [Description("输入的令牌信息不能为空")]
        TokenIsNotAllowedEmpty = 10029,

        /// <summary>
        /// 新密码与旧密码不能相同
        /// </summary>
        [Description("新密码与旧密码不能相同")]
        PasswordIsNotAllowedSame = 10030,

        /// <summary>
        /// 输入的新密码不能为空
        /// </summary>
        [Description("输入的新密码不能为空")]
        NewPasswordIsNotAllowedEmpty = 10031,

        /// <summary>
        /// 上传文件失败
        /// </summary>
        [Description("上传文件失败")]
        UploadFileFailed = 10032,

        /// <summary>
        /// 每页记录数不能小于零
        /// </summary>
        [Description("每页记录数不能小于零")]
        PageSizeIsNotAllowedLessThanZero = 10033,

        /// <summary>
        /// 页码不能小于等于零
        /// </summary>
        [Description("页码不能小于等于零")]
        PageIndexIsNotAllowedLessThanZero = 10034,

        /// <summary>
        /// 通用:{0}不可为空
        /// </summary>
        [Description("{0}不可为空")]
        NotAllowedNull = 10035,

        /// <summary>
        /// 通用:{0}不在范围内
        /// </summary>
        [Description("{0}不在范围内")]
        OutOfRange = 10036,

        /// <summary>
        /// 通用:{0}不可重复
        /// </summary>
        [Description("{0}不可重复")]
        NotAlloweDuplicate = 10037,

        /// <summary>
        /// Redis错误
        /// </summary>
        [Description("Redis错误")]
        RedisFailure = 10038,

        /// <summary>
        /// Token已过期，请重新登录！
        /// </summary>
        [Description("用户状态已过期，请重新登录！")]
        TokenPastDue = 10039,

        /// <summary>
        /// 授权认证失败401
        /// </summary>
        [Description("授权认证失败")]
        Unauthorized = 10040,

        /// <summary>
        /// 您的帐号已在其他地方登录，请重新登录！
        /// </summary>
        [Description("您的帐号已在其他地方登录，请重新登录！")]
        OtherWhereLogin = 10041,

        #region 好友
        /// <summary>
        /// 你们已经是好友，无需重复添加
        /// </summary>
        [Description("你们已经是好友，无需重复添加！")]
        IsAlreadyFriend = 10042,

        /// <summary>
        /// 您已经向对方发送过添加好友请求，无需重复发送
        /// </summary>
        [Description("您已经向对方发送过添加好友请求，无需重复发送！")]
        IsAlreadySendAddFriendReq = 10043,

        /// <summary>
        /// 您已经收到对方的添加好友请求，请及时处理！
        /// </summary>
        [Description("您已经收到对方的添加好友请求，请及时处理！")]
        IsAlreadyReceiveAddFriendReq = 10044,

        /// <summary>
        /// 对方拒绝添加你为好友
        /// </summary>
        [Description("对方拒绝添加你为好友")]
        IsAlreadyRefuseFriend = 10045,

        /// <summary>
        /// 您和对方不是好友，无需删除！
        /// </summary>
        [Description("您和对方不是好友，无需删除！")]
        IsNotFriend = 10046,
        /// <summary>
        /// 圈子已满
        /// </summary>
        [Description("成员人数超过圈子限定人数")]
        IsGroupFull = 10047,
        /// <summary>
        /// 用户已经在圈子中
        /// </summary>
        [Description("已在圈子中")]
        IsGroupMemberExist = 10048,
        /// <summary>
        /// 没有群聊Id
        /// </summary>
        [Description("没有群聊Id")]
        CreateGroupEaseFailure = 10049,
        /// <summary>
        /// 添加到群聊群组失败
        /// </summary>
        [Description("添加到群聊群组失败")]
        AddGroupEaseFailure = 10050,
        /// <summary>
        /// 不能与已有圈子重名
        /// </summary>
        [Description("不能与已有圈子重名")]
        AddGroupNameFailure = 10051,
        /// <summary>
        /// 圈子不存在管理员或圈主
        /// </summary>
        [Description("圈子不存在管理员或圈主")]
        ApplayGroupNotExistManagerFailure = 10052,
        /// <summary>
        /// 发送申请加入圈子消息失败
        /// </summary>
        [Description("发送申请加入圈子消息失败")]
        ApplayGroupSendManagerFailure = 10053,
        /// <summary>
        /// 群组不存在
        /// </summary>
        [Description("群组不存在")]
        NotExsitGroupInfo = 10054,
        /// <summary>
        /// "删除环信群组失败
        /// </summary>
        [Description("删除环信群组失败")]
        DeleteEaseGroupError = 10055,
        /// <summary>
        ///转让群组失败
        /// </summary>
        [Description("转让群组失败")]
        ChangeGroupOwerError = 10056,

        [Description("已经被其他管理员同意加入圈子")]
        AlreadyAcceptedByOtherManger = 10057,

        [Description("已经被其他管理员拒绝加入圈子")]
        AlreadRefusedByOtherManager = 10058,

        [Description("已经同意好友请求")]
        AlreadyAcceptedInvitation = 10059,

        [Description("已经拒绝好友请求")]
        AlreadRefusedInvitation = 10060,
        [Description("申请次数太多，请明天再试")]
        ApplyGroupMaximum = 10061,
        [Description("今天已申请")]
        AlreadyApplyGroup = 10062,
        /// <summary>
        /// 不能加自己为好友
        /// </summary>
        [Description("不能加自己为好友")]
        CantAddSelfFriend = 10063,
        [Description("一次最多选择50个！")]
        AddGroupBatchNumberError = 10064,
        #endregion

        /// <summary>
        /// 访问接口频率超出限制
        /// </summary>
        [Description("访问接口频率超出限制")]
        ApiRateLimits = 10065,
        [Description("获取验证码太频繁，请稍后再试")]
        SendCodeLimits = 10066,
        [Description("Excel模板错误")]
        ExcelModelError = 10067,

        /// <summary>
        /// 您没有圈子管理权限
        /// </summary>
        [Description("您没有圈子管理权限")]
        IsNotManger = 10068,
        [Description("您已经认证")]
        AlreadyIdentificated = 10069,
        [Description("手机号已注册，可直接登录")]
        AccountAlreadyExistCanLogin = 10070,
        [Description("活动模板已有商会在使用不允许删除")]
        TemplateAlreadyInUsed = 10071,
        /// <summary>
        /// 圈子不存在
        /// </summary>
        [Description("圈子不存在")]
        GroupNotExist = 10072,
        /// <summary>
        /// 不在圈子
        /// </summary>
        [Description("您不是圈子成员")]
        IsNotInGroup = 10073,
        #region 活动
        /// <summary>
        /// 开始时间必须要晚于现在
        /// </summary>
        [Description("开始时间必须要晚于现在")]
        BeginTimeAfterNow = 10074,
        /// <summary>
        /// 截止时间必须要晚于现在
        /// </summary>
        [Description("截止时间必须要晚于现在")]
        DeadLineTimeAfterNow = 10075,
        /// <summary>
        /// 结束时间必须要晚于开始时间
        /// </summary>
        [Description("结束时间必须要晚于开始时间")]
        EndTimeAfterBeginTime = 10076,
        /// <summary>
        /// 截止时间必须早于开始时间
        /// </summary>
        [Description("截止时间必须早于开始时间")]
        DeadLineTimeBeforeBeginTime = 10077,
        /// <summary>
        /// 抱歉，在你预览活动期间，该活动主题已下架，不能继续发布
        /// </summary>
        [Description("抱歉，在你预览活动期间，该活动主题已下架，不能继续发布")]
        ActivityTemplateDown = 10078,
        /// <summary>
        /// 在你预览活动期间，该活动内容有所更新，请重新选择并发布活动
        /// </summary>
        [Description("在你预览活动期间，该活动内容有所更新，请重新选择并发布活动")]
        ActivityTemplateUpdate = 10079,
        /// <summary>
        /// 抱歉，在你预览活动期间，该活动时间已被他人预定，不能继续发布
        /// </summary>
        [Description("抱歉，在你预览活动期间，该活动时间已被他人预定，不能继续发布")]
        ActivityTemplateTimeUse = 10080,
        /// <summary>
        /// 该圈子今天已经发过活动类短信
        /// </summary>
        [Description("该圈子今天已经发过活动类短信")]
        GroupTodaySendActivityMsg = 10081,
        /// <summary>
        /// 该活动已截止报名或取消
        /// </summary>
        [Description("该活动已截止报名或取消")]
        ActivityDeadLine = 10082,
        /// <summary>
        /// 活动报名失败
        /// </summary>
        [Description("活动报名失败")]
        ActivityJoinFail = 10083,
        #endregion
        /// <summary>
        /// 圈主不能退出圈子
        /// </summary>
        [Description("圈主不能退出圈子")]
        GroupMasterNotQuit = 10084,
        /// <summary>
        /// 退出群聊群组失败
        /// </summary>
        [Description("退出群聊群组失败")]
        QuitGroupEaseFailure = 10085,
        /// <summary>
        /// 圈子名称不能少于2字
        /// </summary>
        [Description("圈子名称不能少于2字")]
        GroupNameLessThan2 = 10086,
        /// <summary>
        /// 非商会圈子的名称不能包含“商会”字眼
        /// </summary>
        [Description("非商会圈子的名称不能包含“商会”字眼")]
        GroupNameHasSHANGHUI = 10087,
        /// <summary>
        /// 圈子名称不能包含“懂老板”
        /// </summary>
        [Description("圈子名称不能包含“懂老板”")]
        GroupNameHasSHANGYOUQUAN = 10088,

        [Description("获取APP版本异常")]
        AppVersionError = 10089,

        [Description("当前版本不存在")]
        VersionUndefined = 10090,

        [Description("设备类型不正确")]
        DeviceError = 10091,
        [Description("apk文件不存在")]
        ApkNotExists = 10092,
        [Description("该版本已经发布过,请重命名！")]
        VersionExists = 10093,
        [Description("内容已删除")]
        DataIsDeleted = 10094,
        [Description("该活动已下架")]
        DataIsUnShelve = 10095,
        [Description("商会圈已经存在该名称！")]
        ChameberExistName = 10096,
        [Description("上传Excel失败")]
        UploadExcelError = 10096,
        [Description("数据合法性，校验失败")]
        InvaildData = 10097,
        [Description("创建圈子失败")]
        CreateGroupFailure = 10098,
        [Description("已经生成活动订单")]
        ActivityOrderExist = 10099,
        [Description("活动已经开始，不能修改")]
        ActivityBegun = 10100,
        [Description("该活动订单不存在")]
        ActivityOrderNotExist = 10101,
        /// <summary>
        /// 姓名不是有效的格式
        /// </summary>
        [Description("姓名不是有效的格式")]
        NameIsWrong = 10102,
        /// <summary>
        /// 已报名参加该活动，去看看其他活动吧~
        /// </summary>
        [Description("已报名参加该活动，去看看其他活动吧~")]
        CustomerActivityIsJoin = 10103,
        /// <summary>
        /// 活动已截止，去看看其他活动吧~
        /// </summary>
        [Description("活动已截止，去看看其他活动吧~")]
        CustomerActivityIsDeadLine = 10104,
        /// <summary>
        /// 报名人数已达到上限，去看看其他活动吧~
        /// </summary>
        [Description("报名人数已达到上限，去看看其他活动吧~")]
        CustomerActivityIsFullJoin = 10105,
        /// <summary>
        /// 请选择门票
        /// </summary>
        [Description("请选择门票")]
        TicketIsNotChoose = 10106,
        /// <summary>
        /// 还有用户待付款，请不断刷新哦~
        /// </summary>
        [Description("还有用户待付款，请不断刷新哦~")]
        HasCustomerNotPay = 10107,
        /// <summary>
        /// 报名失败，请重试
        /// </summary>
        [Description("报名失败，请重试")]
        CustomerActivityJoinFailure = 10108,
        [Description("缺少经纬度")]
        NeedAddressLatitudeError = 10109,
        /// <summary>
        /// 管理员不能删除圈主/管理员
        /// </summary>
        [Description("管理员不能删除圈主/管理员！")]
        CantRemoveManager = 10110,
        [Description("认证会员必须提供职位信息")]
        lackJobInfoError = 10111,
        [Description("Vip必须是认证会员")]
        VipError = 10112,
        [Description("您还未注册懂老板，请下载注册！")]
        UnRegisterError = 10113,
        [Description("懂老板认证用户才能报名！")]
        NeedIndentityError = 10114,

        /// <summary>
        /// 动态错误
        /// </summary>
        [Description("该动态已删除")]
        DynamicDeletedError = 10115,
        [Description("该评论已删除")]
        CommentDeletedError = 10116,
        /// <summary>
        /// 该活动订单支付状态已变更
        /// </summary>
        [Description("该活动订单支付状态已变更")]
        ActivityOrderPayStatusChanged = 10117,
        /// <summary>
        /// 该活动订单已超时
        /// </summary>
        [Description("该活动订单已超时")]
        ActivityOrderExpires = 10118,
        /// <summary>
        /// 该活动订单已支付
        /// </summary>
        [Description("该活动订单已支付")]
        ActivityOrderIsAlreadyPaied = 10119,
        /// <summary>
        /// 爱心用户没有权限
        /// </summary>
        [Description("爱心用户没有该权限")]
        BeautyPermissionDenied = 10120,
        [Description("最多添加100个")]
        AddGroupChatBatchNumberError = 10121,
        [Description("不在当前群聊中")]
        NotInGroupChat = 10122,
        [Description("Api版本错误")]
        ApiVersionIncorrect = 10123,
        [Description("超出数量限制")]
        LimitExceeded = 10124,
        [Description("已经存在活动推荐！")]
        ExistDiscoverActivity = 10125,
        [Description("该功能已关闭")]
        CannotOperateGroup = 10126,
        [Description("订单不存在")]
        OrderNotExist = 10127,
        [Description("该订单已支付")]
        OrderIsAlreadyPaied = 10128,
        [Description("该订单支付状态已变更")]
        OrderPayStatusChanged = 10129,
        [Description("该订单已超时")]
        OrderExpires = 10130,
        /// <summary>
        /// 认证图片不可为空
        /// </summary>
        [Description("认证图片不可为空")]
        IdentificationNotAllowedNull = 10131,
        [Description("下单失败！")]
        CustomerOrderJoinFailure = 10132,
        [Description("订单明细不能为空！")]
        CustomerOrderNotAllowEmpty = 10133,
        [Description("订单总金额和订单明细总金额不一致！")]
        CustomerOrderTotalMoenyNotEqualOrderItemMoeny = 10134,
        [Description("订单不明确！")]
        CustomerOrderNotClear = 10135,
        [Description("生成支付宝支付信息失败")]
        GetAlipayTradeAppPayInfoFailed = 10136,
        [Description("获取支付宝支付状态失败")]
        GetAlipayStatusFailed = 10137,
        [Description("生成微信支付信息失败")]
        GetWxPayInfoFailed = 10138,
        [Description("获取微信支付状态失败")]
        GetWxPayStatusFailed = 10139,
        [Description("商品信息已存在")]
        ItemIdAlreadyExist = 10140,
        [Description("获取淘宝联盟选品库的宝贝信息失败")]
        GetItemsFailed = 10141,
        [Description("获取淘宝联盟选品库的列表信息失败")]
        GetItemsListFailed = 10142,
        [Description("生成淘口令信息失败")]
        CreatePwdFailed = 10143,
        [Description("搜索宝贝信息失败")]
        SearchFailed = 10144,
        [Description("搜索商品信息失败")]
        SearchShopFailed = 10144,
        [Description("创建广告位id失败")]
        CreatePidFailed = 10145,
        [Description("获取收入信息失败")]
        GetIncomFailed = 10146,
        [Description("获取订单信息列表失败")]
        GetordersFailed = 10147,
        [Description("获取订单详情失败")]
        GetorderDetailFailed = 10148
    }
}