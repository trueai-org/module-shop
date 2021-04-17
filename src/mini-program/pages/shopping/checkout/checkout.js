var util = require('../../../utils/util.js');
var api = require('../../../config/api.js');
const pay = require('../../../services/pay.js');

var app = getApp();

Page({
  data: {
    checkedGoodsList: [],
    checkedAddress: {},
    checkedCoupon: [],
    couponList: [],
    goodsTotalPrice: 0.00, //商品总价
    freightPrice: 0.00, //快递费
    couponPrice: 0.00, //优惠券的价格
    orderTotalPrice: 0.00, //订单总价
    actualPrice: 0.00, //实际需要支付的总价
    addressId: 0,
    couponId: 0,

    address: {},
    items: [],
    info: {},

    type: 'cart', // 购买方式 cart:通过购物车跳转购买(type=cart), product:单品购买(type=product&productId=1&quantity=2), order:通过订单再次购买(type=order&orderId=1)
    productId: 0,
    quantity: 0,
    orderId: 0
  },
  onLoad: function (options) {
    console.log(options);

    // 页面初始化 options为页面跳转所带来的参数
    try {
      this.setData({
        type: options.type || 'cart',
        productId: options.productId || 0,
        quantity: options.quantity || 0,
        orderId: options.orderId || 0
      });

      var addressId = wx.getStorageSync('addressId');
      if (addressId) {
        this.setData({
          'addressId': addressId
        });
      }
      var couponId = wx.getStorageSync('couponId');
      if (couponId) {
        this.setData({
          'couponId': couponId
        });
      }
    } catch (e) {
      // Do something when catch error
    }
  },
  getCheckoutInfo: function () {
    let that = this;
    wx.showLoading({
      title: '加载中...',
    })
    let type = that.data.type;
    let url = api.CheckoutCart;
    if (type == 'product') {
      url = api.CheckoutProduct;
    } else if (type == 'order') {
      url = api.CheckoutOrder;
    }
    let param = {
      userAddressId: that.data.addressId,
      productId: that.data.productId,
      quantity: that.data.quantity,
      orderId: that.data.orderId
    };
    util.request(url, param).then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        that.setData({
          checkedGoodsList: res.data.items,
          checkedAddress: res.data.address,
          actualPrice: 0,
          checkedCoupon: 1,
          couponList: [],
          couponPrice: 0,
          freightPrice: 0,
          goodsTotalPrice: 0,
          orderTotalPrice: 0
        });
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
  selectAddress() {
    wx.navigateTo({
      url: '/pages/shopping/address/address',
    })
  },
  addAddress() {
    wx.navigateTo({
      url: '/pages/shopping/addressAdd/addressAdd',
    })
  },
  onReady: function () {
    // 页面渲染完成

  },
  onShow: function () {
    // 页面显示
    this.getCheckoutInfo();
  },
  onHide: function () {
    // 页面隐藏

  },
  onUnload: function () {
    // 页面关闭

  },
  submitOrder: function () {
    if (this.data.addressId <= 0) {
      util.showErrorToast('请选择收货地址');
      return false;
    }
    wx.showLoading({
      title: '提交订单中...'
    });
    let that = this;
    let type = that.data.type;
    let url = api.CheckoutCart;
    if (type == 'product') {
      url = api.CheckoutProduct;
    } else if (type == 'order') {
      url = api.CheckoutOrder;
    }
    let param = {
      shippingUserAddressId: that.data.addressId,
      productId: that.data.productId,
      quantity: that.data.quantity,
      orderId: that.data.orderId,
      orderNote: '',
      couponId: that.data.couponId
    };

    util.request(url, param, 'POST').then(res => {
      wx.hideLoading();
      if (res.success === true && res.data && res.data.orderId) {
        wx.redirectTo({
          url: `/pages/pay/pay?orderId=${res.data.orderId}&orderTotal=${res.data.orderTotal}`
        });
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  }
})