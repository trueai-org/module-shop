using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using Shop.Module.SmsSenderAliyun.Models;

namespace Shop.Module.Core.Tests
{
    /// <summary>
    /// 阿里云AI嵌入向量结果类的单元测试
    /// </summary>
    public class AliyunAIEmbeddingResultTests
    {
        /// <summary>
        /// 测试AliyunAIEmbeddingResult的基本属性
        /// </summary>
        [Fact]
        public void AliyunAIEmbeddingResult_BasicProperties_ShouldWork()
        {
            // Arrange
            var result = new AliyunAIEmbeddingResult
            {
                RequestId = "test-request-id",
                Output = new EmbeddingOutputData
                {
                    Embeddings = new List<EmbeddingData>
                    {
                        new EmbeddingData
                        {
                            TextIndex = 0,
                            Embedding = new List<float> { 0.1f, 0.2f, 0.3f }
                        }
                    }
                },
                Usage = new UsageInfo
                {
                    TotalTokens = 100,
                    InputTokens = 80
                }
            };

            // Act & Assert
            Assert.Equal("test-request-id", result.RequestId);
            Assert.NotNull(result.Output);
            Assert.NotNull(result.Usage);
            Assert.Equal(100, result.Usage.TotalTokens);
            Assert.Equal(80, result.Usage.InputTokens);
        }

        /// <summary>
        /// 测试IsValid方法
        /// </summary>
        [Fact]
        public void AliyunAIEmbeddingResult_IsValid_ShouldReturnCorrectValue()
        {
            // Arrange
            var validResult = new AliyunAIEmbeddingResult
            {
                RequestId = "test-request-id",
                Output = new EmbeddingOutputData
                {
                    Embeddings = new List<EmbeddingData>
                    {
                        new EmbeddingData { TextIndex = 0, Embedding = new List<float> { 0.1f } }
                    }
                },
                Usage = new UsageInfo { TotalTokens = 10, InputTokens = 8 }
            };

            var invalidResult = new AliyunAIEmbeddingResult
            {
                RequestId = "",
                Output = null,
                Usage = null
            };

            // Act & Assert
            Assert.True(validResult.IsValid());
            Assert.False(invalidResult.IsValid());
        }

        /// <summary>
        /// 测试GetEmbeddingCount方法
        /// </summary>
        [Fact]
        public void AliyunAIEmbeddingResult_GetEmbeddingCount_ShouldReturnCorrectCount()
        {
            // Arrange
            var result = new AliyunAIEmbeddingResult
            {
                Output = new EmbeddingOutputData
                {
                    Embeddings = new List<EmbeddingData>
                    {
                        new EmbeddingData(),
                        new EmbeddingData()
                    }
                }
            };

            var emptyResult = new AliyunAIEmbeddingResult();

            // Act & Assert
            Assert.Equal(2, result.GetEmbeddingCount());
            Assert.Equal(0, emptyResult.GetEmbeddingCount());
        }

        /// <summary>
        /// 测试GetEmbeddingDimension方法
        /// </summary>
        [Fact]
        public void AliyunAIEmbeddingResult_GetEmbeddingDimension_ShouldReturnCorrectDimension()
        {
            // Arrange
            var result = new AliyunAIEmbeddingResult
            {
                Output = new EmbeddingOutputData
                {
                    Embeddings = new List<EmbeddingData>
                    {
                        new EmbeddingData
                        {
                            Embedding = new List<float> { 0.1f, 0.2f, 0.3f, 0.4f }
                        }
                    }
                }
            };

            var emptyResult = new AliyunAIEmbeddingResult();

            // Act & Assert
            Assert.Equal(4, result.GetEmbeddingDimension());
            Assert.Equal(0, emptyResult.GetEmbeddingDimension());
        }

        /// <summary>
        /// 测试EmbeddingData的L2范数计算
        /// </summary>
        [Fact]
        public void EmbeddingData_GetL2Norm_ShouldCalculateCorrectly()
        {
            // Arrange
            var embeddingData = new EmbeddingData
            {
                Embedding = new List<float> { 3.0f, 4.0f } // L2范数应该为5.0
            };

            var emptyEmbeddingData = new EmbeddingData();

            // Act
            var norm = embeddingData.GetL2Norm();
            var emptyNorm = emptyEmbeddingData.GetL2Norm();

            // Assert
            Assert.Equal(5.0, norm, 1e-6);
            Assert.Equal(0.0, emptyNorm);
        }

        /// <summary>
        /// 测试余弦相似度计算
        /// </summary>
        [Fact]
        public void EmbeddingData_CalculateCosineSimilarity_ShouldCalculateCorrectly()
        {
            // Arrange
            var embedding1 = new EmbeddingData
            {
                Embedding = new List<float> { 1.0f, 0.0f }
            };

            var embedding2 = new EmbeddingData
            {
                Embedding = new List<float> { 1.0f, 0.0f }
            };

            var embedding3 = new EmbeddingData
            {
                Embedding = new List<float> { 0.0f, 1.0f }
            };

            // Act
            var similarity1 = embedding1.CalculateCosineSimilarity(embedding2); // 应该为1.0
            var similarity2 = embedding1.CalculateCosineSimilarity(embedding3); // 应该为0.0

            // Assert
            Assert.Equal(1.0, similarity1, 1e-6);
            Assert.Equal(0.0, similarity2, 1e-6);
        }

        /// <summary>
        /// 测试UsageInfo的token使用率计算
        /// </summary>
        [Fact]
        public void UsageInfo_GetTokenUsageRatio_ShouldCalculateCorrectly()
        {
            // Arrange
            var usage = new UsageInfo
            {
                TotalTokens = 100,
                InputTokens = 80
            };

            var emptyUsage = new UsageInfo();

            // Act
            var ratio = usage.GetTokenUsageRatio();
            var emptyRatio = emptyUsage.GetTokenUsageRatio();

            // Assert
            Assert.Equal(0.8, ratio, 1e-6);
            Assert.Equal(0.0, emptyRatio);
        }

        /// <summary>
        /// 测试JSON序列化和反序列化
        /// </summary>
        [Fact]
        public void AliyunAIEmbeddingResult_JsonSerialization_ShouldWork()
        {
            // Arrange
            var originalResult = new AliyunAIEmbeddingResult
            {
                RequestId = "test-request-id",
                Output = new EmbeddingOutputData
                {
                    Embeddings = new List<EmbeddingData>
                    {
                        new EmbeddingData
                        {
                            TextIndex = 0,
                            Embedding = new List<float> { 0.1f, 0.2f, 0.3f }
                        }
                    }
                },
                Usage = new UsageInfo
                {
                    TotalTokens = 100,
                    InputTokens = 80
                }
            };

            // Act
            var json = JsonSerializer.Serialize(originalResult);
            var deserializedResult = JsonSerializer.Deserialize<AliyunAIEmbeddingResult>(json);

            // Assert
            Assert.NotNull(deserializedResult);
            Assert.Equal(originalResult.RequestId, deserializedResult.RequestId);
            Assert.Equal(originalResult.Usage.TotalTokens, deserializedResult.Usage.TotalTokens);
            Assert.Equal(originalResult.Usage.InputTokens, deserializedResult.Usage.InputTokens);
            Assert.Equal(originalResult.Output.Embeddings.Count, deserializedResult.Output.Embeddings.Count);
            Assert.Equal(originalResult.Output.Embeddings[0].TextIndex, deserializedResult.Output.Embeddings[0].TextIndex);
            Assert.Equal(originalResult.Output.Embeddings[0].Embedding.Count, deserializedResult.Output.Embeddings[0].Embedding.Count);
        }

        /// <summary>
        /// 测试EmbeddingInputData的验证方法
        /// </summary>
        [Fact]
        public void EmbeddingInputData_IsValid_ShouldReturnCorrectValue()
        {
            // Arrange
            var validInput = new EmbeddingInputData
            {
                Texts = new List<string> { "测试文本1", "测试文本2" }
            };

            var invalidInput1 = new EmbeddingInputData
            {
                Texts = new List<string>()
            };

            var invalidInput2 = new EmbeddingInputData
            {
                Texts = new List<string> { "", "  " }
            };

            // Act & Assert
            Assert.True(validInput.IsValid());
            Assert.False(invalidInput1.IsValid());
            Assert.False(invalidInput2.IsValid());
        }

        /// <summary>
        /// 测试EmbeddingInputData的总文本长度计算
        /// </summary>
        [Fact]
        public void EmbeddingInputData_GetTotalTextLength_ShouldCalculateCorrectly()
        {
            // Arrange
            var input = new EmbeddingInputData
            {
                Texts = new List<string> { "hello", "world" } // 总长度为10
            };

            var emptyInput = new EmbeddingInputData();

            // Act
            var totalLength = input.GetTotalTextLength();
            var emptyLength = emptyInput.GetTotalTextLength();

            // Assert
            Assert.Equal(10, totalLength);
            Assert.Equal(0, emptyLength);
        }
    }
}