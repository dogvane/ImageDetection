using ImageDetection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tiia.V20190529;
using TencentCloud.Tiia.V20190529.Models;

namespace QcloudAI {

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 返回的结果看
    /// https://cloud.tencent.com/document/product/669/14415
    /// </remarks>
    public class ImageAI {
                public static string Region_Id = "";

        public static string API_KEY = "";
        public static string SECRET_KEY = "";

        public static ImageModerationResponse ImageModeration (byte[] bytes) {
            Credential cred = new Credential {
                SecretId = API_KEY,
                SecretKey = SECRET_KEY
            };

            ClientProfile clientProfile = new ClientProfile ();
            HttpProfile httpProfile = new HttpProfile ();
            httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;

            TiiaClient client = new TiiaClient (cred, Region_Id, clientProfile);
            ImageModerationRequest req = new ImageModerationRequest ();
            req.Scenes = new [] { "PORN", "TERRORISM", "POLITICS", "TEXT" };
            req.ImageBase64 = Convert.ToBase64String (bytes);

            ImageModerationResponse resp = client.ImageModeration (req).
            ConfigureAwait (false).GetAwaiter ().GetResult ();
            return resp;
        }

        /// <summary>
        /// 注意使用时，要缩小图片的尺寸
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DetectionResult Detection(byte[] bytes)
        {
            try
            {
                Credential cred = new Credential
                {
                    SecretId = API_KEY,
                    SecretKey = SECRET_KEY
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                TiiaClient client = new TiiaClient(cred, Region_Id, clientProfile);
                ImageModerationRequest req = new ImageModerationRequest();
                req.Scenes = new[] { "PORN" };
                req.ImageBase64 = Convert.ToBase64String(bytes);

                ImageModerationResponse resp = client.ImageModeration(req).
                ConfigureAwait(false).GetAwaiter().GetResult();

                var ret = new DetectionResult() { Platform = "腾讯" };

                switch (resp.PornResult.Type.ToLower())
                {
                    case "normal":
                        ret.Result = DetectionResultType.Normal;
                        break;
                    case "porn":
                        ret.Result = DetectionResultType.Porn;
                        break;
                    case "hot":
                        ret.Result = DetectionResultType.Sexy;
                        break;
                    case "breast":
                    case "ass":
                    case "bareBody":
                    case "unrealHotPeople":
                        if (resp.PornResult.Suggestion == "PASS")
                            ret.Result = DetectionResultType.Sexy;
                        else
                            ret.Result = DetectionResultType.Porn;
                        break;
                    default:
                        ret.Result = DetectionResultType.Unknow;
                        break;
                }

                ret.Items.Add(new DetectionResultItem { Suggestion = resp.PornResult.Confidence ?? 0, TypeName = resp.PornResult.Type });
                ret.SourceResult = JsonConvert.SerializeObject(resp, Formatting.Indented);
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new DetectionResult { Error = ex.Message, Platform = "腾讯" };
            }
        }
    }
}