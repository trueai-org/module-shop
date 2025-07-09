using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shop.Module.SmsSenderAliyun.Models
{
    /// <summary>
    /// 阿里云百炼AI嵌入向量服务的请求参数类
    /// 用于封装发送到阿里云百炼Embedding API的请求数据
    /// </summary>
    public class AliyunAIEmbeddingRequest
    {
        /// <summary>
        /// 模型名称
        /// 指定要使用的嵌入模型，如 "text-embedding-v1"
        /// </summary>
        [JsonPropertyName("model")]
        [Required(ErrorMessage = "模型名称不能为空")]
        public string Model { get; set; }

        /// <summary>
        /// 输入数据
        /// 包含要生成嵌入向量的文本内容
        /// </summary>
        [JsonPropertyName("input")]
        [Required(ErrorMessage = "输入数据不能为空")]
        public EmbeddingInputData Input { get; set; }

        /// <summary>
        /// 请求参数
        /// 包含API调用的各种配置参数
        /// </summary>
        [JsonPropertyName("parameters")]
        public EmbeddingParameters Parameters { get; set; }
    }

    /// <summary>
    /// 嵌入向量输入数据
    /// 包含要处理的文本内容
    /// </summary>
    public class EmbeddingInputData
    {
        /// <summary>
        /// 要生成嵌入向量的文本数组
        /// 每个元素代表一个文本片段
        /// </summary>
        [JsonPropertyName("texts")]
        [Required(ErrorMessage = "文本数组不能为空")]
        [MinLength(1, ErrorMessage = "至少需要提供一个文本")]
        public List<string> Texts { get; set; } = new List<string>();

        /// <summary>
        /// 验证输入数据的有效性
        /// </summary>
        /// <returns>如果数据有效则返回true，否则返回false</returns>
        public bool IsValid()
        {
            return Texts != null && 
                   Texts.Any() && 
                   Texts.All(text => !string.IsNullOrWhiteSpace(text));
        }

        /// <summary>
        /// 获取总文本长度
        /// </summary>
        /// <returns>所有文本的总字符数</returns>
        public int GetTotalTextLength()
        {
            return Texts?.Sum(text => text?.Length ?? 0) ?? 0;
        }
    }

    /// <summary>
    /// 嵌入向量请求参数
    /// 包含API调用的配置选项
    /// </summary>
    public class EmbeddingParameters
    {
        /// <summary>
        /// 文本类型
        /// 指定输入文本的类型，如 "query"（查询）或 "document"（文档）
        /// </summary>
        [JsonPropertyName("text_type")]
        public string TextType { get; set; } = "document";

        /// <summary>
        /// 输出格式
        /// 指定返回向量的格式，通常为 "float"
        /// </summary>
        [JsonPropertyName("output_format")]
        public string OutputFormat { get; set; } = "float";

        /// <summary>
        /// 维度
        /// 指定输出向量的维度，如果不指定则使用模型默认维度
        /// </summary>
        [JsonPropertyName("dimension")]
        [Range(1, 4096, ErrorMessage = "维度必须在1到4096之间")]
        public int? Dimension { get; set; }

        /// <summary>
        /// 批处理大小
        /// 指定批处理的大小，用于控制并发处理数量
        /// </summary>
        [JsonPropertyName("batch_size")]
        [Range(1, 100, ErrorMessage = "批处理大小必须在1到100之间")]
        public int BatchSize { get; set; } = 1;
    }

    /// <summary>
    /// 阿里云百炼AI服务配置选项
    /// 用于配置阿里云百炼服务的连接和认证信息
    /// </summary>
    public class AliyunAIServiceOptions
    {
        /// <summary>
        /// API密钥
        /// 用于认证阿里云百炼服务的API密钥
        /// </summary>
        [Required(ErrorMessage = "API密钥不能为空")]
        public string ApiKey { get; set; }

        /// <summary>
        /// 服务端点
        /// 阿里云百炼服务的API端点地址
        /// </summary>
        [Required(ErrorMessage = "服务端点不能为空")]
        public string Endpoint { get; set; }

        /// <summary>
        /// 工作空间ID
        /// 阿里云百炼服务的工作空间标识符
        /// </summary>
        public string WorkspaceId { get; set; }

        /// <summary>
        /// 请求超时时间（秒）
        /// </summary>
        [Range(1, 300, ErrorMessage = "超时时间必须在1到300秒之间")]
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// 重试次数
        /// 请求失败时的重试次数
        /// </summary>
        [Range(0, 10, ErrorMessage = "重试次数必须在0到10之间")]
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// 是否为测试环境
        /// </summary>
        public bool IsTestEnvironment { get; set; } = false;
    }
}