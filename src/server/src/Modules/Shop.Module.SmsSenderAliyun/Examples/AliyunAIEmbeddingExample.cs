using System;
using System.Collections.Generic;
using System.Text.Json;
using Shop.Module.SmsSenderAliyun.Models;

namespace Shop.Module.SmsSenderAliyun.Examples
{
    /// <summary>
    /// 阿里云AI嵌入向量服务使用示例
    /// 演示如何使用AliyunAIEmbeddingResult及相关类
    /// </summary>
    public class AliyunAIEmbeddingExample
    {
        /// <summary>
        /// 创建示例请求数据
        /// </summary>
        /// <returns>配置好的请求对象</returns>
        public static AliyunAIEmbeddingRequest CreateSampleRequest()
        {
            return new AliyunAIEmbeddingRequest
            {
                Model = "text-embedding-v1",
                Input = new EmbeddingInputData
                {
                    Texts = new List<string>
                    {
                        "阿里云是一家领先的云计算服务提供商。",
                        "机器学习技术在现代应用中越来越重要。"
                    }
                },
                Parameters = new EmbeddingParameters
                {
                    TextType = "document",
                    OutputFormat = "float",
                    Dimension = 1024,
                    BatchSize = 2
                }
            };
        }

        /// <summary>
        /// 创建示例响应数据
        /// </summary>
        /// <returns>配置好的响应对象</returns>
        public static AliyunAIEmbeddingResult CreateSampleResponse()
        {
            return new AliyunAIEmbeddingResult
            {
                RequestId = "fb10a00c-b87c-44a5-98e2-5d3b8c9d1234",
                Output = new EmbeddingOutputData
                {
                    Embeddings = new List<EmbeddingData>
                    {
                        new EmbeddingData
                        {
                            TextIndex = 0,
                            Embedding = GenerateRandomEmbedding(1024)
                        },
                        new EmbeddingData
                        {
                            TextIndex = 1,
                            Embedding = GenerateRandomEmbedding(1024)
                        }
                    }
                },
                Usage = new UsageInfo
                {
                    TotalTokens = 45,
                    InputTokens = 42
                }
            };
        }

        /// <summary>
        /// 生成随机嵌入向量（仅用于演示）
        /// </summary>
        /// <param name="dimension">向量维度</param>
        /// <returns>随机生成的嵌入向量</returns>
        private static List<float> GenerateRandomEmbedding(int dimension)
        {
            var random = new Random();
            var embedding = new List<float>();
            
            for (int i = 0; i < dimension; i++)
            {
                embedding.Add((float)(random.NextDouble() * 2 - 1)); // -1 到 1 之间的随机数
            }
            
            return embedding;
        }

        /// <summary>
        /// 演示JSON序列化和反序列化
        /// </summary>
        public static void DemonstrateJsonSerialization()
        {
            // 创建示例数据
            var request = CreateSampleRequest();
            var response = CreateSampleResponse();

            // 序列化为JSON
            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var responseJson = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            Console.WriteLine("=== 请求JSON示例 ===");
            Console.WriteLine(requestJson);
            Console.WriteLine();

            Console.WriteLine("=== 响应JSON示例 ===");
            Console.WriteLine(responseJson);
            Console.WriteLine();

            // 反序列化验证
            var deserializedRequest = JsonSerializer.Deserialize<AliyunAIEmbeddingRequest>(requestJson);
            var deserializedResponse = JsonSerializer.Deserialize<AliyunAIEmbeddingResult>(responseJson);

            Console.WriteLine("=== 反序列化验证 ===");
            Console.WriteLine($"请求模型: {deserializedRequest.Model}");
            Console.WriteLine($"输入文本数量: {deserializedRequest.Input.Texts.Count}");
            Console.WriteLine($"响应请求ID: {deserializedResponse.RequestId}");
            Console.WriteLine($"嵌入向量数量: {deserializedResponse.GetEmbeddingCount()}");
            Console.WriteLine($"向量维度: {deserializedResponse.GetEmbeddingDimension()}");
            Console.WriteLine($"数据有效性: {deserializedResponse.IsValid()}");
            Console.WriteLine($"Token使用率: {deserializedResponse.Usage.GetTokenUsageRatio():P2}");
        }

        /// <summary>
        /// 演示向量相似度计算
        /// </summary>
        public static void DemonstrateSimilarityCalculation()
        {
            Console.WriteLine("=== 向量相似度计算示例 ===");

            // 创建两个测试向量
            var vector1 = new EmbeddingData
            {
                TextIndex = 0,
                Embedding = new List<float> { 0.8f, 0.6f, 0.0f }
            };

            var vector2 = new EmbeddingData
            {
                TextIndex = 1,
                Embedding = new List<float> { 0.6f, 0.8f, 0.0f }
            };

            var vector3 = new EmbeddingData
            {
                TextIndex = 2,
                Embedding = new List<float> { -0.8f, -0.6f, 0.0f }
            };

            // 计算相似度
            var similarity12 = vector1.CalculateCosineSimilarity(vector2);
            var similarity13 = vector1.CalculateCosineSimilarity(vector3);
            var similarity23 = vector2.CalculateCosineSimilarity(vector3);

            Console.WriteLine($"向量1与向量2的余弦相似度: {similarity12:F4}");
            Console.WriteLine($"向量1与向量3的余弦相似度: {similarity13:F4}");
            Console.WriteLine($"向量2与向量3的余弦相似度: {similarity23:F4}");

            // 计算L2范数
            Console.WriteLine($"向量1的L2范数: {vector1.GetL2Norm():F4}");
            Console.WriteLine($"向量2的L2范数: {vector2.GetL2Norm():F4}");
            Console.WriteLine($"向量3的L2范数: {vector3.GetL2Norm():F4}");
        }

        /// <summary>
        /// 演示数据验证功能
        /// </summary>
        public static void DemonstrateDataValidation()
        {
            Console.WriteLine("=== 数据验证示例 ===");

            // 有效的输入数据
            var validInput = new EmbeddingInputData
            {
                Texts = new List<string> { "有效的文本1", "有效的文本2" }
            };

            // 无效的输入数据
            var invalidInput = new EmbeddingInputData
            {
                Texts = new List<string> { "", "   " }
            };

            Console.WriteLine($"有效输入数据验证结果: {validInput.IsValid()}");
            Console.WriteLine($"有效输入总文本长度: {validInput.GetTotalTextLength()}");
            Console.WriteLine($"无效输入数据验证结果: {invalidInput.IsValid()}");
            Console.WriteLine($"无效输入总文本长度: {invalidInput.GetTotalTextLength()}");

            // 创建完整的响应进行验证
            var completeResponse = CreateSampleResponse();
            var incompleteResponse = new AliyunAIEmbeddingResult();

            Console.WriteLine($"完整响应验证结果: {completeResponse.IsValid()}");
            Console.WriteLine($"不完整响应验证结果: {incompleteResponse.IsValid()}");
        }
    }
}