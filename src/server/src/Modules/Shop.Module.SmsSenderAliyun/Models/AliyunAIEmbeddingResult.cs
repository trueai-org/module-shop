using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shop.Module.SmsSenderAliyun.Models
{
    /// <summary>
    /// 阿里云百炼AI嵌入向量服务的响应结果类
    /// 用于封装阿里云百炼Embedding API的返回数据
    /// 参考文档：https://help.aliyun.com/document_detail/2842587.html
    /// </summary>
    public class AliyunAIEmbeddingResult
    {
        /// <summary>
        /// 请求ID，用于标识和跟踪API请求
        /// </summary>
        [JsonPropertyName("request_id")]
        [Required(ErrorMessage = "请求ID不能为空")]
        public string RequestId { get; set; }

        /// <summary>
        /// 响应输出数据
        /// </summary>
        [JsonPropertyName("output")]
        public EmbeddingOutputData Output { get; set; }

        /// <summary>
        /// 使用统计信息
        /// </summary>
        [JsonPropertyName("usage")]
        public UsageInfo Usage { get; set; }

        /// <summary>
        /// 验证响应结果的完整性
        /// </summary>
        /// <returns>如果数据完整且有效则返回true，否则返回false</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(RequestId) && 
                   Output != null && 
                   Output.Embeddings != null && 
                   Output.Embeddings.Any() &&
                   Usage != null;
        }

        /// <summary>
        /// 获取嵌入向量的总数量
        /// </summary>
        /// <returns>嵌入向量的总数量</returns>
        public int GetEmbeddingCount()
        {
            return Output?.Embeddings?.Count ?? 0;
        }

        /// <summary>
        /// 获取第一个嵌入向量的维度
        /// </summary>
        /// <returns>向量维度，如果没有向量则返回0</returns>
        public int GetEmbeddingDimension()
        {
            return Output?.Embeddings?.FirstOrDefault()?.Embedding?.Count ?? 0;
        }
    }

    /// <summary>
    /// 嵌入向量输出数据
    /// 包含生成的嵌入向量集合
    /// </summary>
    public class EmbeddingOutputData
    {
        /// <summary>
        /// 嵌入向量数组
        /// 每个元素代表一个文本片段的向量表示
        /// </summary>
        [JsonPropertyName("embeddings")]
        [Required(ErrorMessage = "嵌入向量数组不能为空")]
        public List<EmbeddingData> Embeddings { get; set; } = new List<EmbeddingData>();
    }

    /// <summary>
    /// 单个嵌入向量数据
    /// 包含文本索引和对应的向量值
    /// </summary>
    public class EmbeddingData
    {
        /// <summary>
        /// 文本片段在输入数组中的索引位置
        /// </summary>
        [JsonPropertyName("text_index")]
        [Range(0, int.MaxValue, ErrorMessage = "文本索引必须为非负数")]
        public int TextIndex { get; set; }

        /// <summary>
        /// 文本片段的嵌入向量表示
        /// 浮点数组，向量维度通常为512、768、1024等
        /// </summary>
        [JsonPropertyName("embedding")]
        [Required(ErrorMessage = "嵌入向量不能为空")]
        public List<float> Embedding { get; set; } = new List<float>();

        /// <summary>
        /// 获取向量的L2范数（欧几里得范数）
        /// </summary>
        /// <returns>向量的L2范数</returns>
        public double GetL2Norm()
        {
            if (Embedding == null || !Embedding.Any())
                return 0.0;

            return Math.Sqrt(Embedding.Sum(x => x * x));
        }

        /// <summary>
        /// 计算与另一个向量的余弦相似度
        /// </summary>
        /// <param name="other">要比较的另一个嵌入向量</param>
        /// <returns>余弦相似度值，范围在-1到1之间</returns>
        public double CalculateCosineSimilarity(EmbeddingData other)
        {
            if (other == null || Embedding == null || other.Embedding == null || 
                Embedding.Count != other.Embedding.Count)
                return 0.0;

            var dotProduct = 0.0;
            var normA = 0.0;
            var normB = 0.0;

            for (int i = 0; i < Embedding.Count; i++)
            {
                dotProduct += Embedding[i] * other.Embedding[i];
                normA += Embedding[i] * Embedding[i];
                normB += other.Embedding[i] * other.Embedding[i];
            }

            if (normA == 0.0 || normB == 0.0)
                return 0.0;

            return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }

    /// <summary>
    /// API使用统计信息
    /// 包含总token数和输入token数
    /// </summary>
    public class UsageInfo
    {
        /// <summary>
        /// 总token使用数量
        /// 包含输入和处理过程中的所有token
        /// </summary>
        [JsonPropertyName("total_tokens")]
        [Range(0, int.MaxValue, ErrorMessage = "总token数必须为非负数")]
        public int TotalTokens { get; set; }

        /// <summary>
        /// 输入token数量
        /// 仅包含输入文本的token数量
        /// </summary>
        [JsonPropertyName("input_tokens")]
        [Range(0, int.MaxValue, ErrorMessage = "输入token数必须为非负数")]
        public int InputTokens { get; set; }

        /// <summary>
        /// 计算token使用率
        /// </summary>
        /// <returns>输入token占总token的比例</returns>
        public double GetTokenUsageRatio()
        {
            return TotalTokens > 0 ? (double)InputTokens / TotalTokens : 0.0;
        }
    }
}