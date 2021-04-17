var util = require('../../utils/util.js');
var api = require('../../config/api.js');

var app = getApp();

Page({
  data: {
    cartGoods: [],
    cartTotal: {
      "goodsCount": 0,
      "goodsAmount": 0.00,
      "checkedGoodsCount": 0,
      "checkedGoodsAmount": 0.00
    },
    isEditCart: false,
    checkedAllStatus: true,
    editCartList: [],

    cart: {
      items: []
    }
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数


  },
  onReady: function () {
    // 页面渲染完成

  },
  onShow: function () {
    // if (typeof this.getTabBar === 'function' && this.getTabBar()) {
    //   this.getTabBar().setData({
    //     selected: 2
    //   })
    // }

    // 页面显示
    this.getCartList();
  },
  onHide: function () {
    // 页面隐藏

  },
  onUnload: function () {
    // 页面关闭

  },
  getCartList: function () {
    let that = this;
    // util.request(api.CartList).then(function (res) {
    //   if (res.errno === 0) {
    //     console.log(res.data);
    //     that.setData({
    //       cartGoods: res.data.cartList,
    //       cartTotal: res.data.cartTotal
    //     });
    //   }
    //   that.setData({
    //     checkedAllStatus: that.isCheckedAll()
    //   });
    // });

    util.request(api.Cart).then(function (res) {
      if (res.success === true) {
        that.setData({
          cart: res.data
        });
        that.setData({
          checkedAllStatus: that.isCheckedAll()
        });
      }
    });
  },
  isCheckedAll: function () {
    //判断购物车商品已全选
    return this.data.cart.items.every(function (element, index, array) {
      if (element.isChecked == true) {
        return true;
      } else {
        return false;
      }
    });
  },
  checkedItem: function (event) {
    let itemIndex = event.target.dataset.itemIndex;
    let that = this;

    if (!this.data.isEditCart) {
      util.request(api.CartCheckedItem,
        { productIds: [that.data.cart.items[itemIndex].productId], isChecked: that.data.cart.items[itemIndex].isChecked ? false : true }, 'PUT').then(function (res) {
          if (res.success === true) {
            that.setData({
              cart: res.data
            });
          }
          that.setData({
            checkedAllStatus: that.isCheckedAll()
          });
        });
    } else {
      //编辑状态
      let cart = that.data.cart;
      cart.items = cart.items.map(function (element, index, array) {
        if (index == itemIndex) {
          element.isChecked = !element.isChecked;
        }
        return element;
      });
      cart.checkedSubCount = that.getCheckedGoodsCount();
      that.setData({
        cart: cart,
        checkedAllStatus: that.isCheckedAll(),
      });
    }
  },
  getCheckedGoodsCount: function () {
    let checkedGoodsCount = 0;
    this.data.cart.items.forEach(function (v) {
      if (v.isChecked === true) {
        checkedGoodsCount += v.quantity;
      }
    });
    return checkedGoodsCount;
  },
  checkedAll: function () {
    let that = this;

    if (!this.data.isEditCart) {
      var productIds = this.data.cart.items.map(function (v) {
        return v.productId;
      });
      util.request(api.CartCheckedItem,
        { productIds: productIds, isChecked: that.isCheckedAll() ? false : true }, 'PUT').then(function (res) {
          if (res.success === true) {
            that.setData({
              cart: res.data
            });
          }
          that.setData({
            checkedAllStatus: that.isCheckedAll()
          });
        });
    } else {
      //编辑状态
      let checkedAllStatus = that.isCheckedAll();
      let cart = that.data.cart;
      cart.items = cart.items.map(function (v) {
        v.isChecked = !checkedAllStatus;
        return v;
      });
      cart.checkedSubCount = that.getCheckedGoodsCount();
      that.setData({
        cart: cart,
        checkedAllStatus: that.isCheckedAll(),
      });
    }

  },
  editCart: function () {
    var that = this;
    if (this.data.isEditCart) {
      this.getCartList();
      this.setData({
        isEditCart: !this.data.isEditCart
      });
    } else {
      // 编辑状态
      let cart = this.data.cart;
      cart.items = cart.items.map(function (v) {
        v.isChecked = false;
        return v;
      });
      cart.checkedSubCount = that.getCheckedGoodsCount();
      this.setData({
        cart: cart,
        isEditCart: !this.data.isEditCart,
        checkedAllStatus: that.isCheckedAll(),
      });
    }
  },
  updateCart: function (productId, quantity) {
    let that = this;
    util.request(api.CartUpdateItem,
      { productId: productId, quantity: quantity }, 'PUT').then(function (res) {
        if (res.success === true) {
          // that.setData({
          //   cart: res.data
          // });
        }
        that.setData({
          checkedAllStatus: that.isCheckedAll()
        });
      });
  },
  cutNumber: function (event) {
    let itemIndex = event.target.dataset.itemIndex;
    let cart = this.data.cart;
    let cartItem = cart.items[itemIndex];
    cartItem.quantity = cartItem.quantity <= 1 ? 1 : cartItem.quantity - 1;
    cart.checkedSubCount = this.getCheckedGoodsCount();
    this.setData({
      cart: cart
    });
    this.updateCart(cartItem.productId, cartItem.quantity);
  },
  addNumber: function (event) {
    let itemIndex = event.target.dataset.itemIndex;
    let cart = this.data.cart;
    let cartItem = cart.items[itemIndex];
    cartItem.quantity = cartItem.quantity >= 999 ? cartItem.quantity : cartItem.quantity + 1;
    cart.checkedSubCount = this.getCheckedGoodsCount();
    this.setData({
      cart: cart
    });
    this.updateCart(cartItem.productId, cartItem.quantity);
  },
  checkoutOrder: function () {
    //获取已选择的商品
    let that = this;
    var checkedGoods = this.data.cart.items.filter(function (element, index, array) {
      if (element.isChecked == true) {
        return true;
      } else {
        return false;
      }
    });
    if (checkedGoods.length <= 0) {
      return false;
    }
    wx.navigateTo({
      url: '../shopping/checkout/checkout'
    })
  },
  deleteCart: function () {
    //获取已选择的商品
    let that = this;
    let products = this.data.cart.items.filter(function (element, index, array) {
      if (element.isChecked == true) {
        return true;
      } else {
        return false;
      }
    });

    if (products.length <= 0) {
      return false;
    }

    let productIds = products.map(function (element, index, array) {
      if (element.isChecked == true) {
        return element.productId;
      }
    });

    util.request(api.CartRemoveItem,
      { productIds: productIds }, 'DELETE').then(function (res) {
        if (res.success === true) {
          let cart = res.data;
          cart.items = cart.items.map(v => {
            v.isChecked = false;
            return v;
          });
          cart.checkedSubCount = 0;
          that.setData({
            cart: cart
          });
        }
        that.setData({
          checkedAllStatus: that.isCheckedAll()
        });
      });
  }
})