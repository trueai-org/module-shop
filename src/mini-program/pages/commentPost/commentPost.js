var app = getApp();
var util = require('../../utils/util.js');
var api = require('../../config/api.js');
Page({
  data: {
    content: '',
    InputBottom: 0,
    loadProgress: 0,

    defaultAvatar: app.globalData.defaultAvatar,
    pageNum: 1,
    pageSize: 5,
    total: 0,
    predicate: 'id',
    reverse: true,
    pageData: [],

    reviewId: 0,
    toReplyId: null,
    item: {},
    isAnonymous: true,

    defaultPlaceholder: '说点什么呗~',
    placeholder: '说点什么呗~',
  },
  InputFocus(e) {
    this.setData({
      InputBottom: e.detail.height
    })
  },
  InputBlur(e) {
    this.setData({
      InputBottom: 0
    })
  },
  onLoad: function (options) {
    let that = this;
    that.setData({
      reviewId: parseInt(options.reviewId)
    });
    this.getInfo();
    this.getList();
  },
  onClose() {
    wx.navigateBack({
      delta: 1
    });
  },
  onPost() {
    let that = this;
    if (!this.data.content) {
      util.showErrorToast('请填写评论')
      return false;
    }
    if (this.data.content.length < 2) {
      util.showErrorToast('评论太短~')
      return false;
    }
    this.setData({
      loadProgress: true
    })
    // wx.showLoading({
    //   title: '发送中',
    // })
    util.request(api.Replies, {
      reviewId: that.data.reviewId,
      toReplyId: that.data.toReplyId,
      comment: that.data.content,
      isAnonymous: that.data.isAnonymous
    }, 'POST').then(function (res) {
      that.setData({
        loadProgress: false
      });
      if (res.success === true) {
        wx.showToast({
          title: '评论成功',
          complete: function () {
            that.setData({
              content: ''
            });
            // that.queckResetQuery();
            // wx.navigateBack({
            //   delta: 1
            // });
          }
        })
      } else {
        wx.showToast({
          title: res.message
        })
      }
    });
  },
  bindInpuntValue(event) {
    let value = event.detail.value;
    //判断是否超过140个字符
    if (value && value.length > 400) {
      return false;
    }
    this.setData({
      content: event.detail.value,
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

  },
  getInfo: function () {
    let that = this;
    util.request(api.Reviews + '/' + that.data.reviewId).then(function (res) {
      if (res.success === true) {
        that.setData({
          item: res.data,
        });
      }
    });
  },
  supportReview: function () {
    let id = this.data.reviewId;
    if (id) {
      let that = this;
      util.request(api.Supports, {
        entityId: id,
        entityTypeId: 10 //Review
      }, 'POST').then(function (res) {
        if (res.success === true) {
          that.getInfo();
        }
      });
    }
  },
  supportReply: function (event) {
    let id = event.currentTarget.dataset.id;
    if (id) {
      let that = this;
      util.request(api.Supports, {
        entityId: id,
        entityTypeId: 11 //Reply
      }, 'POST').then(function (res) {
        if (res.success === true) {
          let first = that.data.pageData.find(c => c.id == id);
          if (first) {
            first.supportCount = parseInt(res.data);
            that.setData({
              pageData: that.data.pageData
            });
          }
        }
      });
    }
  },
  replyReplier: function (event) {
    let id = event.currentTarget.dataset.id;
    let name = event.currentTarget.dataset.name;
    this.setData({
      placeholder: '回复 ' + name + ':',
      toReplyId: id
    });
  },
  replyReset: function () {
    this.setData({
      placeholder: this.data.defaultPlaceholder,
      toReplyId: null
    });
  },
  onReachBottom: function () {
    if (this.data.total > 0 && this.data.pageNum * this.data.pageSize < this.data.total) {
      this.setData({
        pageNum: this.data.pageNum + 1
      }, () => {
        this.getList();
      });
    }
  },
  getList: function () {
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
        reviewId: that.data.reviewId
      }
    };
    util.request(api.RepliesGrid, params, "POST")
      .then(function (res) {
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
  switchAnonymous: function () {
    this.setData({
      isAnonymous: !this.data.isAnonymous
    });
  },
  queckResetQuery() {
    this.setData({
      pageData: [],
      pageNum: 1,
      pageSize: this.data.pageSize,
      total: 0,
      predicate: this.data.predicate,
      reverse: true
    }, () => {
      this.getList();
    });
  }
})