Page({
  data: {
    status: 0,
    orderId: 0
  },
  payOrder() {
    let that = this;
    wx.redirectTo({
      url: '/pages/pay/pay?orderId=' + that.data.orderId
    });
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    this.setData({
      orderId: options.orderId,
      status: options.status
    })
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
  }
})