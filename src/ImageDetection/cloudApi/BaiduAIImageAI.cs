using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using Baidu.Aip.ContentCensor;
using Baidu.Aip.ImageClassify;
using ImageDetection;
using Newtonsoft.Json.Linq;

namespace BaiduAI {
    /// <summary>
    /// 脸数据库
    /// </summary>
    public class ImageAI {
        public static string APP_ID = "";
        public static string API_KEY = "";
        public static string SECRET_KEY = "";

        public JObject ImageUserDefined(byte[] bytes) {
            var client = new ImageCensor(API_KEY, SECRET_KEY);
            client.Timeout = 60000; // 修改超时时间

            var ret = client.UserDefined(bytes);

            return ret;
        }
        public JObject AntiPorn(byte[] bytes) {
            var client = new AntiPorn(API_KEY, SECRET_KEY);
            client.Timeout = 60000; // 修改超时时间

            var ret = client.Detect(bytes);

            return ret;
        }

        /// <summary>
        /// 图片检测
        /// </summary>
        /// <param name=""></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public DetectionResult Detection(byte[] bytes)
        {
            try
            {
                var client = new AntiPorn(API_KEY, SECRET_KEY);
                client.Timeout = 50000; // 修改超时时间

                var jsonObj = client.Detect(bytes);

                var ret2 = jsonObj.ToObject<BaiduImageDetectionResult>();
                var ret = new DetectionResult() { Platform = "百度" };

                foreach (var item in ret2.result)
                {
                    // 只考虑返回概率大于50%的情况，这个后面顶一个参数吧
                    if (item.probability > 0.5)
                    {
                        ret.Items.Add(new DetectionResultItem
                        {
                            TypeName = item.class_name,
                            Suggestion = item.probability * 100
                        });
                    }
                }

                var max = ret.Items.OrderByDescending(o => o.Suggestion).First();
                switch (max.TypeName)
                {
                    case "正常":
                        ret.Result = DetectionResultType.Normal;
                        break;
                    case "色情":
                        ret.Result = DetectionResultType.Porn;
                        break;
                    case "性感":
                        ret.Result = DetectionResultType.Sexy;
                        break;
                    default:
                        ret.Result = DetectionResultType.Unknow;
                        break;
                }

                ret.SourceResult = jsonObj.ToString();
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new DetectionResult { Error = ex.Message, Platform = "百度" };
            }
        }

        public class BaiduImageDetectionResultItem
        {
            /// <summary>
            /// 结果类型
            /// </summary>
            public string class_name { get; set; }
            /// <summary>
            /// 确信的概率
            /// </summary>
            public double probability { get; set; }
        }

        public class BaiduImageDetectionResult
        {
            /// <summary>
            /// 
            /// </summary>
            public int result_num { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public long log_id { get; set; }
            /// <summary>
            /// 置信度（确定）
            /// </summary>
            public string confidence_coefficient { get; set; }
            /// <summary>
            /// 简单的返回
            /// </summary>
            public List<BaiduImageDetectionResultItem> result { get; set; }
            /// <summary>
            /// 更加信息的返回
            /// </summary>
            public List<BaiduImageDetectionResultItem> result_fine { get; set; }
            /// <summary>
            /// 返回结果（正常）
            /// </summary>
            public string conclusion { get; set; }
        }
    }
}