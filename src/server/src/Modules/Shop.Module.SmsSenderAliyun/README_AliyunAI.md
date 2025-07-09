# 阿里云百炼AI嵌入向量服务 - 数据模型

本文档描述了阿里云百炼AI嵌入向量服务的数据模型实现，包括请求和响应类的详细说明。

## 概述

该模块提供了一套完整的数据模型来支持阿里云百炼Embedding API的调用，包括：

- **请求模型**：`AliyunAIEmbeddingRequest` - 用于构建API请求
- **响应模型**：`AliyunAIEmbeddingResult` - 用于解析API响应  
- **配置模型**：`AliyunAIServiceOptions` - 用于配置服务连接
- **示例代码**：`AliyunAIEmbeddingExample` - 演示如何使用这些模型

## 核心特性

### 1. 符合C#命名规范
- 使用Pascal命名法（PascalCase）
- 避免使用下划线等不规范命名
- 提供清晰、具描述性的属性名

### 2. 完整的中文注释
- 所有类、属性和方法都有详细的中文注释
- 包含参数说明、返回值说明和使用示例
- 参考阿里云官方API文档

### 3. JSON序列化支持
- 使用`System.Text.Json`进行序列化/反序列化
- 通过`JsonPropertyName`特性保持与API的兼容性
- 支持自定义序列化选项

### 4. 数据验证与错误处理
- 使用`DataAnnotations`进行数据验证
- 提供完整性检查方法
- 包含错误处理机制

### 5. 实用工具方法
- 向量相似度计算（余弦相似度）
- L2范数计算
- Token使用率统计
- 数据完整性验证

## 主要类说明

### AliyunAIEmbeddingResult
主要的响应结果类，包含：
- `RequestId`：请求ID
- `Output`：嵌入向量输出数据
- `Usage`：使用统计信息
- `IsValid()`：验证数据完整性
- `GetEmbeddingCount()`：获取向量数量
- `GetEmbeddingDimension()`：获取向量维度

### EmbeddingData
单个嵌入向量数据，包含：
- `TextIndex`：文本索引
- `Embedding`：向量数据
- `GetL2Norm()`：计算L2范数
- `CalculateCosineSimilarity()`：计算余弦相似度

### AliyunAIEmbeddingRequest
请求参数类，包含：
- `Model`：模型名称
- `Input`：输入数据
- `Parameters`：请求参数

### AliyunAIServiceOptions
服务配置类，包含：
- `ApiKey`：API密钥
- `Endpoint`：服务端点
- `WorkspaceId`：工作空间ID
- `TimeoutSeconds`：超时时间
- `RetryCount`：重试次数

## 使用示例

### 1. 创建请求
```csharp
var request = new AliyunAIEmbeddingRequest
{
    Model = "text-embedding-v1",
    Input = new EmbeddingInputData
    {
        Texts = new List<string> { "要处理的文本1", "要处理的文本2" }
    },
    Parameters = new EmbeddingParameters
    {
        TextType = "document",
        OutputFormat = "float",
        Dimension = 1024
    }
};
```

### 2. 处理响应
```csharp
var response = JsonSerializer.Deserialize<AliyunAIEmbeddingResult>(responseJson);

if (response.IsValid())
{
    Console.WriteLine($"获得 {response.GetEmbeddingCount()} 个向量");
    Console.WriteLine($"向量维度: {response.GetEmbeddingDimension()}");
    Console.WriteLine($"Token使用率: {response.Usage.GetTokenUsageRatio():P2}");
}
```

### 3. 计算向量相似度
```csharp
var vector1 = response.Output.Embeddings[0];
var vector2 = response.Output.Embeddings[1];
var similarity = vector1.CalculateCosineSimilarity(vector2);
Console.WriteLine($"相似度: {similarity:F4}");
```

### 4. 配置服务
```csharp
var options = new AliyunAIServiceOptions
{
    ApiKey = "your-api-key",
    Endpoint = "https://dashscope.aliyuncs.com",
    WorkspaceId = "your-workspace-id",
    TimeoutSeconds = 30,
    RetryCount = 3
};
```

## JSON格式示例

### 请求JSON
```json
{
  "model": "text-embedding-v1",
  "input": {
    "texts": ["文本1", "文本2"]
  },
  "parameters": {
    "text_type": "document",
    "output_format": "float",
    "dimension": 1024,
    "batch_size": 2
  }
}
```

### 响应JSON
```json
{
  "request_id": "fb10a00c-b87c-44a5-98e2-5d3b8c9d1234",
  "output": {
    "embeddings": [
      {
        "text_index": 0,
        "embedding": [0.1, 0.2, 0.3, ...]
      },
      {
        "text_index": 1,
        "embedding": [0.4, 0.5, 0.6, ...]
      }
    ]
  },
  "usage": {
    "total_tokens": 45,
    "input_tokens": 42
  }
}
```

## 测试

项目包含完整的单元测试，覆盖：
- 基本属性和方法测试
- 数据验证测试
- JSON序列化/反序列化测试
- 向量计算测试
- 错误处理测试

运行测试：
```bash
dotnet test Shop.Module.Core.Tests
```

## 更新日志

### 版本优化内容
1. **命名规范优化**：从使用下划线的命名（如`_object`）改为Pascal命名法
2. **添加中文注释**：为所有类、属性和方法添加详细中文文档
3. **JSON序列化支持**：添加`JsonPropertyName`特性保持API兼容性
4. **数据验证**：使用`DataAnnotations`进行输入验证
5. **实用工具方法**：添加向量计算、相似度计算等实用功能
6. **错误处理**：改进错误处理和数据完整性检查
7. **代码结构优化**：提高可读性和维护性

## 参考文档

- [阿里云百炼API文档](https://help.aliyun.com/document_detail/2842587.html)
- [System.Text.Json 文档](https://docs.microsoft.com/en-us/dotnet/api/system.text.json)
- [DataAnnotations 文档](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations)