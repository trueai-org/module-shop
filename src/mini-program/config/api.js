// const ApiHost = 'https://localhost:44372/api/';
const ApiHost = 'https://shopapi.circle.ac.cn/api/';

module.exports = {
  Host: ApiHost,

  // 首页
  Widgets: ApiHost + 'widgets', // 首页小部件

  // 分类
  Catalogs: ApiHost + 'catalogs', // 分类
  SubCategories: ApiHost + 'catalogs/sub-categories', // 仅二级分类

  // 商品
  Goods: ApiHost + 'goods', // 商品列表
  GoodsRelated2: ApiHost + 'goods/related', // 推荐商品列表
  GoodsGrid: ApiHost + 'goods/grid', // 商品列表
  GoodsStocks: ApiHost + 'goods/stocks', // 商品库存

  // 商品搜索
  Keywords: ApiHost + 'goods/keywords', // 搜索关键词列表
  ClearKeywords: ApiHost + 'goods/keywords/clear-histories', // 清除历史搜索记录

  // 购物车
  Cart: ApiHost + 'cart', // 购物车列表
  CartAddItem: ApiHost + 'cart/add-item',
  CartUpdateItem: ApiHost + 'cart/update-item-quantity',
  CartCheckedItem: ApiHost + 'cart/checked',
  CartRemoveItem: ApiHost + 'cart/remove-items',

  // 购物车 下单
  CheckoutCart: ApiHost + 'checkout/cart', // 下单前确认信息
  CheckoutProduct: ApiHost + 'checkout/product', // 下单前确认信息
  CheckoutOrder: ApiHost + 'checkout/order', // 下单前确认信息

  // 登录
  LoginByWeixin: ApiHost + 'mp/login', // 微信登录

  // 用户地址
  Addresses: ApiHost + 'user-addresses', // 用户地址
  Provinces: ApiHost + 'user-addresses/provinces', // 省市区

  // 客户订单
  Orders: ApiHost + 'customer-orders',
  OrderGrid: ApiHost + 'customer-orders/grid', // 订单列表
  OrderCancel2: ApiHost + 'customer-orders/grid', // 订单取消

  // 最近浏览
  RecentlyViewed: ApiHost + 'recently-viewed', // 最近浏览

  // 我的收藏
  Wishlist: ApiHost + 'wishlist', // 我的收藏
  WishlistCollectStatusByProduct: ApiHost + 'wishlist/collect-status', // {productId:int:min(1)} 商品收藏状态

  // 评论
  Reviews: ApiHost + 'reviews',
  ReviewsInfo: ApiHost + 'reviews/info',
  ReviewsList: ApiHost + 'reviews/list',
  ReviewsGrid: ApiHost + 'reviews/grid',

  // 评论回复 
  Replies: ApiHost + 'replies',
  RepliesGrid: ApiHost + 'replies/grid',

  // 推荐/赞
  Supports: ApiHost + 'supports',

  // 留言
  Feedbacks: ApiHost + 'feedbacks'
};