namespace Moniter.Features.PushNotification
{
    public interface IPushSetting
    {
        long AppKey { set; get; }
        string AccessKey { set; get; }
        string AccessSecret { set; get; }
    }
}