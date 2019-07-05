## ModuleShop

一个基于 .NET Core构建的简单、跨平台、模块化的商城系统

**跨平台**

Docker, Windows, Linux, Mac. Microsoft SQL Server, PostgreSQL, MySQL, SQLite

**模块化**

该应用程序分为许多自包含模块

**免费并且开源**

ModuleShop完全免费并且开源

## 后台API

项目地址：[https://github.com/trueai-org/module-shop-admin](https://github.com/trueai-org/module-shop-admin)

在线预览：[https://demo.shopapi.circle.ac.cn/swagger](https://demo.shopapi.circle.ac.cn/swagger)

### 项目结构

![商品](https://gogs.circle.ac.cn/gogs/data/raw/master/images/devenv_2019-05-23_18-20-12.png)

## 后台前端

项目地址：[https://github.com/trueai-org/module-shop](https://github.com/trueai-org/module-shop)

在线预览：[https://demo.shop.circle.ac.cn](https://demo.shop.circle.ac.cn) *admin/123456*

前端框架：[Ant Design Pro](https://pro.ant.design)

使用文档：http://pro.ant.design/docs/getting-started-cn

### 项目截图

![商品](https://gogs.circle.ac.cn/gogs/data/raw/master/images/chrome_2019-05-23_18-14-51.png)
![订单](https://gogs.circle.ac.cn/gogs/data/raw/master/images/chrome_2019-05-23_18-14-19.png)
![首页显示配置](https://gogs.circle.ac.cn/gogs/data/raw/master/images/chrome_2019-05-23_18-13-50.png)

### 功能列表

+ 商品：分类、品牌、单位、选项（销售属性）、属性、属性模板、属性组
+ 销售：订单、物流
+ 内容：首页配置、评论、回复
+ 配置：国家、用户、仓库、运费、高级设置、~~主体化~~、~~本土化~~、~~税~~
+ 系统：样本数据
+ ~~活动：折扣~~
+ ~~首页：首页~~

## 微信小程序

项目地址：[https://github.com/trueai-org/module-shop-mini-program](https://github.com/trueai-org/module-shop-mini-program)

在线预览（天网商城）：

![小程序码](https://gogs.circle.ac.cn/gogs/data/raw/master/images/shop_mp_8.jpg)

### 项目截图

![首页](https://gogs.circle.ac.cn/gogs/data/raw/master/images/wechatdevtools_2019-04-26_17-44-30.png)

![分类](https://gogs.circle.ac.cn/gogs/data/raw/master/images/wechatdevtools_2019-04-26_17-45-37.png)

![购物车](https://gogs.circle.ac.cn/gogs/data/raw/master/images/wechatdevtools_2019-04-26_17-50-15.png)

![商品详情](https://gogs.circle.ac.cn/gogs/data/raw/master/images/wechatdevtools_2019-04-26_17-50-50.png)

![订单列表](https://gogs.circle.ac.cn/gogs/data/raw/master/images/wechatdevtools_2019-04-26_17-54-33.png)

![评价](https://gogs.circle.ac.cn/gogs/data/raw/master/images/wechatdevtools_2019-04-26_17-56-43.png)

![我的](https://gogs.circle.ac.cn/gogs/data/raw/master/images/wechatdevtools_2019-04-26_17-57-19.png)


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
