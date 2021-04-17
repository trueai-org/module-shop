var app = getApp();
var util = require('../../utils/util.js');
var api = require('../../config/api.js');

Page({
  data: {
    showType: 0,

    entityId: 0,
    entityTypeId: 0,
    reviewsInfo: {
      reviewsCount: 0,
      mediasCount: 0,
      ratingAverage: 0,
      positiveRatingPercent: 0,
      rating1Count: 0,
      rating2Count: 0,
      rating3Count: 0,
      rating4Count: 0,
      rating5Count: 0
    },

    defaultAvatar: app.globalData.defaultAvatar,

    pageNum: 1,
    pageSize: 5,
    total: 0,
    predicate: 'supportCount',
    reverse: true,
    pageData: [],
    isMedia: null,
    ratingLevel: null
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    this.setData({
      entityId: options.entityId,
      entityTypeId: options.entityTypeId
    });
    this.getReviewsInfo();
    this.getList();
  },
  getReviewsInfo: function () {
    let that = this;
    util.request(api.ReviewsInfo, {
      entityId: that.data.entityId,
      entityTypeId: that.data.entityTypeId
    }, 'POST').then(function (res) {
      if (res.success === true) {
        that.setData({
          reviewsInfo: res.data,
        });
      }
    });
  },
  ViewImage(e) {
    console.log(e);
    let id = e.currentTarget.dataset.id;
    let index = parseInt(e.currentTarget.dataset.index);
    if (id && index >= 0) {
      let item = this.data.pageData.find(c => c.id == id);
      if (item) {
        wx.previewImage({
          urls: item.mediaUrls,
          current: item.mediaUrls[index]
        });
      }
    }
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
  switchTab: function () {
    let type = this.data.showType == 1 ? 0 : 1;
    this.setData({
      showType: type,
      predicate: type == 1 ? 'id' : 'supportCount'
    });
    this.queckResetQuery();
  },
  redirectReview: function (event) {
    let id = event.currentTarget.dataset.id;
    wx.navigateTo({
      url: '../commentPost/commentPost?reviewId=' + id
    })
  },
  supportReview: function (event) {
    let id = event.currentTarget.dataset.id;
    if (id) {
      let that = this;
      util.request(api.Supports, {
        entityId: id,
        entityTypeId: 10 //Review
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
  supportReply: function (event) {
    let id = event.currentTarget.dataset.id;
    if (id) {
      let that = this;
      util.request(api.Supports, {
        entityId: id,
        entityTypeId: 11 //Reply
      }, 'POST').then(function (res) {
        if (res.success === true) {
          let any = false;
          that.data.pageData.forEach(x => {
            if (any)
              return;
            let first = x.replies.find(c => c.id == id);
            if (first) {
              first.supportCount = parseInt(res.data);
              any = true;
            }
          });
          if (any) {
            that.setData({
              pageData: that.data.pageData
            });
          }
        }
      });
    }
  },
  switchAll: function () {
    this.setData({
      isMedia: null,
      ratingLevel: null
    });
    this.queckResetQuery();
  },
  switchMedia: function () {
    this.setData({
      isMedia: this.data.isMedia === true ? null : true
    });
    this.queckResetQuery();
  },
  switchLevel: function (event) {
    if (event) {
      let level = event.currentTarget.dataset.id;
      this.setData({
        ratingLevel: level == this.data.ratingLevel ? null : level
      });
    } else {
      this.setData({
        ratingLevel: null
      });
    }
    this.queckResetQuery();
  },
  queckResetQuery() {
    this.setData({
      pageData: [],
      pageNum: 1,
      pageSize: this.data.pageSize,
      total: 0,
      predicate: this.data.predicate,
      reverse: true,
      isMedia: this.data.isMedia,
      ratingLevel: this.data.ratingLevel
    }, () => {
      this.getList();
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
        entityId: that.data.entityId,
        entityTypeId: that.data.entityTypeId,
        isMedia: that.data.isMedia,
        ratingLevel: that.data.ratingLevel
      }
    };
    util.request(api.ReviewsGrid, params, "POST")
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
  }
})