using System;
using System.Linq;
using System.Collections.Generic;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Green.Model.V20180509;
using ImageDetection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AliyunAI {

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 2019-12-14 时的文档地址
    /// https://help.aliyun.com/document_detail/70292.html?spm=a2c4g.11186623.2.49.69ab75fd61QYTt#reference-fzy-ztm-v2b
    /// </remarks>
    public class ImageAI {
        public static string Region_Id = "";
        public static string API_KEY = "";
        public static string SECRET_KEY = "";

        public object ImageDetection (string url) {
            IClientProfile profile = DefaultProfile.GetProfile (
                Region_Id, API_KEY, SECRET_KEY);

            DefaultAcsClient client = new DefaultAcsClient (profile);
            CommonRequest request = new CommonRequest ();
            request.Method = MethodType.POST;
            request.Domain = "green.cn-shanghai.aliyuncs.com";
            request.Version = "2018-05-09";
            request.UriPattern = "/green/image/scan";
            // request.Protocol = ProtocolType.HTTP;

            request.AddHeadParameters ("Content-Type", "application/json");
            var queryObj = new {
                scenes = new [] { "porn", "terrorism", "ad", "live", "logo" },
                tasks = new [] { new { url } }
            };
            var requestBody = Newtonsoft.Json.JsonConvert.SerializeObject (queryObj);
            

            request.SetContent (System.Text.Encoding.Default.GetBytes (requestBody), "utf-8", FormatType.JSON);
            CommonResponse response = client.GetCommonResponse (request);

            return response.Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public DetectionResult Detection(string url)
        {
            try
            {

                IClientProfile profile = DefaultProfile.GetProfile(
                    Region_Id, API_KEY, SECRET_KEY);

                DefaultAcsClient client = new DefaultAcsClient(profile);
                CommonRequest request = new CommonRequest();
                request.Method = MethodType.POST;
                request.Domain = "green.cn-shanghai.aliyuncs.com";
                request.Version = "2018-05-09";
                request.UriPattern = "/green/image/scan";

                request.AddHeadParameters("Content-Type", "application/json");
                var queryObj = new
                {
                    scenes = new[] { "porn" },
                    tasks = new[] { new { url } }
                };

                var requestBody = JsonConvert.SerializeObject(queryObj);

                request.SetContent(Encoding.Default.GetBytes(requestBody), "utf-8", FormatType.JSON);
                CommonResponse response = client.GetCommonResponse(request);
                var aliyunRet = JsonConvert.DeserializeObject<AliyunResult>(response.Data);

                var ret = new DetectionResult() { Platform = "阿里" };

                if (aliyunRet.code == 200)
                {
                    var data1 = aliyunRet.data.FirstOrDefault(o => o.code == 200);
                    var result = data1.results.FirstOrDefault(o => o.scene == "porn");
                    switch (result.label)
                    {
                        case "normal":
                            ret.Result = DetectionResultType.Normal;
                            break;
                        case "sexy":
                            ret.Result = DetectionResultType.Sexy;
                            break;
                        case "porn":
                            ret.Result = DetectionResultType.Porn;
                            break;
                        default:
                            ret.Result = DetectionResultType.Unknow;
                            break;
                    }

                    ret.Items.Add(new DetectionResultItem { TypeName = result.label, Suggestion = result.rate });
                }

                ret.SourceResult = JsonConvert.SerializeObject(aliyunRet, Formatting.Indented);
                return ret;
            }
            catch (Exception ex)
            {
                return new DetectionResult { Error = ex.Message, Platform = "阿里" };
            }
        }

        public class ResultsItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string label { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double rate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string scene { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string suggestion { get; set; }
        }

        public class DataItem
        {
            /// <summary>
            /// 
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string msg { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<ResultsItem> results { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string taskId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string url { get; set; }
        }

        public class AliyunResult
        {
            /// <summary>
            /// 
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DataItem> data { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string msg { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string requestId { get; set; }
        }
    }
}