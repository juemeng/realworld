using System;
using System.ComponentModel;

namespace Moniter.Models
{
    public enum AlertMessage
    {
        /// <summary>
        /// 0x50
        /// 分机呼叫主机（普通呼叫）
        /// </summary>
        [Description("分机呼叫主机（普通呼叫）")]
        NormalCall = 80, 
        
        /// <summary>
        /// 0x51
        /// 分机呼叫主机（紧急呼叫）
        /// </summary>
        [Description("分机呼叫主机（紧急呼叫）")]
        UrgencyCall = 81,  
        
        /// <summary>
        /// 0x52
        /// 开通对讲
        /// </summary>
        [Description("开通对讲")]
        OnCall = 82,
        
        /// <summary>
        /// 0x53
        /// 分机复位
        /// </summary>
        [Description("分机复位")]
        SubReset = 83,
        
        /// <summary>
        /// 0x54
        /// 主机复位分机
        /// </summary>
        [Description("主机复位分机")]
        MainReset = 84,
        
        /// <summary>
        /// 0x55
        /// 系统总复位
        /// </summary>
        [Description("系统总复位")]
        SystemReset = 85,

        /// <summary>
        /// 0x5E
        /// 只关闭对讲但呼叫仍然有效（个别医院的特殊要求！）
        /// </summary>
        [Description("关闭对讲但呼叫仍然有效")]
        CloseTalkButCallStillInEffect = 94,
        
        /// <summary>
        /// 0x5F
        /// 发送系统统一时间（24小时制）
        /// </summary>
        [Description("发送系统时间")]
        Time = 95,
    }


    /// <summary>
    /// 主机设置分机功能
    /// </summary>
    public enum MainCommand
    {
        /// <summary>
        /// 0x4E
        /// 设置分机具有群呼广播模式
        /// </summary>
        SetSubCallToBroadcastMode = 78,
        
        /// <summary>
        /// 0x4F
        /// 设置分机取消群呼广播模式
        /// </summary>
        CancelSubCallBroadcastModel = 79,
        
        /// <summary>
        /// 0x56
        /// 设置分机为普通呼叫模式
        /// </summary>
        SetSubCallToNormalMode = 86,
        
        /// <summary>
        /// 0x57
        /// 设置分机为紧急呼叫模式
        /// </summary>
        SetSubCallToUrgencyMode = 87,
    }


    public enum SaveAlertStatus
    {
        Saved,
        Skipped
    }

    public enum UserRole
    {
        Admin,
        User
    }

    public enum AlertStatus
    {
        New,
        Reset,
        Respond
    }
    
    public enum BindingStatus
    {
        None,
        Bound
    }
    
    
}