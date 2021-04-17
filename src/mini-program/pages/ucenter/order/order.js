var util = require('../../../utils/util.js');
var api = require('../../../config/api.js');

Page({
  data: {
    pageNum: 1,
    pageSize: 10,
    total: 0,
    predicate: 'id',
    reverse: true,
    name: '',
    categoryId: '',
    pageData: [],

    showType: 0,
    orderStatus: [],
    show: false,

    visibleDelete: false,
    visibleCancel: false,
    currentDeleteId: 0,
    currentCancelId: 0
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    // console.log(options);
    if (options.type && parseInt(options.type) >= 0) {
      this.switchTabById(parseInt(options.type));
    } else {
      this.getGrid();
    }
  },
  payOrder(e) {
    wx.navigateTo({
      url: `/pages/pay/pay?orderId=${e.currentTarget.dataset.id}&orderTotal=${e.currentTarget.dataset.amount}`
    });
  },
  onReady: function () {
    // 页面渲染完成
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
  onReachBottom: function () {
    // 当界面的下方距离页面底部距离小于100像素时触发回调
    if (this.data.total > 0 && this.data.pageNum * this.data.pageSize < this.data.total) {
      this.setData({
        pageNum: this.data.pageNum + 1
      }, () => {
        this.getGrid();
      });
    }
  },
  getGrid: function () {
    // wx.showLoading({
    //   title: '加载中...'
    // });
    this.setData({
      show: true
    });
    let that = this;
    var params = {
      pagination: {
        current: that.data.pageNum,
        pageSize: that.data.pageSize
      },
      sort: {
        predicate: that.data.predicate,
        reverse: that.data.reverse,
      },
      search: {
        orderStatus: that.data.orderStatus
      }
    };
    util.request(api.OrderGrid, params, "POST")
      .then(function (res) {
        // wx.hideLoading();
        that.setData({
          show: false
        });
        if (res.success === true) {
          let origin_data = that.data.pageData || [];
          let new_data = origin_data.concat(res.data.list)
          that.setData({
            pageData: new_data,
            total: parseInt(res.data.pagination.total)
          });
        }
      });
  },
  switchTab: function (e) {
    let id = e.currentTarget.dataset.id;
    this.switchTabById(id);
  },
  switchTabById: function (id) {
    // let id = e.currentTarget.dataset.id;
    let os = [];
    switch (id) {
      case 1:
        os = [20, 25];
        break;
      case 2:
        os = [30, 40, 50];
        break;
      case 3:
        os = [60];
        break;
      case 4:
        os = [70];
        break;
      default:
        break;
    }
    this.setData({
      showType: id,
      orderStatus: os
    });
    this.quickResetQuery();
  },
  quickResetQuery() {
    this.setData({
      pageData: [],
      pageNum: 1,
      pageSize: this.data.pageSize,
      total: 0,
      predicate: this.data.predicate,
      reverse: true,
      orderStatus: this.data.orderStatus
    }, () => {
      this.getGrid();
    });
  },
  showDeleteModal(e) {
    this.setData({
      visibleDelete: true,
      currentDeleteId: e.currentTarget.dataset.id
    })
  },
  hideModal() {
    this.setData({
      visibleDelete: false
    })
  },
  deleteOrder() {
    wx.showLoading({
      title: '删除中...'
    });
    let that = this;
    util.request(api.Orders + '/' + this.data.currentDeleteId, {}, 'DELETE').then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        // wx.navigateBack({
        //   delta: 1
        // });
        // wx.redirectTo({
        //   url: '../order/order',
        // });
        that.hideModal();
        that.quickResetQuery();
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
      visibleCancel: true,
      currentCancelId: e.currentTarget.dataset.id
    })
  },
  hideCancelModal() {
    this.setData({
      visibleCancel: false
    })
  },
  cancelOrder() {
    wx.showLoading({
      title: '取消中...'
    });
    let that = this;
    util.request(api.Orders + '/' + this.data.currentCancelId + '/cancel', {}, 'PUT').then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        // wx.navigateBack({
        //   delta: 1
        // });
        // wx.redirectTo({
        //   url: '../order/order',
        // });
        that.hideCancelModal();
        that.quickResetQuery();
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
  againOrder(e) {
    wx.navigateTo({
      url: '/pages/shopping/checkout/checkout?type=order&orderId=' + e.currentTarget.dataset.id,
    });
  },
})