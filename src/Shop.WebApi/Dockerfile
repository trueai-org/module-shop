#FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM microsoft/dotnet:2.2-sdk AS build
#WORKDIR /src
#COPY ["src/Shop.WebApi/Shop.WebApi.csproj", "src/Shop.WebApi/"]
#COPY ["src/Shop.Infrastructure/Shop.Infrastructure.csproj", "src/Shop.Infrastructure/"]
#COPY ["src/Modules/Shop.Module.Core/Shop.Module.Core.csproj", "src/Modules/Shop.Module.Core/"]
#RUN dotnet restore "src/Shop.WebApi/Shop.WebApi.csproj"
#COPY . .
#WORKDIR "/src/src/Shop.WebApi"
#RUN dotnet build "Shop.WebApi.csproj" -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish "Shop.WebApi.csproj" -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Shop.WebApi.dll"]

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "Shop.WebApi.dll"]