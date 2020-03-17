using System;
using ImageDetection;
using Newtonsoft.Json;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tiia.V20190529;
using TencentCloud.Tiia.V20190529.Models;

namespace QcloudAI
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///     返回的结果看
    ///     https://cloud.tencent.com/document/product/669/14415
    /// </remarks>
    public class ImageAI
    {
        public static string Region_Id = "";

        public static string API_KEY = "";
        public static string SECRET_KEY = "";

        public static ImageModerationResponse ImageModeration(byte[] bytes)
        {
            var cred = new Credential
            {
                SecretId = API_KEY,
                SecretKey = SECRET_KEY
            };

            var clientProfile = new ClientProfile();
            var httpProfile = new HttpProfile();
            httpProfile.Endpoint = "tiia.tencentcloudapi.com";
            clientProfile.HttpProfile = httpProfile;

            var client = new TiiaClient(cred, Region_Id, clientProfile);
            var req = new ImageModerationRequest();
            req.Scenes = new[] {"PORN", "TERRORISM", "POLITICS", "TEXT"};
            req.ImageBase64 = Convert.ToBase64String(bytes);

            var resp = client.ImageModeration(req).ConfigureAwait(false).GetAwaiter().GetResult();
            return resp;
        }

        /// <summary>
        ///     注意使用时，要缩小图片的尺寸
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DetectionResult Detection(byte[] bytes)
        {
            try
            {
                var cred = new Credential
                {
                    SecretId = API_KEY,
                    SecretKey = SECRET_KEY
                };

                var clientProfile = new ClientProfile();
                var httpProfile = new HttpProfile();
                httpProfile.Endpoint = "tiia.tencentcloudapi.com";
                clientProfile.HttpProfile = httpProfile;

                var client = new TiiaClient(cred, Region_Id, clientProfile);
                var req = new ImageModerationRequest();
                req.Scenes = new[] {"PORN"};
                req.ImageBase64 = Convert.ToBase64String(bytes);

                var resp = client.ImageModeration(req).ConfigureAwait(false).GetAwaiter().GetResult();

                var ret = new DetectionResult {Platform = "腾讯"};

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
                        ret.Result = resp.PornResult.Suggestion == "PASS"
                            ? DetectionResultType.Sexy
                            : DetectionResultType.Porn;
                        break;
                    default:
                        ret.Result = DetectionResultType.Unknow;
                        break;
                }

                ret.Items.Add(new DetectionResultItem
                    {Suggestion = resp.PornResult.Confidence ?? 0, TypeName = resp.PornResult.Type});
                ret.SourceResult = JsonConvert.SerializeObject(resp, Formatting.Indented);
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new DetectionResult {Error = ex.Message, Platform = "腾讯"};
            }
        }
    }
}
