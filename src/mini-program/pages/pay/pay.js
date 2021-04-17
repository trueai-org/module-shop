var app = getApp();
var util = require('../../utils/util.js');
var api = require('../../config/api.js');

Page({
  data: {
    orderId: 0,
    orderTotal: 0.00,
    timeStamp: '',
    nonceStr: '',
    package: '',
    signType: '',
    paySign: '',
  },
  //向服务请求支付参数
  pay() {
    let that = this;
    wx.requestPayment({
      'timeStamp': that.data.timeStamp,
      'nonceStr': that.data.nonceStr,
      'package': that.data.package,
      'signType': that.data.signType,
      'paySign': that.data.paySign,
      'success': function (res) {
        // console.log(res);
        wx.redirectTo({
          url: '/pages/payResult/payResult?status=1&orderId=' + that.data.orderId,
        })
      },
      'fail': function (res) {
        // console.log(res);
        wx.redirectTo({
          url: '/pages/payResult/payResult?status=0&orderId=' + that.data.orderId,
        })
      },
      'complete': function (res) {
        console.log(res);
      }
    })
  },
  startPay() {
    this.pay();
  },
  payInfo() {
    let that = this;
    wx.showLoading({
      title: '加载中...'
    });
    util.request(api.Orders + '/' + this.data.orderId + '/pay')
      .then(res => {
        wx.hideLoading();
        if (res.success === true) {
          that.setData({
            timeStamp: res.data.timeStamp,
            nonceStr: res.data.nonceStr,
            package: res.data.package,
            signType: res.data.signType,
            paySign: res.data.paySign
          });
          that.pay();
        } else {
          if (res.message == 'OK') {
            res.message = "订单已支付";
          }
          wx.showToast({
            title: res.message,
            icon: 'none'
          });
        }
      });
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    this.setData({
      orderId: options.orderId,
      orderTotal: options.orderTotal
    })
    this.payInfo();
  },
  onReady: function () {

  },
  onShow: function () {
    // 页面显示

  },
  onHide: function () {
    // 页面隐藏

  },
  onUnload: function () {
    // 页面关闭

  },
})