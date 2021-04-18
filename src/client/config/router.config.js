export default [
  // user
  {
    path: '/user',
    component: '../layouts/UserLayout',
    routes: [
      { path: '/user', redirect: '/user/login' },
      { path: '/user/login', component: './User/Login' },
      { path: '/user/register', component: './User/Register' },
      { path: '/user/register-result', component: './User/RegisterResult' },
      { path: '/user/confirm-email', component: './User/ConfirmEmail' },
      { path: '/user/forgot-password', component: './User/ForgotPassword' },
      { path: '/user/reset-password', component: './User/ResetPassword' },
      { path: '/user/add-email', component: './User/AddEmail' }
    ],
  },

  // app
  {
    path: '/',
    component: '../layouts/BasicLayout',
    Routes: ['src/pages/Authorized'],
    authority: ['admin', 'user'],
    routes: [

      { path: '/', redirect: '/index' },

      //后台首页
      {
        name: 'index',
        icon: 'home',
        path: '/index',
        component: './Home/Index'
      },

      //结果页
      {
        name: 'result',
        icon: 'check-circle-o',
        path: '/result',
        hideInMenu: true,
        routes: [
          // result
          {
            path: '/result/success',
            name: 'success',
            component: './Result/Success',
          },
          { path: '/result/fail', name: 'fail', component: './Result/Error' },
        ],
      },

      //异常页
      {
        name: 'exception',
        icon: 'warning',
        path: '/exception',
        hideInMenu: true,
        routes: [
          // exception
          {
            path: '/exception/403',
            name: 'not-permission',
            component: './Exception/403',
          },
          {
            path: '/exception/404',
            name: 'not-find',
            component: './Exception/404',
          },
          {
            path: '/exception/500',
            name: 'server-error',
            component: './Exception/500',
          },
          {
            path: '/exception/trigger',
            name: 'trigger',
            hideInMenu: true,
            component: './Exception/TriggerException',
          },
        ],
      },

      //个人页
      {
        name: 'account',
        icon: 'user',
        path: '/account',
        hideInMenu: true,
        routes: [
          {
            path: '/account/settings',
            name: 'settings',
            component: './Account/Settings/Info',
            routes: [
              {
                path: '/account/settings',
                redirect: '/account/settings/base',
              },
              {
                path: '/account/settings/base',
                component: './Account/Settings/BaseView',
              },
              {
                path: '/account/settings/security',
                component: './Account/Settings/SecurityView',
              },
              {
                path: '/account/settings/binding',
                component: './Account/Settings/BindingView',
              },
              {
                path: '/account/settings/notification',
                component: './Account/Settings/NotificationView',
              },
            ],
          },
        ],
      },

      // 商品目录
      {
        name: 'catalog',
        icon: 'gold',
        path: '/catalog',
        routes: [
          {
            path: '/catalog/product',
            name: 'product',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/catalog/product',
                redirect: '/catalog/product/list',
              },
              {
                path: '/catalog/product/list',
                name: 'list',
                component: './Catalog/Product/List',
              },
              // {
              //   path: '/catalog/product/add',
              //   name: 'add',
              //   component: './Catalog/Product/Add'
              // },
              // {
              //   path: '/catalog/product/edit',
              //   name: 'edit',
              //   component: './Catalog/Product/Edit'
              // },
              {
                path: '/catalog/product/info',
                name: 'info',
                component: './Catalog/Product/Info',
              },
            ],
          },
          {
            path: '/catalog/category',
            name: 'category',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/catalog/category',
                redirect: '/catalog/category/list',
              },
              {
                path: '/catalog/category/list',
                name: 'list',
                component: './Catalog/Category/List',
              },
              {
                path: '/catalog/category/add',
                name: 'add',
                component: './Catalog/Category/Add',
              },
              {
                path: '/catalog/category/edit',
                name: 'edit',
                component: './Catalog/Category/Edit',
              },
            ],
          },
          {
            path: '/catalog/brand',
            name: 'brand',
            component: './Catalog/Brand/List',
          },
          {
            path: '/catalog/product-option',
            name: 'product-option',
            // component: './Catalog/ProductAttribute/List',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/catalog/product-option',
                redirect: '/catalog/product-option/list',
              },
              {
                path: '/catalog/product-option/list',
                name: 'list',
                component: './Catalog/ProductOption/List',
              },
              {
                path: '/catalog/product-option/data',
                name: 'data',
                component: './Catalog/ProductOption/Data',
              },
            ],
          },
          {
            path: '/catalog/product-attribute-group',
            name: 'product-attribute-group',
            component: './Catalog/ProductAttributeGroup/List',
          },
          {
            path: '/catalog/product-attribute',
            name: 'product-attribute',
            // component: './Catalog/ProductAttribute/List',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/catalog/product-attribute',
                redirect: '/catalog/product-attribute/list',
              },
              {
                path: '/catalog/product-attribute/list',
                name: 'list',
                component: './Catalog/ProductAttribute/List',
              },
              {
                path: '/catalog/product-attribute/data',
                name: 'data',
                component: './Catalog/ProductAttribute/Data',
              },
            ],
          },
          {
            path: '/catalog/product-attribute-template',
            name: 'product-attribute-template',
            component: './Catalog/ProductAttributeTemplate/List',
          },
          {
            path: '/catalog/unit',
            name: 'unit',
            component: './Catalog/Unit/List',
          },
        ],
      },

      // 销售
      {
        name: 'sale',
        icon: 'shopping',
        path: '/sale',
        routes: [
          {
            path: '/sale/order',
            name: 'order',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/sale/order',
                redirect: '/sale/order/list',
              },
              {
                path: '/sale/order/list',
                name: 'list',
                component: './Sale/Order/List',
              },
              {
                path: '/sale/order/add',
                name: 'add',
                component: './Sale/Order/Add',
              },
              {
                path: '/sale/order/edit',
                name: 'edit',
                component: './Sale/Order/Edit',
              },
              {
                path: '/sale/order/detail',
                name: 'detail',
                component: './Sale/Order/Detail',
              },
              {
                path: '/sale/order/shipment',
                name: 'shipment',
                component: './Sale/Order/Shipment',
              },
            ],
          },
          {
            path: '/sale/shipment',
            name: 'shipment',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/sale/shipment',
                redirect: '/sale/shipment/list',
              },
              {
                path: '/sale/shipment/list',
                name: 'list',
                component: './Sale/Shipment/List'
              },
              // {
              //   path: '/sale/shipment/add',
              //   name: 'add',
              //   component: './Sale/Shipment/Add'
              // },
            ],
          },
        ],
      },

      // 活动
      {
        name: 'promotion',
        icon: 'tags',
        path: '/promotion',
        routes: [
          {
            path: 'discount',
            name: 'discount',
            component: './Promotion/Discount/List',
          }
        ],
      },

      // 内容管理
      {
        name: 'content',
        icon: 'profile',
        path: '/content',
        routes: [
          {
            path: 'widget',
            name: 'widget',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/content/widget',
                redirect: '/content/widget/list',
              },
              {
                path: '/content/widget/list',
                name: 'list',
                component: './Content/Widget/List',
              },

              // 分类部件
              {
                path: '/content/widget/widget-category-create',
                redirect: '/content/widget/widget-category',
              },
              {
                path: '/content/widget/widget-category-edit/:id',
                redirect: '/content/widget/widget-category?id=:id',
              },
              {
                path: '/content/widget/widget-category',
                name: 'widget-category',
                component: './Content/Widget/WidgetCategory',
              },

              // html部件
              {
                path: '/content/widget/widget-html-create',
                redirect: '/content/widget/widget-html',
              },
              {
                path: '/content/widget/widget-html-edit/:id',
                redirect: '/content/widget/widget-html?id=:id',
              },
              {
                path: '/content/widget/widget-html',
                name: 'widget-html',
                component: './Content/Widget/WidgetHtml',
              },

              // 最近浏览部件
              {
                path: '/content/widget/widget-recently-viewed-create',
                redirect: '/content/widget/widget-recently-viewed',
              },
              {
                path: '/content/widget/widget-recently-viewed-edit/:id',
                redirect: '/content/widget/widget-recently-viewed?id=:id',
              },
              {
                path: '/content/widget/widget-recently-viewed',
                name: 'widget-recently-viewed',
                component: './Content/Widget/WidgetRecentlyViewed',
              },

              // 简单商品部件
              {
                path: '/content/widget/widget-simple-product-create',
                redirect: '/content/widget/widget-simple-product',
              },
              {
                path: '/content/widget/widget-simple-product-edit/:id',
                redirect: '/content/widget/widget-simple-product?id=:id',
              },
              {
                path: '/content/widget/widget-simple-product',
                name: 'widget-simple-product',
                component: './Content/Widget/WidgetSimpleProduct',
              },

              // 商品部件
              {
                path: '/content/widget/widget-product-create',
                redirect: '/content/widget/widget-product',
              },
              {
                path: '/content/widget/widget-product-edit/:id',
                redirect: '/content/widget/widget-product?id=:id',
              },
              {
                path: '/content/widget/widget-product',
                name: 'widget-product',
                component: './Content/Widget/WidgetProduct',
              },

              // 轮播部件
              {
                path: '/content/widget/widget-carousel-create',
                redirect: '/content/widget/widget-carousel',
              },
              {
                path: '/content/widget/widget-carousel-edit/:id',
                redirect: '/content/widget/widget-carousel?id=:id',
              },
              {
                path: '/content/widget/widget-carousel',
                name: 'widget-carousel',
                component: './Content/Widget/WidgetCarousel',
              },

            ],
          },
          {
            path: '/content/review',
            name: 'review',
            component: './Content/Review/List',
          },
          {
            path: '/content/reply',
            name: 'reply',
            component: './Content/Reply/List',
          }
        ],
      },

      // 商城配置
      {
        name: 'configuration',
        icon: 'tool',
        path: '/configuration',
        routes: [
          {
            path: '/configuration/country',
            name: 'country',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/configuration/country',
                redirect: '/configuration/country/list',
              },
              {
                path: '/configuration/country/list',
                name: 'list',
                component: './Configuration/Country/List',
              },
              {
                path: '/configuration/country/add',
                name: 'add',
                component: './Configuration/Country/Add',
              },
              {
                path: '/configuration/country/edit',
                name: 'edit',
                component: './Configuration/Country/Edit',
              },
              {
                path: '/configuration/country/province',
                name: 'province',
                component: './Configuration/Country/Province',
              },
            ],
          },

          //用户
          {
            path: '/configuration/user',
            name: 'user',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/configuration/user',
                redirect: '/configuration/user/list',
              },
              {
                path: '/configuration/user/list',
                name: 'list',
                component: './Configuration/User/List',
              },
            ],
          },

          //仓库
          {
            path: '/configuration/warehouse',
            name: 'warehouse',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/configuration/warehouse',
                redirect: '/configuration/warehouse/list',
              },
              {
                path: '/configuration/warehouse/list',
                name: 'list',
                component: './Configuration/Warehouse/List',
              },
            ],
          },

          //运费模板
          {
            path: '/configuration/freight-template',
            name: 'freight-template',
            hideChildrenInMenu: true,
            routes: [
              {
                path: '/configuration/freight-template',
                redirect: '/configuration/freight-template/list',
              },
              {
                path: '/configuration/freight-template/list',
                name: 'list',
                component: './Configuration/FreightTemplate/List',
              },
              {
                path: '/configuration/freight-template/setting',
                name: 'setting',
                component: './Configuration/FreightTemplate/Setting',
              },
            ],
          },
          //高级设置
          {
            path: '/configuration/setting',
            name: 'setting',
            component: './Configuration/Setting/List',
          }
        ],
      },

      // 系统
      {
        name: 'system',
        icon: 'deployment-unit',
        path: '/system',
        routes: [
          // 系统信息
          {
            path: 'info',
            name: 'info',
            component: './System/Info',
          },
          {
            path: 'simple-data',
            name: 'simple-data',
            component: './System/SimpleData',
          }
        ],
      },
      
      // 所有页面配置要放到404之前，否则会报404
      {
        component: '404',
      },
    ],
  },
];
