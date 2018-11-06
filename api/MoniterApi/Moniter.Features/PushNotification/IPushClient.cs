using System.Collections.Generic;
using System.Linq;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Push.Model.V20160801;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Moniter.Infrastructure;

namespace Moniter.Features.PushNotification
{
    public interface IAliyunPushClient
    {
        void PushMessage(List<string> pushDeviceIds, string title, string body);
        void PushNotification(List<string> pushDeviceIds, string title, string body, string data);
    }

    public class AliyunPushClient : IAliyunPushClient
    {
        private readonly MoniterContext _context;
        private readonly PushSetting _pushSetting;

        public AliyunPushClient(MoniterContext context, IOptions<PushSetting> pushSettings)
        {
            _context = context;
            _pushSetting = pushSettings.Value;
        }

        public void PushNotification(List<string> pushDeviceIds, string title, string body, string data)
        {
//            if (pushDeviceIds.Any())
//            {
//                for (int i = 0; i < pushDeviceIds.Count / 100 + 1; i++)
//                {
//                    var deviceIds = pushDeviceIds.GetRange(i, pushDeviceIds.Count % 100);
//                    if (deviceIds.Any())
//                    {
//                        PushNoticeToAndroid(deviceIds.Join(","), title, body, data);
//                    }
//                }
//            }
        }


        public void PushMessage(List<string> pushDeviceIds, string title, string body)
        {
//            if (pushDeviceIds.Any())
//            {
//                for (int i = 0; i < pushDeviceIds.Count / 100 + 1; i++)
//                {
//                    var deviceIds = pushDeviceIds.GetRange(i, pushDeviceIds.Count % 100);
//                    if (deviceIds.Any())
//                    {
//                        PushMessageToAndroid(deviceIds.Join(","), title, body);
//                    }
//                }
//            }
        }

        public void PushMessageToAndroid(string deviceId, string title, string body)
        {
            var clientProfile =
                DefaultProfile.GetProfile("cn-hangzhou", _pushSetting.AccessKey,
                    _pushSetting.AccessSecret);
            var client = new DefaultAcsClient(clientProfile);

            var request = new PushMessageToAndroidRequest
            {
                AppKey = _pushSetting.AppKey,
                Target = "DEVICE",
                TargetValue = deviceId,
                Title = title,
                Body = body
            };
            //推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送
            ////根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备一次最多100)
            try
            {
                var respone = client.GetAcsResponse(request);
            }
            catch (ServerException e)
            {
                throw e;
            }
            catch (ClientException e)
            {
                throw e;
            }
        }

        public void PushNoticeToAndroid(string deviceId, string title, string body, string data)
        {
            IClientProfile clientProfile =
                DefaultProfile.GetProfile("cn-hangzhou", _pushSetting.AccessKey,
                    _pushSetting.AccessSecret);
            DefaultAcsClient client = new DefaultAcsClient(clientProfile);
            PushNoticeToAndroidRequest request = new PushNoticeToAndroidRequest();
            request.AppKey = _pushSetting.AppKey; //<your Appkey>;
            request.Target = "DEVICE"; //推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送
            request.TargetValue = deviceId; //根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备一次最多100个)
            request.Title = title;
            request.Body = body;
            if (!string.IsNullOrEmpty(data))
            {
                request.ExtParameters = data;
            }

            try
            {
                PushNoticeToAndroidResponse response = client.GetAcsResponse(request);
            }
            catch (ServerException e)
            {
            }
        }
    }
}