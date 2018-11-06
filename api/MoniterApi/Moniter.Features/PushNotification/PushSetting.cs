namespace Moniter.Features.PushNotification
{
    public class PushSetting : IPushSetting
    {
        public long AppKey { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
    }
}