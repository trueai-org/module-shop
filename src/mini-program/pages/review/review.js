var app = getApp();
var util = require('../../utils/util.js');
var api = require('../../config/api.js');

Page({

  /**
   * 页面的初始数据
   */
  data: {
    entityId: 0,
    entityTypeId: 0,
    sourceId: 0,
    sourceType: 0,

    maxFavors: 5,
    favors: 5,

    maxImg: 9,
    imgList: [],
    mediaIds: [],
    textareaAValue: '',
    isAnonymous: true
  },
  checkFavor(e) {
    let num = parseInt(e.currentTarget.dataset.id);
    if (num >= 0) {
      this.setData({
        favors: num + 1
      });
    }
  },
  ChooseImage() {
    wx.chooseImage({
      count: this.data.maxImg, // 默认9
      sizeType: ['compressed'], //['original', 'compressed'] // 可以指定是原图还是压缩图，默认二者都有
      sourceType: ['album'], //从相册选择
      success: (res) => {
        let that = this;
        if (that.data.imgList.length >= that.data.maxImg) {
          return;
        }
        res.tempFilePaths.forEach((item, index) => {
          // console.log(item);
          // return;
          wx.showLoading({
            title: '上传中...'
          });
          wx.uploadFile({
            url: api.Host + 'upload',
            filePath: item,
            name: 'file',
            header: {
              'Content-Type': 'application/x-www-form-urlencoded',
              'Authorization': 'Bearer ' + wx.getStorageSync('token')
            },
            success(res) {
              let data = JSON.parse(res.data);
              if (data.success === true) {
                if (that.data.imgList.length != 0) {
                  if (!that.data.mediaIds.some(c => c == data.data.id)) {
                    that.setData({
                      imgList: that.data.imgList.concat([item]),
                      mediaIds: that.data.mediaIds.concat([data.data.id])
                    })
                  }
                } else {
                  that.setData({
                    imgList: [item],
                    mediaIds: [data.data.id]
                  })
                }
                wx.showToast({
                  title: '上传成功'
                })
              } else {
                wx.showToast({
                  title: data.message,
                  icon: 'none'
                });
              }
            },
            fail: () => {
              console.log('fail')
            },
            complete: () => {
              // if (index + 1 == that.data.imgList.length) {
              wx.hideLoading();
              // }
              console.log('complete');
            }
          })
        });
        // const uploadTask =
        // uploadTask.onProgressUpdate((res) => {
        //   console.log('上传进度', res.progress)
        //   console.log('已经上传的数据长度', res.totalBytesSent)
        //   console.log('预期需要上传的数据总长度', res.totalBytesExpectedToSend)
        // })
        // uploadTask.abort() // 取消上传任务
      }
    });
  },
  ViewImage(e) {
    wx.previewImage({
      urls: this.data.imgList,
      current: e.currentTarget.dataset.url
    });
  },
  DelImg(e) {
    this.data.imgList.splice(e.currentTarget.dataset.index, 1);
    this.data.mediaIds.splice(e.currentTarget.dataset.index, 1);
    this.setData({
      imgList: this.data.imgList,
      mediaIds: this.data.mediaIds
    })
    // wx.showModal({
    //   title: '召唤师',
    //   content: '确定要删除这段回忆吗？',
    //   cancelText: '再看看',
    //   confirmText: '再见',
    //   success: res => {
    //     if (res.confirm) {
    //       this.data.imgList.splice(e.currentTarget.dataset.index, 1);
    //       this.setData({
    //         imgList: this.data.imgList
    //       })
    //     }
    //   }
    // })
  },
  textareaAInput(e) {
    this.setData({
      textareaAValue: e.detail.value
    })
  },
  save() {
    wx.showLoading({
      title: '保存中...'
    });
    let that = this;
    util.request(api.Reviews, {
      entityId: that.data.entityId,
      entityTypeId: that.data.entityTypeId,
      sourceId: that.data.sourceId,
      sourceType: that.data.sourceType,
      rating: that.data.favors,
      comment: that.data.textareaAValue,
      isAnonymous: that.data.isAnonymous,
      mediaIds: that.data.mediaIds
    }, 'POST').then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        wx.showToast({
          title: '评论成功'
        });
        wx.navigateBack({
          delta: 1
        });
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
  },
  checkboxChange(e) {
    this.setData({
      isAnonymous: (e.detail.value && e.detail.value.length > 0)
    });
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    this.setData({
      entityId: options.entityId,
      entityTypeId: options.entityTypeId,
      sourceId: options.sourceId,
      sourceType: options.sourceType,
    });
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  }
})