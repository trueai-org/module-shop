var util = require('../../../utils/util.js');
var api = require('../../../config/api.js');
var app = getApp();

Page({
  data: {
    array: ['商品相关', '物流状况', '客户服务', '优惠活动', '功能异常', '产品建议', '其他'],
    textareaAValue: '',
    index: null,
    contact: ''
  },
  PickerChange(e) {
    this.setData({
      index: e.detail.value
    })
  },
  onLoad: function (options) {

  },
  onReady: function () {

  },
  onShow: function () {

  },
  onHide: function () {
    // 页面隐藏

  },
  onUnload: function () {
    // 页面关闭
  },
  textareaAInput(e) {
    this.setData({
      textareaAValue: e.detail.value
    })
  },
  onContact(e) {
    this.setData({
      contact: e.detail.value
    });
  },
  save() {
    if (!this.data.index) {
      wx.showToast({
        title: '请选择反馈类型',
        icon: 'none'
      });
      return;
    }
    wx.showLoading({
      title: '提交中...'
    });
    let that = this;
    util.request(api.Feedbacks, {
      contact: this.data.contact,
      content: this.data.textareaAValue,
      type: this.data.index
    }, 'POST').then(res => {
      wx.hideLoading();
      if (res.success == true) {
        wx.showToast({
          title: '提交成功'
        });
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
})