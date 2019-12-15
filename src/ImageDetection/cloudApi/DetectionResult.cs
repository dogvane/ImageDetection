using System;
using System.Collections.Generic;

namespace ImageDetection {

    public interface IImageDetection {
        DetectionResult Detection (string url);
    }

    public class DetectionResultItem {

        /// <summary>
        /// 最终分类出来的类型
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 可信度
        /// </summary>
        public double Suggestion { get; set; }
    }

    public enum DetectionResultType
    {
        Unknow,

        /// <summary>
        /// 普通
        /// </summary>
        Normal,

        /// <summary>
        /// 性感
        /// </summary>
        Sexy,

        /// <summary>
        /// 色情
        /// </summary>
        Porn,
    }

    // 检查的结果
    public class DetectionResult {
        
        /// <summary>
        /// 检测结果
        /// </summary>
        public DetectionResultType Result { get; set; }

        public List<DetectionResultItem> Items { get; set; } = new List<DetectionResultItem>();

        /// <summary>
        /// 原始的返回值
        /// </summary>
        public string SourceResult { get; set; }

        /// <summary>
        /// 如果发生错误，这里是错误内容
        /// </summary>
        public string Error { get; set; }

        public string Platform { get; set; }
    }
}