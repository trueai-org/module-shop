const app = getApp();
var api = require('../config/api.js')

function formatTime(date) {
  var year = date.getFullYear()
  var month = date.getMonth() + 1
  var day = date.getDate()

  var hour = date.getHours()
  var minute = date.getMinutes()
  var second = date.getSeconds()

  return [year, month, day].map(formatNumber).join('-') + ' ' + [hour, minute, second].map(formatNumber).join(':')
}

function formatNumber(n) {
  n = n.toString()
  return n[1] ? n : '0' + n
}

/**
 * 封封微信的的request
 */
function request(url, data = {}, method = "GET") {
  let that = this;
  return new Promise(function (resolve, reject) {
    wx.request({
      url: url,
      data: data,
      method: method,
      header: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + wx.getStorageSync('token')
      },
      success: function (res) {
        if (res.statusCode == 200) {
          resolve(res.data);
        } else if (res.statusCode == 401) {
          // wx.showToast({
          //   title: '请登录',
          //   icon: 'none'
          // })
          // // 清空本地缓存
          // try {
          //   wx.removeStorageSync('userInfo');
          //   wx.removeStorageSync('token');
          // } catch (e) {
          //   // Do something when catch error
          // }
          // resolve(res.data);

          // 如果没有权限，则重新执行自动登录处理
          // 自动登录代码，已测试通过
          let code = null;
          login().then((res) => {
            code = res;
            return getUserInfo2();
          }).then((e) => {
            // console.log(e);
            // console.log(code);
            if (e && e.userInfo && code) {
              // 如果获取到用户信息，则自动重新登录
              request2(api.LoginByWeixin, {
                code: code,
                nickName: e.userInfo.nickName,
                avatarUrl: e.userInfo.avatarUrl
              }, 'POST').then(res => {
                if (res.success === true) {
                  let userInfo = {
                    name: res.data.name,
                    avatar: res.data.avatar
                  };

                  app.globalData.userInfo = userInfo;
                  app.globalData.token = res.data.token;
                  wx.setStorageSync('userInfo', JSON.stringify(userInfo));
                  wx.setStorageSync('token', res.data.token);

                  // 自动登录成功
                  // 重新发起请求
                  let data2 = request2(url, data, method);
                  resolve(data2);
                } else {
                  wx.showToast({
                    title: '请登录',
                    icon: 'none'
                  })
                  resolve(res.data);
                }
              });
            } else {
              wx.showToast({
                title: '请登录',
                icon: 'none'
              })
              resolve(res.data);
            }
          }).catch((err) => {
            wx.showToast({
              title: '请登录',
              icon: 'none'
            })
            console.log(err)
          })
        } else if (res.statusCode == 403) {
          wx.showToast({
            title: '您没有操作权限',
            icon: 'none'
          })
        } else {
          wx.showToast({
            title: '系统繁忙',
            icon: 'none'
          })
        }
      },
      fail: function (err) {
        wx.showToast({
          title: '网络异常',
          icon: 'none'
        })
        console.log("failed")
        reject(err)
      }
    })
  });
}

/**
 * 封封微信的的request2 用于自动登录成功后重新发起请求
 */
function request2(url, data = {}, method = "GET") {
  return new Promise(function (resolve, reject) {
    wx.request({
      url: url,
      data: data,
      method: method,
      header: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + wx.getStorageSync('token')
      },
      success: function (res) {
        if (res.statusCode == 200) {
          resolve(res.data);
        } else if (res.statusCode == 401) {
          wx.showToast({
            title: '请登录',
            icon: 'none'
          })
          // 清空本地缓存
          try {
            wx.removeStorageSync('userInfo');
            wx.removeStorageSync('token');
          } catch (e) {
            // Do something when catch error
          }
          resolve(res.data);
        } else if (res.statusCode == 403) {
          wx.showToast({
            title: '您没有操作权限',
            icon: 'none'
          })
        } else {
          wx.showToast({
            title: '系统繁忙',
            icon: 'none'
          })
        }
      },
      fail: function (err) {
        reject(err)
        console.log("failed")
      }
    })
  });
}

function get(url, data = {}) {
  return request(url, data, 'GET')
}

function post(url, data = {}) {
  return request(url, data, 'POST')
}

/**
 * 检查微信会话是否过期
 */
function checkSession() {
  return new Promise(function (resolve, reject) {
    wx.checkSession({
      success: function () {
        resolve(true);
      },
      fail: function () {
        reject(false);
      }
    })
  });
}

/**
 * 调用微信登录
 */
function login() {
  return new Promise(function (resolve, reject) {
    wx.login({
      success: function (res) {
        if (res.code) {
          resolve(res.code);
        } else {
          reject(res);
        }
      },
      fail: function (err) {
        reject(err);
      }
    });
  });
}

function getUserInfo() {
  return new Promise(function (resolve, reject) {
    wx.getUserInfo({
      withCredentials: true,
      success: function (res) {
        if (res.detail.errMsg === 'getUserInfo:ok') {
          resolve(res);
        } else {
          reject(res)
        }
      },
      fail: function (err) {
        reject(err);
      }
    })
  });
}

/**
 * 注意这里是二次登录 用于自动登录处理
 */
function getUserInfo2() {
  return new Promise(function (resolve, reject) {
    wx.getUserInfo({
      withCredentials: true,
      success: function (res) {
        if (res.errMsg === 'getUserInfo:ok') {
          resolve(res);
        } else {
          reject(res)
        }
      },
      fail: function (err) {
        reject(err);
      }
    })
  });
}

function redirect(url) {
  //判断页面是否需要登录
  if (false) {
    wx.redirectTo({
      url: '/pages/auth/login/login'
    });
    return false;
  } else {
    wx.redirectTo({
      url: url
    });
  }
}

function showErrorToast(msg) {
  wx.showToast({
    title: msg,
    image: '/static/images/icon_error.png'
  })
}

module.exports = {
  formatTime,
  request,
  get,
  post,
  redirect,
  showErrorToast,
  checkSession,
  login,
  getUserInfo
}