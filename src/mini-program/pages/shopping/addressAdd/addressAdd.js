var util = require('../../../utils/util.js');
var api = require('../../../config/api.js');
var app = getApp();

Page({
  data: {
    address: {},
    userAddressId: 0,
    openSelectRegion: false,
    fistOpen: true,

    value: [0, 0, 0],
    allCitys: [],
    allDistricts: [],
    provinces: [],
    citys: [],
    districts: [],
    provinceStr: ''
  },

  bindChange: function (e) {
    const val = e.detail.value;
    if (!val || val[0] < 0)
      return;
    if (val[1] < 0 || val[2] < 0) {
      this.setData({
        citys: [],
        districts: [],
      })
      return;
    }
    let p = this.data.provinces[val[0]];
    let cs = [];
    let c = undefined;
    if (p) {
      cs = this.data.allCitys.filter(c => c.parentId == p.id);
    }
    if (cs.length > val[1]) {
      c = cs[val[1]];
    }
    let ds = [];
    let d = undefined;
    if (c) {
      ds = this.data.allDistricts.filter(x => x.parentId == c.id);
    }
    if (ds.length > val[2]) {
      d = ds[val[2]];
    }
    this.setData({
      value: val,
      citys: cs,
      districts: ds,
      provinceStr: (p ? p.name : '') + ' ' + (c ? c.name : '') + ' ' + (d ? d.name : '')
    })
  },
  bindinputMobile(event) {
    let address = this.data.address;
    address.phone = event.detail.value;
    this.setData({
      address: address
    });
  },
  bindinputName(event) {
    let address = this.data.address;
    address.contactName = event.detail.value;
    this.setData({
      address: address
    });
  },
  bindinputAddress(event) {
    let address = this.data.address;
    address.addressLine1 = event.detail.value;
    this.setData({
      address: address
    });
  },
  bindIsDefault() {
    let address = this.data.address;
    address.isDefault = !address.isDefault;
    this.setData({
      address: address
    });
  },
  getAddressDetail() {
    let that = this;
    util.request(api.Addresses + '/' + that.data.userAddressId).then(function (res) {
      if (res.success === true) {
        that.setData({
          address: res.data
        }, () => {
          that.getRegionList(res.data);
        });
      }
    });
  },
  chooseRegion() {
    let that = this;
    this.setData({
      openSelectRegion: !this.data.openSelectRegion
    }, () => {
      if (this.data.fistOpen && !this.data.userAddressId) {
        this.bindChange({ detail: { value: that.data.value } });
        this.setData({ fistOpen: false });
      }
    });
  },
  onLoad: function (options) {
    // 页面初始化 options为页面跳转所带来的参数
    if (options.id) {
      this.setData({
        userAddressId: options.id
      });
      this.getAddressDetail();
    } else {
      this.getRegionList();
    }
  },
  onReady: function () {

  },
  cancelSelectRegion() {
    this.setData({
      openSelectRegion: false
    });
  },
  getRegionList(info) {
    let that = this;
    util.request(api.Provinces).then(function (res) {
      if (res.success === true) {
        that.setData({
          provinces: res.data.provinces,
          allCitys: res.data.citys,
          allDistricts: res.data.districts,
          citys: [],
          districts: [],
        });

        if (info) {
          let i0 = 0, i1 = 0, i2 = 0;
          let p = undefined, c = undefined, d = undefined;
          let cs = [], ds = [];
          i0 = that.data.provinces.findIndex(c => c.id == info.stateOrProvinceId);
          if (i0 >= 0) {
            p = that.data.provinces[i0];;
            if (p) {
              cs = that.data.allCitys.filter(c => c.parentId == p.id);
              i1 = cs.findIndex(c => c.id == info.cityId);
              if (i1 >= 0) {
                c = cs[i1];
                ds = that.data.allDistricts.filter(x => x.parentId == c.id);
                if (info.districtId && info.districtId >= 0 && ds.length >= 0) {
                  i2 = ds.findIndex(c => c.id == info.districtId)
                  if (i2 >= 0) {
                    d = ds[i2];
                  }
                }
              }
            }
          }
          i0 = i0 < 0 ? 0 : i0;
          i1 = i1 < 0 ? 0 : i1;
          i2 = i2 < 0 ? 0 : i2;
          that.setData({
            value: [i0, i1, i2],
            citys: cs,
            districts: ds,
            provinceStr: (p ? p.name : '') + ' ' + (c ? c.name : '') + ' ' + (d ? d.name : '')
          });
        }
      }
    });
  },
  cancelAddress() {
    wx.reLaunch({
      url: '/pages/shopping/address/address',
    })
  },
  saveAddress() {
    let address = this.data.address;
    if (address.contactName == '') {
      util.showErrorToast('请输入姓名');
      return false;
    }

    if (address.phone == '') {
      util.showErrorToast('请输入手机号码');
      return false;
    }

    if (address.addressLine1 == '') {
      util.showErrorToast('请输入详细地址');
      return false;
    }

    let that = this;
    if (that.data.citys.length <= 0 || that.data.districts.length <= 0) {
      util.showErrorToast('请选择省/市/区县');
      return false;
    }

    let value = that.data.value;
    let stateOrProvinceId, cityId, districtId;
    stateOrProvinceId = that.data.provinces[value[0]].id;
    cityId = that.data.citys[value[1]].id;
    districtId = that.data.districts[value[2]].id;

    let url = api.Addresses;
    let method = 'POST';
    if (that.data.userAddressId) {
      url += '/' + that.data.userAddressId;
      method = 'PUT';
    }
    util.request(url, {
      contactName: address.contactName,
      phone: address.phone,
      isDefault: address.isDefault,
      addressLine1: address.addressLine1,
      stateOrProvinceId: stateOrProvinceId,
      cityId: cityId,
      districtId: districtId,
    }, method).then(function (res) {
      if (res.success === true) {
        wx.reLaunch({
          url: '/pages/shopping/address/address',
        })
      } else {
        util.showErrorToast(res.message);
      }
    });

  },
  onShow: function () {
    // 页面显示

  },
  onHide: function () {
    // 页面隐藏

  },
  onUnload: function () {
    // 页面关闭

  }
})