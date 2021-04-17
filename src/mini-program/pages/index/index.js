const util = require('../../utils/util.js');
const api = require('../../config/api.js');
var HtmlToJson = require('../../lib/wxParse/html2json.js');
const app = getApp()

Page({
  data: {
    newGoods: [],
    hotGoods: [],
    topics: [],
    brands: [],
    floorGoods: [],
    banner: [],
    channel: [],

    // todo
    widgets: [],
    carouselWidgets: [],
    categorys: [],
    htmls: [],
    simplProducts: [],
    products: []
  },
  onShareAppMessage: function () {
    return {
      title: app.globalData.title,
      desc: app.globalData.desc,
      path: '/pages/index/index'
    }
  },
  navToGoods: function (e) {
    wx.navigateTo({
      url: `../goods/goods?id=${e.currentTarget.dataset.id}`
    })
  },
  getIndexData: function () {
    let that = this;
    wx.showLoading({
      title: '加载中...',
    });
    util.request(api.Widgets).then(function (res) {
      // wx.hideLoading();
      if (res.success === true) {
        that.setData({
          widgets: res.data.widgetInstances || []
        }, () => {
          that.data.widgets.forEach(e => {
            if (e.widgetId == 4) {
              let olds = that.data.htmls;
              let transData = HtmlToJson.html2json(e.htmlData);
              olds.push({
                id: e.id,
                widgetId: e.widgetId,
                widgetZoneId: e.widgetZoneId,
                htmlData: transData.nodes
              });
              that.setData({
                htmls: olds
              });
              return;
            }
            wx.showLoading({
              title: '加载中...',
            });
            util.request(api.Widgets + '/' + e.id).then(function (itemRes) {
              wx.hideLoading();
              if (itemRes.success === true) {
                if (e.widgetId == 5 &&
                  itemRes.data &&
                  itemRes.data.items &&
                  itemRes.data.items.length > 0) {
                  let list = that.data.carouselWidgets;
                  list.push(itemRes.data);
                  that.setData({
                    carouselWidgets: list
                  });
                } else if (e.widgetId == 1) {
                  let list = that.data.categorys;
                  list.push(itemRes.data);
                  that.setData({
                    categorys: list
                  });
                } else if (e.widgetId == 3) {
                  let list = that.data.simplProducts;
                  list.push(itemRes.data);
                  that.setData({
                    simplProducts: list
                  });
                } else if (e.widgetId == 2) {
                  let list = that.data.products;
                  list.push(itemRes.data);
                  that.setData({
                    products: list
                  });
                }
              }
            });
          });
        });
      } else {
        // wx.hideLoading();
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
  onLoad: function (options) {
    this.getIndexData();
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
})