#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Shop.WebApi/Shop.WebApi.csproj", "src/Shop.WebApi/"]
COPY ["src/Modules/Shop.Module.ApiProfilerAuth/Shop.Module.ApiProfilerAuth.csproj", "src/Modules/Shop.Module.ApiProfilerAuth/"]
COPY ["src/Shop.Infrastructure/Shop.Infrastructure.csproj", "src/Shop.Infrastructure/"]
COPY ["src/Modules/Shop.Module.ApiProfiler/Shop.Module.ApiProfiler.csproj", "src/Modules/Shop.Module.ApiProfiler/"]
COPY ["src/Modules/Shop.Module.BasicAuth/Shop.Module.BasicAuth.csproj", "src/Modules/Shop.Module.BasicAuth/"]
COPY ["src/Modules/Shop.Module.Catalog.Abstractions/Shop.Module.Catalog.Abstractions.csproj", "src/Modules/Shop.Module.Catalog.Abstractions/"]
COPY ["src/Modules/Shop.Module.Core.Abstractions/Shop.Module.Core.Abstractions.csproj", "src/Modules/Shop.Module.Core.Abstractions/"]
COPY ["src/Modules/Shop.Module.Shipping.Abstractions/Shop.Module.Shipping.Abstractions.csproj", "src/Modules/Shop.Module.Shipping.Abstractions/"]
COPY ["src/Modules/Shop.Module.Catalog/Shop.Module.Catalog.csproj", "src/Modules/Shop.Module.Catalog/"]
COPY ["src/Modules/Shop.Module.Inventory.Abstractions/Shop.Module.Inventory.Abstractions.csproj", "src/Modules/Shop.Module.Inventory.Abstractions/"]
COPY ["src/Modules/Shop.Module.MQ.Abstractions/Shop.Module.MQ.Abstractions.csproj", "src/Modules/Shop.Module.MQ.Abstractions/"]
COPY ["src/Modules/Shop.Module.Orders.Abstractions/Shop.Module.Orders.Abstractions.csproj", "src/Modules/Shop.Module.Orders.Abstractions/"]
COPY ["src/Modules/Shop.Module.Payments.Abstractions/Shop.Module.Payments.Abstractions.csproj", "src/Modules/Shop.Module.Payments.Abstractions/"]
COPY ["src/Modules/Shop.Module.Core.MiniProgram/Shop.Module.Core.MiniProgram.csproj", "src/Modules/Shop.Module.Core.MiniProgram/"]
COPY ["src/Modules/Shop.Module.Core/Shop.Module.Core.csproj", "src/Modules/Shop.Module.Core/"]
COPY ["src/Modules/Shop.Module.Schedule.Abstractions/Shop.Module.Schedule.Abstractions.csproj", "src/Modules/Shop.Module.Schedule.Abstractions/"]
COPY ["src/Modules/Shop.Module.EmailSenderSmtp/Shop.Module.EmailSenderSmtp.csproj", "src/Modules/Shop.Module.EmailSenderSmtp/"]
COPY ["src/Modules/Shop.Module.Feedbacks.Abstractions/Shop.Module.Feedbacks.Abstractions.csproj", "src/Modules/Shop.Module.Feedbacks.Abstractions/"]
COPY ["src/Modules/Shop.Module.Feedback/Shop.Module.Feedbacks.csproj", "src/Modules/Shop.Module.Feedback/"]
COPY ["src/Modules/Shop.Module.Hangfire/Shop.Module.Hangfire.csproj", "src/Modules/Shop.Module.Hangfire/"]
COPY ["src/Modules/Shop.Module.Inventory/Shop.Module.Inventory.csproj", "src/Modules/Shop.Module.Inventory/"]
COPY ["src/Modules/Shop.Module.MassTransitMQ/Shop.Module.MassTransitMQ.csproj", "src/Modules/Shop.Module.MassTransitMQ/"]
COPY ["src/Modules/Shop.Module.Reviews.Abstractions/Shop.Module.Reviews.Abstractions.csproj", "src/Modules/Shop.Module.Reviews.Abstractions/"]
COPY ["src/Modules/Shop.Module.Orders/Shop.Module.Orders.csproj", "src/Modules/Shop.Module.Orders/"]
COPY ["src/Modules/Shop.Module.Shipments.Abstractions/Shop.Module.Shipments.Abstractions.csproj", "src/Modules/Shop.Module.Shipments.Abstractions/"]
COPY ["src/Modules/Shop.Module.ShoppingCart.Abstractions/Shop.Module.ShoppingCart.Abstractions.csproj", "src/Modules/Shop.Module.ShoppingCart.Abstractions/"]
COPY ["src/Modules/Shop.Module.Payments.Payments/Shop.Module.Payments.Payments.csproj", "src/Modules/Shop.Module.Payments.Payments/"]
COPY ["src/Modules/Shop.Module.RateLimit/Shop.Module.RateLimit.csproj", "src/Modules/Shop.Module.RateLimit/"]
COPY ["src/Modules/Shop.Module.Reviews/Shop.Module.Reviews.csproj", "src/Modules/Shop.Module.Reviews/"]
COPY ["src/Modules/Shop.Module.SampleData/Shop.Module.SampleData.csproj", "src/Modules/Shop.Module.SampleData/"]
COPY ["src/Modules/Shop.Module.Shipments/Shop.Module.Shipments.csproj", "src/Modules/Shop.Module.Shipments/"]
COPY ["src/Modules/Shop.Module.Shipping/Shop.Module.Shipping.csproj", "src/Modules/Shop.Module.Shipping/"]
COPY ["src/Modules/Shop.Module.ShoppingCart/Shop.Module.ShoppingCart.csproj", "src/Modules/Shop.Module.ShoppingCart/"]
COPY ["src/Modules/Shop.Module.SmsSenderAliyun/Shop.Module.SmsSenderAliyun.csproj", "src/Modules/Shop.Module.SmsSenderAliyun/"]
COPY ["src/Modules/Shop.Module.StorageGitHub/Shop.Module.StorageGitHub.csproj", "src/Modules/Shop.Module.StorageGitHub/"]
COPY ["src/Modules/Shop.Module.StorageLocal/Shop.Module.StorageLocal.csproj", "src/Modules/Shop.Module.StorageLocal/"]
RUN dotnet restore "./src/Shop.WebApi/./Shop.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Shop.WebApi"
RUN dotnet build "./Shop.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Shop.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.WebApi.dll"]