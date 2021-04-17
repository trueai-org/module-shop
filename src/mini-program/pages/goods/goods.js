var app = getApp();
var WxParse = require('../../lib/wxParse/wxParse.js');
var util = require('../../utils/util.js');
var api = require('../../config/api.js');
var user = require('../../services/user.js');

Page({
  data: {
    id: 0,
    goods: {},
    gallery: [],
    attribute: [],
    issueList: [],
    comment: [],
    brand: {},
    specificationList: [],
    productList: [],
    relatedGoods: [],
    cartGoodsCount: 0,
    userHasCollect: 0,
    number: 1,
    checkedSpecText: '请选择规格数量',
    openAttr: false,

    isCollect: false,
    defaultAvatar: app.globalData.defaultAvatar,

    product: {
      categoryIds: [],
      attributes: [],
      options: [],
      variations: [],
      productImages: []
    },
    relateds: [],
    stocks: [],
    currentStock: {},
    isAllSpec: false,
    currentSkuImageUrl: '',

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

    tabCur: 0,
    reviewsList: []
  },
  navToReview: function () {
    wx.navigateTo({
      url: `../comment/comment?entityId=${this.data.id}&entityTypeId=3`
    })
  },
  navToGoods: function (e) {
    wx.navigateTo({
      url: `./goods?id=${e.currentTarget.dataset.id}`
    })
  },
  tabSelect(e) {
    this.setData({
      tabCur: e.currentTarget.dataset.id
    })
  },
  getGoodsInfo: function () {
    wx.showLoading({
      title: '加载中...'
    });
    let that = this;
    util.request(api.Goods + '/' + that.data.id).then(function (res) {
      wx.hideLoading();
      if (res.success === true) {
        that.setData({
          product: res.data,
          currentSkuImageUrl: res.data.mediaUrl
        });
        WxParse.wxParse('specification', 'html', res.data.specification, that);
        WxParse.wxParse('description', 'html', res.data.description, that);
      } else {
        wx.showToast({
          title: res.message,
          icon: 'none'
        });
      }
    });
    that.getCollectStatus();
    that.getReviewsInfo();
    that.getGoodsStocks();
    that.getReviewsList();
    that.getGoodsRelated();
  },
  getReviewsInfo: function () {
    let that = this;
    util.request(api.ReviewsInfo, {
      entityId: that.data.id,
      entityTypeId: 3
    }, 'POST').then(function (res) {
      if (res.success === true) {
        that.setData({
          reviewsInfo: res.data,
        });
      }
    });
  },
  getReviewsList: function () {
    let that = this;
    util.request(api.ReviewsList, {
      entityId: that.data.id,
      entityTypeId: 3
    }, 'POST').then(function (res) {
      if (res.success === true) {
        that.setData({
          reviewsList: res.data,
        });
      }
    });
  },
  getGoodsRelated: function () {
    let that = this;
    util.request(api.GoodsRelated2 + '/' + that.data.id).then(function (res) {
      if (res.success === true) {
        that.setData({
          relateds: res.data,
        });
      }
    });
  },
  getCollectStatus: function () {
    let that = this;
    user.checkLogin().then(c => {
      if (c) {
        util.request(api.WishlistCollectStatusByProduct + '/' + that.data.id).then(function (res) {
          if (res.success === true) {
            if (res.data == true) {
              that.setData({
                isCollect: true
              });
            } else {
              that.setData({
                isCollect: false
              });
            }
          }
        });
      }
    })
  },
  getGoodsStocks: function () {
    let that = this;
    util.request(api.GoodsStocks + '/' + that.data.id).then(function (res) {
      if (res.success === true) {
        that.setData({
          stocks: res.data,
        });
      }
    });
  },
  clickSkuValue: function (event) {
    let that = this;
    let specNameId = event.currentTarget.dataset.nameId;
    let specValueId = event.currentTarget.dataset.valueId;

    //判断是否可以点击

    //TODO 性能优化，可在wx:for中添加index，可以直接获取点击的属性名和属性值，不用循环
    let _specificationList = this.data.product.options;
    for (let i = 0; i < _specificationList.length; i++) {
      if (_specificationList[i].id == specNameId) {
        for (let j = 0; j < _specificationList[i].values.length; j++) {
          if (_specificationList[i].values[j].id == specValueId) {
            //如果已经选中，则反选
            if (_specificationList[i].values[j].checked) {
              _specificationList[i].values[j].checked = false;
            } else {
              _specificationList[i].values[j].checked = true;
            }
          } else {
            _specificationList[i].values[j].checked = false;
          }
        }
      }
    }
    this.setData({
      'product.options': _specificationList
    });

    //重新计算spec改变后的信息
    this.changeSpecInfo();

    //重新计算哪些值不可以点击
  },

  //获取选中的规格信息
  getCheckedSpecValue: function () {
    let mediaUrl = '';
    let checkedValues = [];
    let _specificationList = this.data.product.options;
    for (let i = 0; i < _specificationList.length; i++) {
      let _checkedObj = {
        nameId: _specificationList[i].id,
        valueId: 0,
        valueText: ''
      };
      for (let j = 0; j < _specificationList[i].values.length; j++) {
        if (_specificationList[i].values[j].checked) {
          _checkedObj.valueId = _specificationList[i].values[j].id;
          _checkedObj.valueText = _specificationList[i].values[j].value;

          if (_specificationList[i].values[j].mediaUrl)
            mediaUrl = _specificationList[i].values[j].mediaUrl;
        }
      }
      checkedValues.push(_checkedObj);
    }

    if (!mediaUrl) {
      mediaUrl = this.data.product.mediaUrl;
    }
    this.setData({
      currentSkuImageUrl: mediaUrl
    });
    return checkedValues;
  },
  //根据已选的值，计算其它值的状态
  setSpecValueStatus: function () {

  },
  //判断规格是否选择完整
  isCheckedAllSpec: function () {
    let isAllSpec = !this.getCheckedSpecValue().some(function (v) {
      if (v.valueId == 0) {
        return true;
      }
    });
    this.setData({
      isAllSpec: isAllSpec
    });
    if (this.data.product.options.length <= 0 && this.data.stocks.length > 0) {
      this.setData({
        currentStock: this.data.stocks[0]
      });
    }
    return isAllSpec;
  },
  getCheckedSpecKey: function () {
    let checkedValue = this.getCheckedSpecValue().map(function (v) {
      return v.valueId;
    });

    return checkedValue.join('_');
  },
  changeSpecInfo: function () {

    let checkedNameValue = this.getCheckedSpecValue();

    //设置选择的信息
    let checkedValue = checkedNameValue.filter(function (v) {
      if (v.valueId != 0) {
        return true;
      } else {
        return false;
      }
    }).map(function (v) {
      return v.valueText;
    });
    if (checkedValue.length > 0) {
      this.setData({
        'checkedSpecText': checkedValue.join('　')
      });
    } else {
      this.setData({
        'checkedSpecText': '请选择规格数量'
      });
    }
    let currentStock = this.getValidateStock();
    this.setData({
      currentStock: currentStock
    });
  },
  getValidateStock: function () {
    let that = this;
    if (!this.isCheckedAllSpec()) {
      that.setData({
        currentStock: {}
      });
      return {};
    }
    let stockProduct = {};
    let stocks = that.data.stocks;
    let specValues = that.getCheckedSpecValue();
    stocks.forEach(s => {
      let any = false;
      specValues.forEach((c, i) => {
        if (i <= 0 || (i > 0 && any)) {
          any = s.optionCombinations.some(x => x.optionId == c.nameId && x.value == c.valueText);
        }
      });
      if (any) {
        stockProduct = s;
      }
    });
    // console.log(stockProduct);
    return stockProduct;
  },
  getCheckedProductItem: function (key) {
    return this.data.productList.filter(function (v) {
      if (v.goods_specification_ids == key) {
        return true;
      } else {
        return false;
      }
    });
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    var that = this;
    this.setData({
      id: parseInt(options.id)
    });

    user.checkLogin().then(v => {
      if (v === true) {
        util.request(api.Cart).then(function (res) {
          if (res.success === true) {
            that.setData({
              cartGoodsCount: res.data.subCount
            });
          }
        });
      }
    });

    this.getGoodsInfo();
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
  switchAttrPop: function () {
    if (this.data.openAttr == false) {
      this.setData({
        openAttr: !this.data.openAttr
      });
      this.isCheckedAllSpec();
    }
  },
  closeAttr: function () {
    this.setData({
      openAttr: false,
    });
  },
  addCollect: function () {
    let that = this;
    util.request(api.Wishlist, {
        productId: this.data.id
      }, "POST")
      .then(function (res) {
        if (res.success === true) {
          that.setData({
            isCollect: true
          });
        } else {
          wx.showToast({
            title: res.message,
            mask: true
          });
        }
      });
  },
  removeCollect: function () {
    let that = this;
    util.request(api.Wishlist + '/' + this.data.id, {}, "DELETE")
      .then(function (res) {
        if (res.success === true) {
          that.setData({
            isCollect: false
          });
        } else {
          wx.showToast({
            title: res.message,
            mask: true
          });
        }
      });
  },
  switchCollect: function () {
    if (this.data.isCollect) {
      this.removeCollect();
    } else {
      this.addCollect();
    }
  },
  openCartPage: function () {
    wx.switchTab({
      url: '/pages/cart/cart',
    });
  },
  buy: function (e) {
    console.log(e);

    this.isCheckedAllSpec();

    var that = this;
    if (this.data.openAttr === false) {
      //打开规格选择窗口
      this.setData({
        openAttr: !this.data.openAttr
      });
    } else {
      //提示选择完整规格
      if (!this.isCheckedAllSpec()) {
        wx.showToast({
          image: '/static/images/icon_error.png',
          title: '请选择规格',
          mask: true
        });
        return false;
      }

      let stockProduct = {};
      if (that.data.product.options.length > 0) {
        stockProduct = this.getValidateStock();
      } else if (that.data.stocks.length > 0) {
        stockProduct = that.data.stocks[0];
      }
      that.setData({
        currentStock: stockProduct
      });

      if (stockProduct) {
        if (!stockProduct.isAllowToOrder) {
          wx.showToast({
            image: '/static/images/icon_error.png',
            title: '商品库存不足',
            // mask: true
          });
          return false;
        }
      } else {
        wx.showToast({
          image: '/static/images/icon_error.png',
          title: '商品不允许购买',
          // mask: true
        });
        return false;
      }

      // 验证库存
      if (stockProduct.displayStockQuantity && stockProduct.stockQuantity < that.data.number) {
        wx.showToast({
          image: '/static/images/icon_error.png',
          title: '剩余库存:' + stockProduct.stockQuantity,
          // mask: true
        });
        return false;
      }
      if (e.currentTarget.dataset.type == 1) {
        wx.navigateTo({
          url: `/pages/shopping/checkout/checkout?type=product&productId=${stockProduct.productId}&quantity=${that.data.number}`,
        });
      } else {
        // 添加到购物车
        util.request(api.CartAddItem, {
          productId: stockProduct.productId,
          quantity: that.data.number
        }, "POST").then(function (res) {
          let _res = res;
          if (_res.success === true) {
            wx.showToast({
              title: '添加成功'
            });
            that.setData({
              openAttr: !that.data.openAttr,
              cartGoodsCount: _res.data.subCount
            });
          } else {
            wx.showToast({
              image: '/static/images/icon_error.png',
              title: _res.message,
              // mask: true
            });
          }
        });
      }

      // //根据选中的规格，判断是否有对应的sku信息
      // var specKey = this.getCheckedSpecKey();
      // let checkedProduct = this.getCheckedProductItem(specKey);
      // if (!checkedProduct || checkedProduct.length <= 0) {
      //   //找不到对应的product信息，提示没有库存
      //   wx.showToast({
      //     image: '/static/images/icon_error.png',
      //     title: '库存不足',
      //     mask: true
      //   });
      //   return false;
      // }

      //验证库存
      // if (checkedProduct.goods_number < this.data.number) {
      //   //找不到对应的product信息，提示没有库存
      //   wx.showToast({
      //     image: '/static/images/icon_error.png',
      //     title: '库存不足',
      //     mask: true
      //   });
      //   return false;
      // }

      //添加到购物车
      // util.request(api.CartAdd, { goodsId: this.data.goods.id, number: this.data.number, productId: checkedProduct[0].id }, "POST")
      //   .then(function (res) {
      //     let _res = res;
      //     if (_res.errno == 0) {
      //       wx.showToast({
      //         title: '添加成功'
      //       });
      //       that.setData({
      //         openAttr: !that.data.openAttr,
      //         cartGoodsCount: _res.data.cartTotal.goodsCount
      //       });
      //     } else {
      //       wx.showToast({
      //         image: '/static/images/icon_error.png',
      //         title: _res.errmsg,
      //         mask: true
      //       });
      //     }
      //   });
    }
  },
  cutNumber: function () {
    this.setData({
      number: (this.data.number - 1 > 1) ? this.data.number - 1 : 1
    });
  },
  addNumber: function () {
    this.setData({
      number: this.data.number + 1
    });
    this.isCheckedAllSpec();
  }
})