var util = require('../../utils/util.js');
var api = require('../../config/api.js');

var app = getApp()
Page({
  data: {
    keywrod: '',
    searchStatus: false,
    goodsList: [],
    helpKeyword: [],
    historyKeyword: [],
    categoryFilter: false,
    currentSortType: 'default',
    currentSortOrder: '',
    filterCategory: [],
    defaultKeyword: {},
    hotKeyword: [],
    page: 1,
    size: 20,
    // categoryId: 0,

    categories: [{ id: 0, name: '全部' }],
    current: {},
    pageNum: 1,
    pageSize: 6,
    total: 0,
    predicate: 'id',
    reverse: true,
    name: '',
    categoryId: 0,
    pageData: []
  },
  //事件处理函数
  closeSearch: function () {
    wx.navigateBack()
  },
  clearKeyword: function () {
    this.setData({
      keyword: '',
      searchStatus: false
    });
  },
  navToGoods: function (e) {
    wx.navigateTo({
      url: `../goods/goods?id=${e.currentTarget.dataset.id}`
    })
  },
  onLoad: function () {
    let that = this;
    util.request(api.SubCategories)
      .then(function (res) {
        if (res.success === true && res.data && res.data.length > 0) {
          let origin_data = that.data.categories || [];
          let new_data = origin_data.concat(res.data);
          that.setData({
            categories: new_data,
            current: new_data[0]
          });
        }
      });

    this.getKeywords();
    // this.getSearchKeyword();
  },
  hideModal(e) {
    this.setData({
      categoryFilter: !this.data.categoryFilter,
    })
  },
  getSearchKeyword() {
    let that = this;
    util.request(api.SearchIndex).then(function (res) {
      if (res.errno === 0) {
        that.setData({
          historyKeyword: res.data.historyKeywordList,
          defaultKeyword: res.data.defaultKeyword,
          hotKeyword: res.data.hotKeywordList
        });
      }
    });
  },

  inputChange: function (e) {
    this.setData({
      keyword: e.detail.value,
      searchStatus: false
    });
    this.getHelpKeyword();
  },
  getHelpKeyword: function () {
    let that = this;
    util.request(api.SearchHelper, { keyword: that.data.keyword }).then(function (res) {
      if (res.errno === 0) {
        that.setData({
          helpKeyword: res.data
        });
      }
    });
  },
  getKeywords: function () {
    let that = this;
    util.request(api.Keywords).then(function (res) {
      if (res.success === true) {
        that.setData({
          defaultKeyword: res.data.defaultKeyword,
          hotKeyword: res.data.hotKeywords,
          historyKeyword: res.data.historyKeywords
        });
      }
    });
  },
  inputFocus: function () {
    this.setData({
      searchStatus: false,
      goodsList: []
    });

    if (this.data.keyword) {
      // this.getHelpKeyword();
    }
  },
  clearHistory: function () {
    this.setData({
      historyKeyword: []
    })

    util.request(api.ClearKeywords, {}, 'POST')
      .then(function (res) {
        console.log('清除成功');
      });
    // util.request(api.SearchClearHistory, {}, 'POST')
    //   .then(function (res) {
    //     console.log('清除成功');
    //   });
  },
  getGoodsList: function () {
    let that = this;
    util.request(api.GoodsList, { keyword: that.data.keyword, page: that.data.page, size: that.data.size, sort: that.data.currentSortType, order: that.data.currentSortOrder, categoryId: that.data.categoryId }).then(function (res) {
      if (res.errno === 0) {
        that.setData({
          searchStatus: true,
          categoryFilter: false,
          goodsList: res.data.data,
          filterCategory: res.data.filterCategory,
          page: res.data.currentPage,
          size: res.data.numsPerPage
        });
      }

      // 重新获取关键词
      // that.getSearchKeyword();
    });
  },
  onKeywordTap: function (event) {
    this.getSearchResult(event.target.dataset.keyword);
  },
  getSearchResult(keyword) {
    // this.setData({
    //   keyword: keyword,
    //   page: 1,
    //   categoryId: 0,
    //   goodsList: []
    // });
    // this.getGoodsList();

    this.setData({
      keyword: keyword,

      pageData: [],
      pageNum: 1,
      pageSize: this.data.pageSize,
      total: 0,
      predicate: 'id',
      reverse: true,
      name: keyword,
      categoryId: 0,
    }, () => {
      this.getGoods();
    });
  },
  openSortFilter: function (event) {
    let currentId = event.currentTarget.id;
    switch (currentId) {
      case 'categoryFilter':
        this.setData({
          'categoryFilter': !this.data.categoryFilter,
          'currentSortOrder': 'asc'
        });
        break;
      case 'priceSort':
        let tmpSortOrder = 'asc';
        if (this.data.currentSortOrder == 'asc') {
          tmpSortOrder = 'desc';
        }
        this.setData({
          'currentSortType': 'price',
          'currentSortOrder': tmpSortOrder,
          'categoryFilter': false
        });
        this.getGoodsList();
        break;
      default:
        //综合排序
        this.setData({
          'currentSortType': 'default',
          'currentSortOrder': 'desc',
          'categoryFilter': false
        });
        this.getGoodsList();
    }
  },
  selectCategory: function (event) {
    let currentIndex = event.target.dataset.categoryIndex;
    let filterCategory = this.data.filterCategory;
    let currentCategory = null;
    for (let key in filterCategory) {
      if (key == currentIndex) {
        filterCategory[key].selected = true;
        currentCategory = filterCategory[key];
      } else {
        filterCategory[key].selected = false;
      }
    }
    this.setData({
      'filterCategory': filterCategory,
      'categoryFilter': false,
      categoryId: currentCategory.id,
      page: 1,
      goodsList: []
    });
    this.getGoodsList();
  },
  onKeywordConfirm(event) {
    this.getSearchResult(event.detail.value);
  },
  onReachBottom: function () {
    // 当界面的下方距离页面底部距离小于100像素时触发回调
    if (this.data.total > 0 && this.data.pageNum * this.data.pageSize < this.data.total) {
      this.setData({
        pageNum: this.data.pageNum + 1
      }, () => {
        this.getGoods();
      });
    }
  },
  getGoods: function () {
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
        name: that.data.name,
        categoryId: that.data.categoryId,
      }
    };
    util.request(api.GoodsGrid, params, "POST")
      .then(function (res) {
        if (res.success === true) {
          let origin_data = that.data.pageData || [];
          let new_data = origin_data.concat(res.data.list)
          that.setData({
            searchStatus: true,
            categoryFilter: false,
            pageData: new_data,
            total: parseInt(res.data.pagination.total)
          });
        }

        // 重新获取关键词
        that.getKeywords();
      });
  },
  selectCategory2: function (event) {
    let id = event.currentTarget.dataset.id;
    if (this.data.current.id == id) {
      return false;
    }
    let first = this.data.categories.find(c => c.id == id);
    if (first) {
      this.setData({
        categoryFilter: false,
        current: first,
        pageData: [],
        pageNum: 1,
        pageSize: this.data.pageSize,
        total: 0,
        categoryId: first.id,
      }, () => {
        this.getGoods();
      });
    }
  },
  openSortFilter2: function (event) {
    let currentId = event.currentTarget.id;
    switch (currentId) {
      case 'categoryFilter':
        this.setData({
          'categoryFilter': !this.data.categoryFilter,
          'currentSortOrder': 'asc'
        });
        break;
      case 'priceSort':
        // let tmpSortOrder = 'asc';
        // if (this.data.currentSortOrder == 'asc') {
        //   tmpSortOrder = 'desc';
        // }
        // this.setData({
        //   'currentSortType': 'price',
        //   'currentSortOrder': tmpSortOrder,
        //   'categoryFilter': false
        // });
        // this.getGoodsList();
        this.setData({
          currentSortType: 'price',
          categoryFilter: false,
          predicate: 'price',
          reverse: !this.data.reverse,
          pageNum: 1,
          pageData: []
        }, () => {
          this.getGoods();
        });
        break;
      default:
        //综合排序
        // this.setData({
        //   'currentSortType': 'default',
        //   'currentSortOrder': 'desc',
        //   'categoryFilter': false
        // });
        // this.getGoodsList();
        this.setData({
          currentSortType: 'default',
          categoryFilter: false,
          predicate: 'id',
          reverse: true,
          pageNum: 1,
          pageData: []
        }, () => {
          this.getGoods();
        });
    }
  },
})