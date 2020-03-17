using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading;

namespace ImageDetection.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPost(List<IFormFile> files, string imageUrl)
        {
            long size = 0;
            this.ViewData["error"] = -1;

            if (files != null && files.Count > 0)
            {
                // 上传图片的模式
                var file = files[0];
                var stream = file.OpenReadStream();
                size = stream.Length;
                var bytes = new byte[size];
                stream.Read(bytes, 0, (int)size);

                if(bytes.Length > 0)
                {
                    bytes = ReSizeImageFile(bytes);

                    var baiduApi = new BaiduAI.ImageAI();
                    var baiduRet = baiduApi.Detection(bytes);
                    DetectionResults.Add(baiduRet);

                    var qcloudRet = QcloudAI.ImageAI.Detection(bytes);
                    DetectionResults.Add(qcloudRet);

                    // 阿里云平台，则需要先上传到他们的oss，然后再进行测试
                    var aliyunApi = new AliyunAI.ImageAI();
                    var url = aliyunApi.UploadFile(file.FileName.Replace(" ", ""), bytes);                    
                    var aliyunRet = aliyunApi.Detection(url);
                    DetectionResults.Add(aliyunRet);
                    aliyunApi.DeleteFile(file.FileName);

                    ImageBase64 = Convert.ToBase64String(bytes);
                }
            }
            else if (!string.IsNullOrEmpty(imageUrl))
            {
                var webclient = new WebClient();
                var bytes = webclient.DownloadData(imageUrl);
                if (bytes.Length > 0)
                {
                    bytes = ReSizeImageFile(bytes);

                    var baiduApi = new BaiduAI.ImageAI();
                    var baiduRet = baiduApi.Detection(bytes);
                    DetectionResults.Add(baiduRet);

                    var qcloudRet = QcloudAI.ImageAI.Detection(bytes);
                    DetectionResults.Add(qcloudRet);

                    var aliyunApi = new AliyunAI.ImageAI();
                    var aliyunRet = aliyunApi.Detection(imageUrl);
                    DetectionResults.Add(aliyunRet);

                    ImageBase64 = Convert.ToBase64String(bytes);
                }
            }
            else
            {
                ErrorMessage = "上传图片或者url，你总得设置一个吧";
            }
        }

        /// <summary>
        /// 重新调整图片的大小
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static byte[] ReSizeImageFile(byte[] bytes)
        {
            // 按照图片上传模式走，忽略阿里云的模式
            int maxFileSize = 256 * 1024;
            if (bytes.Length > maxFileSize)
            {
                // 图片体积比较大，建议还是先等比例压缩一下，否则腾讯云上传会有问题
                var rate = maxFileSize / (double)bytes.Length;

                var image = Image.Load(bytes);
                var width = (int)(image.Width * rate);
                var height = (int)(image.Height * rate);

                // 操作缩小图像
                image.Mutate(o => o.Resize(width, height));

                // 将图像重新导出到bytes里
                var memstream = new System.IO.MemoryStream();
                image.SaveAsJpeg(memstream);
                bytes = new byte[memstream.Length];
                memstream.Position = 0;
                memstream.Read(bytes, 0, bytes.Length);
            }

            return bytes;
        }

        public string ErrorMessage { get; set; }

        public string ImageBase64 { get; set; }

        public List<DetectionResult> DetectionResults { get; set; } = new List<DetectionResult>();

        [HttpPost]
        public IActionResult UploadFiles(List<IFormFile> files)
        {
            return RedirectToPage("./Privacy");
        }
    }
}
