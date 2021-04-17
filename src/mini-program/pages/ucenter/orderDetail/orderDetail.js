var util = require('../../../utils/util.js');
var api = require('../../../config/api.js');

Page({
  data: {
    order: {
      address: {}
    },
    showTimer: false,
    timeStrDesc: '',
    timerStr: `${0}天 ${0}小时 ${0}分钟 ${0}秒`,
    timerTrue: false,
    total: 0,

    visibleDelete: false,
    visibleCancel: false,
    visibleShipped: false,
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    this.setData({
      orderId: options.id
    });
    // this.getOrderDetail();
  },
  getOrderDetail() {
    let that = this;
    wx.showLoading({
      title: '加载中...'
    });
    util.request(api.Orders + '/' + that.data.orderId).then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        that.setData({
          order: res.data
        });
        that.payTimer();
      }
    });
  },
  payTimer() {
    let that = this;
    if (!this.data.order)
      return;
    let total = 0
    let desc = '';
    if (parseInt(this.data.order.paymentEndOnForSecond) > 0) {
      desc = '剩余支付时间：';
      total = parseInt(this.data.order.paymentEndOnForSecond);
    } else if (parseInt(this.data.order.deliveredEndOnForSecond) > 0) {
      desc = '确认收货时间：';
      total = parseInt(this.data.order.deliveredEndOnForSecond);
    }
    that.setData({
      total: total
    })
    if (total > 0) {
      if (that.data.timerTrue)
        return;
      that.setData({
        showTimer: true,
        timeStrDesc: desc,
        timerTrue: true
      })
      setInterval(() => {
        total = parseInt(that.data.total);
        if (total > 0) {
          total -= 1;
          let day = parseInt(total / (3600 * 24));
          let hour = parseInt(total % (3600 * 24) / 3600);
          let min = parseInt((total % 3600) / 60);
          let sec = parseInt((total % 60));
          let str = `${day}天 ${hour}小时 ${min}分钟 ${sec}秒`;
          that.setData({
            timerStr: str,
            total: total
          });
        } else {
          that.setData({
            showTimer: false
          })
        }
      }, 1000);
    }
  },
  payOrder() {
    let that = this;
    wx.navigateTo({
      url: `/pages/pay/pay?orderId=${that.data.order.id}&orderTotal=${that.data.order.orderTotal}`
    });
  },
  onReady: function () {
    // 页面渲染完成
  },
  onShow: function () {
    // 页面显示
    this.getOrderDetail();
  },
  onHide: function () {
    // 页面隐藏
  },
  onUnload: function () {
    // 页面关闭
  },
  showDeleteModal(e) {
    this.setData({
      visibleDelete: true
    })
  },
  hideModal(e) {
    this.setData({
      visibleDelete: false
    })
  },
  deleteOrder() {
    wx.showLoading({
      title: '删除中...'
    });
    let that = this;
    util.request(api.Orders + '/' + that.data.orderId, {}, 'DELETE').then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        // wx.navigateBack({
        //   delta: 1
        // });
        wx.redirectTo({
          url: '../order/order',
        });
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
  showCancelModal(e) {
    this.setData({
      visibleCancel: true
    })
  },
  hideCancelModal(e) {
    this.setData({
      visibleCancel: false
    })
  },
  showShippedModal(e) {
    this.setData({
      visibleShipped: true
    })
  },
  hideShippedModal(e) {
    this.setData({
      visibleShipped: false
    })
  },
  cancelOrder() {
    wx.showLoading({
      title: '取消中...'
    });
    let that = this;
    util.request(api.Orders + '/' + that.data.orderId + '/cancel', {}, 'PUT').then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        // wx.navigateBack({
        //   delta: 1
        // });
        wx.redirectTo({
          url: '../order/order',
        });
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
  shippedOrder() {
    wx.showLoading({
      title: '确认中...'
    });
    let that = this;
    util.request(api.Orders + '/' + that.data.orderId + '/cinfirm-receipt', {}, 'PUT').then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        wx.redirectTo({
          url: '../order/order?type=3',
        });
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
  delayShippedOrder() {
    wx.showLoading({
      title: '延长收货中...'
    });
    let that = this;
    util.request(api.Orders + '/' + that.data.orderId + '/delay-cinfirm-receipt', {}, 'PUT')
      .then(function (res) {
        wx.hideLoading();
        if (res.success === true) {
          wx.showToast({
            title: '延长收货成功'
          });
          that.getOrderDetail();
        } else {
          wx.showToast({
            title: res.message,
            icon: 'none'
          });
        }
      });
  },
  redirectToReview(e) {
    wx.navigateTo({
      url: `/pages/review/review?entityId=${e.target.dataset.id}&entityTypeId=3&sourceId=${this.data.orderId}&sourceType=0`
    });
  },
  againOrder(e) {
    wx.navigateTo({
      url: '/pages/shopping/checkout/checkout?type=order&orderId=' + this.data.orderId,
    });
  },
})