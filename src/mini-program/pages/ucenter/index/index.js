const util = require('../../../utils/util.js');
const api = require('../../../config/api.js');
const user = require('../../../services/user.js');
const app = getApp();

Page({
  data: {
    userInfo: {
      name: '',
      avatar: app.globalData.defaultAvatar
    },
    shopName: app.globalData.title,
    showLoginDialog: false
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
  },
  onReady: function () {

  },
  onShow: function () {
    let that = this;
    user.checkLogin().then(res => {
      if (res) {
        that.setData({
          userInfo: app.globalData.userInfo,
        });
      }
    });
  },
  onHide: function () {
    // 页面隐藏

  },
  onUnload: function () {
    // 页面关闭
  },

  onUserInfoClick: function () {
    if (wx.getStorageSync('token')) {

    } else {
      this.showLoginDialog();
    }
  },

  showLoginDialog() {
    this.setData({
      showLoginDialog: true
    })
  },

  onCloseLoginDialog() {
    this.setData({
      showLoginDialog: false
    })
  },

  onDialogBody() {
    // 阻止冒泡
  },

  onWechatLogin(e) {
    if (e.detail.errMsg !== 'getUserInfo:ok') {
      if (e.detail.errMsg === 'getUserInfo:fail auth deny') {
        return false
      }
      wx.showToast({
        title: '微信登录失败',
      })
      return false
    }

    wx.showToast({
      title: '正在登录...',
      icon: 'none'
    });
    util.login().then((res) => {
      // return util.request(api.AuthLoginByWeixin, {
      //   code: res,
      //   userInfo: e.detail
      // }, 'POST');

      return util.request(api.LoginByWeixin, {
        code: res,
        nickName: e.detail.userInfo.nickName,
        avatarUrl: e.detail.userInfo.avatarUrl
      }, 'POST');
    }).then((res) => {
      wx.hideLoading();
      // console.log(res)
      if (res.success !== true) {
        wx.showToast({
          title: '微信登录失败',
        })
        return false;
      } else {
        wx.showToast({
          title: '登录成功'
        });
      }
      let userInfo = {
        name: res.data.name,
        avatar: res.data.avatar
      };
      // 设置用户信息
      this.setData({
        userInfo: userInfo,
        showLoginDialog: false
      });
      app.globalData.userInfo = userInfo;
      app.globalData.token = res.data.token;
      wx.setStorageSync('userInfo', JSON.stringify(userInfo));
      wx.setStorageSync('token', res.data.token);
    }).catch((err) => {
      console.log(err)
    })
  },

  onOrderInfoClick: function (event) {
    wx.navigateTo({
      url: '/pages/ucenter/order/order',
    })
  },

  onSectionItemClick: function (event) {

  },

  // TODO 移到个人信息页面
  exitLogin: function () {
    let that = this;
    wx.showModal({
      title: '',
      confirmColor: '#e54d42',
      content: '退出登录？',
      success: function (res) {
        if (res.confirm) {
          that.setData({
            userInfo: {
              name: '',
              avatar: app.globalData.defaultAvatar
            }
          });
          wx.removeStorageSync('token');
          wx.removeStorageSync('userInfo');
          // wx.clearStorageSync();
          // wx.switchTab({
          //   url: '/pages/index/index'
          // });
        }
      }
    })

  }
})