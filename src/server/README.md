# Module Shop

一个基于 .NET 8.0 构建的简单、跨平台、模块化的商城系统

**跨平台**

> Docker, Windows, Linux, Mac. Microsoft SQL Server, PostgreSQL, MySQL, SQLite

**模块化**

> 该应用程序分为许多自包含模块

**免费并且开源**

> ModuleShop完全免费并且开源

## 后台服务 / API

项目地址：/src/server

#### 接口文档

> 包含对业务的中文说明和注释。

> 支持 Swagger 免输入 token 直接调用接口（自动记忆 token），请使用 MockApi 模拟用户登录，在线调试和调用 API。

在线文档：[https://demo.shopapi.circle.ac.cn/swagger](https://demo.shopapi.circle.ac.cn/swagger)

管理后台：[https://demo.shop.circle.ac.cn](https://demo.shop.circle.ac.cn) *admin/123456*


| API 名称                  | 描述                                                                                   |
|-------------------------|----------------------------------------------------------------------------------------|
| MockApi              | 模拟 API 控制器，仅用于开发环境和演示站点，用来模拟超管、买家用户、重置样本数据、重置测试账号密码等。                                         |
| AccountApi              | 账户API控制器，提供账号、登录、用户信息等功能。                                         |
| AppSettingApi           | 管理后台控制器用于处理应用程序设置相关操作的 API 请求。                                  |
| CacheApi                | 管理后台控制器用于处理缓存相关操作的 API 请求。                                         |
| CountryApi              | 管理后台控制器用于处理国家和省份相关操作的 API 请求。                                    |
| SystemApi               | 管理后台系统服务相关 API                                                               |
| TokenApi                | 管理后台令牌服务相关 API                                                               |
| UploadApi               | 上传服务相关 API                                                                        |
| UserAddressApi          | 用户收货地址相关 API                                                                    |
| UserApi                 | 管理后台用户 API 控制器，包含用户相关操作的接口。                                       |
| WidgetCarouselApi       | 小部件轮播 API 控制器，提供轮播部件相关的接口操作。                                    |
| WidgetHtmlApi           | 小部件 Html API 控制器，提供 HTML 部件相关的接口操作。                                  |
| WidgetInstanceApi       | 小部件实例 API 控制器，提供部件实例相关的接口操作。                                    |
| BrandApi                | 品牌管理API控制器，提供品牌的增删改查等功能。                                           |
| CategoryApi             | 商品分类API控制器，负责商品分类的管理操作，如查询、创建、更新和删除。                    |
| ProductApi              | 商品管理API控制器，负责商品的增删改查等管理操作。                                       |
| ProductAttributeApi     | 商品属性API控制器，负责商品属性的管理操作，如查询、创建、更新和删除。                    |
| ProductAttributeGroupApi| 商品属性组API控制器，负责商品属性组的管理操作，如查询、创建、更新和删除。                |
| ProductAttributeTemplateApi| 产品属性模板的API控制器，负责管理产品属性模板的相关操作。                            |
| ProductOptionApi        | 产品选项API控制器，负责管理产品选项的相关操作。                                         |
| RecentlyViewedApi       | 最近浏览API控制器，负责管理用户的最近浏览商品记录。                                    |
| UnitApi                 | 单位API控制器，负责管理商品单位。                                                      |
| WidgetCategoryApi       | 管理后台控制器用于处理小部件类别相关操作的 API 请求。                                    |
| WidgetProductApi        | 管理后台控制器用于处理小部件产品相关操作的 API 请求。                                    |
| WidgetRecentlyViewedApi | 管理后台控制器用于处理最近浏览小部件相关操作的 API 请求。                                |
| WidgetSimpleProductApi  | 管理后台控制器用于处理简单产品小部件相关操作的 API 请求。                                |
| WishlistApi             | 控制器用于处理心愿单相关操作的 API 请求。                                               |
| StockHistoryApi         | 库存历史 API 控制器，用于管理和查询库存变更历史记录。                                   |
| WarehouseApi            | 仓库管理API控制器，提供仓库的增删改查等功能。                                           |
| FreightTemplateApi      | 运费模板 API 控制器，负责管理运费模板相关的操作。                                       |
| PriceAndDestinationApi  | 运费价格与目的地 API 控制器，负责管理运费模板内的具体运费规则。                         |
| CheckoutApi             | 结算 API 控制器，用于处理购物车、单个商品和订单的结算操作。                             |
| CustomerOrderApi        | 客户订单 API 控制器，用于处理客户订单相关的操作。                                       |
| OrderApi                | 管理员订单 API 控制器，处理订单的管理和操作。                                           |
| OrderHistoryApi         | 订单历史 API 控制器，用于管理和查询订单历史记录。                                       |
| ShipmentApi             | 发货 API 控制器，负责管理发货单相关操作。                                               |
| CartApi                 | 购物车 API 控制器，负责处理购物车相关操作。                                             |
| MPApi                   | 微信小程序API控制器，用于处理微信小程序相关的请求，如登录。                             |
| MpPayApi                | 微信支付API控制器，用于处理微信支付相关的通知和请求。                                    |
| AdminReplyApi           | 管理员回复 API 控制器，用于处理管理员对评论的回复操作。                                  |
| AdminReviewApi          | 管理员评论 API 控制器，用于管理商品评论。                                               |
| ReplyApi                | 评论回复 API 控制器，用于处理评论的回复操作。                                            |
| ReviewApi               | 评论 API 控制器，负责处理评论相关操作。                                                 |
| SupportApi              | 点赞 API 控制器，负责处理点赞相关操作。                                                 |
| FeedbackApi             | 反馈API控制器，用于处理用户反馈相关的请求。                                            |
| SampleData              | 示例数据控制器，用于管理和操作示例数据的重置和生成。                                     |
| CatalogApi              | 商品目录 API 控制器，提供商品目录相关的 API 接口。                                      |
| GoodsApi                | 商品 API 控制器，提供商品相关的 API 接口。                                              |
| HomeApi                 | 首页 API 控制器，提供用于处理首页相关内容的 API 接口。                                   |

#### Visual Studio 2022 和 SQL Server/ MySQL

**先决条件**
 - SQL Server / MySQL 服务器
 - Visual Studio 2022 和 .NET 8


### 项目结构


![商品](/docs/screenshots/devenv_2019-07-06_20-48-19.png)

## 后台管理

项目地址：/src/client

在线预览：[https://demo.shop.circle.ac.cn](https://demo.shop.circle.ac.cn) *admin/123456*

前端框架：[Ant Design Pro](https://pro.ant.design)

使用文档：http://pro.ant.design/docs/getting-started-cn

### 项目截图

![商品](/docs/screenshots/chrome_2019-07-06_20-59-32.png)
![商品](/docs/screenshots/chrome_2019-07-06_20-59-21.png)
![订单](/docs/screenshots/chrome_2019-07-06_20-59-55.png)
![首页显示配置](/docs/screenshots/chrome_2019-07-06_21-00-04.png)

### 功能列表

+ 商品：分类、品牌、单位、选项（销售属性）、属性、属性模板、属性组
+ 销售：订单、物流
+ 内容：首页配置、评论、回复
+ 配置：国家、用户、仓库、运费、高级设置、~~主体化~~、~~本土化~~、~~税~~
+ 系统：样本数据
+ ~~活动：折扣~~
+ ~~首页：首页~~

## 微信小程序

项目地址：/src/mini-program

在线预览（天网商城）：

![小程序码](/docs/screenshots/shop_mp_8.jpg)

### 项目截图

![首页](/docs/screenshots/wechatdevtools_2019-07-06_21-05-55.png)

![分类](/docs/screenshots/wechatdevtools_2019-07-06_21-06-27.png)

![购物车](/docs/screenshots/wechatdevtools_2019-07-06_21-07-05.png)

![商品详情](/docs/screenshots/wechatdevtools_2019-07-07_01-42-31.png)

![订单列表](/docs/screenshots/wechatdevtools_2019-07-07_01-32-17.png)

![订单详情](/docs/screenshots/wechatdevtools_2019-07-06_21-07-35.png)

![评价](/docs/screenshots/wechatdevtools_2019-07-07_01-41-17.png)

![我的](/docs/screenshots/wechatdevtools_2019-07-07_01-35-29.png)


### 功能列表
+ 首页
+ 分类首页、分类商品、新品首发、人气推荐、商品页面等、可通过后台配置
+ 商品详情页面，包含加入购物车、收藏商品、商品评论功能
+ 搜索功能
+ 完整的购物流程，商品的加入、编辑、删除、批量选择，收货地址的选择，下单支付
+ 会员中心（订单、收藏、足迹、收货地址、意见反馈）
....

### 项目结构
```
├─config                
├─lib
│  └─wxParse　　　
├─pages
│  ├─auth
│  │  ├─login
│  │  ├─register
│  │  └─reset
│  ├─brand
│  ├─brandDetail
│  ├─cart
│  ├─catalog
│  ├─category
│  ├─comment
│  ├─goods
│  ├─hotGoods
│  ├─index
│  ├─logs
│  ├─newGoods
│  ├─pay
│  ├─search
│  ├─shopping
│  │  ├─address
│  │  ├─addressAdd
│  │  └─checkout
│  ├─topic
│  ├─topicDetail
│  └─ucenter
│      ├─address
│      ├─addressAdd
│      ├─collect
│      ├─coupon
│      ├─feedback
│      ├─footprint
│      ├─index
│      ├─order
│      └─orderDetail
├─static
│  └─images
└─utils
```

## 交流

喜欢别忘了 Star，有问题可通过微信、公众号、QQ 群联系我，谢谢您的关注。

[ModuleShop交流群](https://jq.qq.com/?_wv=1027&k=5AUnOw5 "ModuleShop交流群（863275860）")（QQ群：863275860）

![ModuleShop交流群二维码](/docs/screenshots/863275860_8_8.png)


## 常见问题

#### 1. 如何初始化数据库？

> 确保安装了 EF Core，使用 EF 更新工具更新数据库。 


详细参考：<https://learn.microsoft.com/zh-cn/ef/core/>

#### 2. 在线演示站点登录不了怎么办？

> 打开 swagger https://demo.shopapi.circle.ac.cn/swagger 文档，调用 [[重置密码](https://demo.shop.circle.ac.cn/api/mock/reset-admin-password)] 密码接口，然后重新登录。

登录地址：[https://demo.shop.circle.ac.cn](https://demo.shop.circle.ac.cn) *admin/123456*