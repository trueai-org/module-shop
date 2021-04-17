var util = require('../../utils/util.js');
var api = require('../../config/api.js');

Page({
  data: {
    categories: [],
    current: {}
  },
  onLoad: function (options) {
    this.getCatalog();
  },
  onReady: function () {
    // 页面渲染完成
  },
  onShow: function () {
    // 页面显示
    // this.getCatalog();
  },
  onHide: function () {
    // 页面隐藏
  },
  onUnload: function () {
    // 页面关闭
  },
  getCatalog() {
    wx.showLoading({
      title: '加载中...'
    });
    let that = this;
    util.request(api.Catalogs)
      .then(function (res) {
        wx.hideLoading();
        if (res.success === true) {
          that.setData({
            categories: res.data,
            current: (res.data && res.data.length > 0) ? res.data[0] : []
          });
        }
      });
  },
  switchCate(e) {
    let id = e.currentTarget.dataset.id;
    if (this.data.current.id == id) {
      return false;
    }
    let first = this.data.categories.find(c => c.id == id);
    this.setData({
      current: first
    });
  }
})