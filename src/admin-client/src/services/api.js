import { stringify } from 'qs';
import request from '@/utils/request';
import requestUpload from '@/utils/requestUpload';

export async function queryProjectNotice() {
  return request('/api/project/notice');
}

export async function queryActivities() {
  return request('/api/activities');
}

export async function queryRule(params) {
  return request(`/api/rule?${stringify(params)}`);
}

export async function removeRule(params) {
  return request('/api/rule', {
    method: 'POST',
    body: {
      ...params,
      method: 'delete',
    },
  });
}

export async function addRule(params) {
  return request('/api/rule', {
    method: 'POST',
    body: {
      ...params,
      method: 'post',
    },
  });
}

export async function updateRule(params) {
  return request('/api/rule', {
    method: 'POST',
    body: {
      ...params,
      method: 'update',
    },
  });
}

export async function fakeChartData() {
  return request('/api/fake_chart_data');
}

export async function queryTags() {
  return request('/api/tags');
}

export async function fakeAccountLogin(params) {
  return request('/api/login/account', {
    method: 'POST',
    body: params,
  });
}

export async function fakeRegister(params) {
  return request('/api/register', {
    method: 'POST',
    body: params,
  });
}

export async function queryNotices() {
  return request('/api/notices');
}

export async function getFakeCaptcha(mobile) {
  return request(`/api/captcha?mobile=${mobile}`);
}

/**
 * 
 *
 */
export async function loginAdmin(params) {
  return request('/api/account/login', {
    method: 'POST',
    body: params,
  });
}

// 退出
export async function logoutAdmin() {
  return request('/api/account/logout', {
    method: 'POST'
  });
}

export async function loginPhoneGetCaptcha(params) {
  return request('/api/account/login-phone-captcha', {
    method: 'POST',
    body: params,
  });
}

export async function loginPhone(params) {
  return request('/api/account/login-phone', {
    method: 'POST',
    body: params,
  });
}

// 双因子登录 - 发送验证码
export async function loginVerifyTwoFactor(params) {
  return request('/api/account/login-verify-two-factor', {
    method: 'POST',
    body: params,
  });
}

// 双因子登录
export async function loginTwoFactor(params) {
  return request('/api/account/login-two-factor', {
    method: 'POST',
    body: params,
  });
}

// 注册
// 验证注册手机号，并发送短信验证码
export async function registerVerifyPhoneAndGetCaptcha(params) {
  return request('/api/account/register-verify-phone', {
    method: 'POST',
    body: params,
  });
}

// 手机号注册
export async function registerByPhone(params) {
  return request('/api/account/register-by-phone', {
    method: 'POST',
    body: params,
  });
}

// 邮箱激活验证
export async function confirmEmail(params) {
  return request(`/api/account/confirm-email`, {
    method: 'PUT',
    body: params,
  });
}

// 找回密码
export async function getForgotPassword(params) {
  return request(`/api//account/forgot-password?name=${params.name}`);
}

// 邮箱找回 - 发送密码重置邮件
export async function forgotPasswordSendEmail(params) {
  return request(`/api/account/forgot-password-email`, {
    method: 'POST',
    body: params,
  });
}

// 邮箱找回 - 重置密码
export async function resetPasswordByEmail(params) {
  return request(`/api/account/reset-password-email`, {
    method: 'PUT',
    body: params,
  });
}

// 手机找回 - 发送验证码
export async function forgotPasswordSendPhone(params) {
  return request(`/api/account/forgot-password-phone`, {
    method: 'POST',
    body: params,
  });
}

// 手机找回 - 重置密码
export async function resetPasswordByPhone(params) {
  return request(`/api/account/reset-password-phone`, {
    method: 'PUT',
    body: params,
  });
}

// 移除手机绑定
export async function removePhone(params) {
  return request(`/api/account/remove-phone`, {
    method: 'POST',
    body: params,
  });
}

// 移除邮箱绑定
export async function removeEmail(params) {
  return request(`/api/account/remove-email`, {
    method: 'POST',
    body: params,
  });
}

// 添加绑定
// 添加手机绑定 - 获取验证码
export async function addPhoneGetCaptcha(params) {
  return request(`/api/account/add-phone-captcha`, {
    method: 'POST',
    body: params,
  });
}
// 添加手机绑定
export async function addPhone(params) {
  return request(`/api/account/add-phone`, {
    method: 'PUT',
    body: params,
  });
}

// 添加邮箱绑定 - 发送绑定链接
export async function addEmailSendToken(params) {
  return request(`/api/account/add-email`, {
    method: 'POST',
    body: params,
  });
}
// 邮箱已绑定，但未验证 - 发送确认邮件
export async function sendConfirmEmail(params) {
  return request(`/api/account/send-confirm-email`, {
    method: 'POST',
    body: params,
  });
}
// 添加邮箱绑定
export async function addEmail(params) {
  return request(`/api/account/add-email`, {
    method: 'PUT',
    body: params,
  });
}

// 修改密码
export async function changePassword(params) {
  return request(`/api/account/change-password`, {
    method: 'PUT',
    body: params,
  });
}

// 分类

export async function categories() {
  return request('/api/categories');
}

export async function queryCategory(params) {
  return request('/api/categories/grid', {
    method: 'POST',
    body: params,
  });
}

export async function categoryInMenuSwitch(params) {
  return request(`/api/categories/switch/${params.id}`, {
    method: 'PUT',
  });
}

export async function deleteCategory(params) {
  return request(`/api/categories/${params.id}`, {
    method: 'DELETE',
  });
}

export async function firstCategory(params) {
  return request(`/api/categories/${params.id}`);
}

export async function addCategory(params) {
  return request(`/api/categories`, {
    method: 'POST',
    body: params,
  });
}

export async function editCategory(params) {
  return request(`/api/categories/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 品牌

export async function queryBrand(params) {
  return request('/api/brands/grid', {
    method: 'POST',
    body: params,
  });
}

export async function queryBrandAll() {
  return request('/api/brands');
}

export async function deleteBrand(params) {
  return request(`/api/brands/${params.id}`, {
    method: 'DELETE',
  });
}

export async function firstBrand(params) {
  return request(`/api/brands/${params.id}`);
}

export async function addBrand(params) {
  return request(`/api/brands`, {
    method: 'POST',
    body: params,
  });
}

export async function editBrand(params) {
  return request(`/api/brands/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 上传

export async function uploadImage(params) {
  return requestUpload(`/api/upload`, {
    method: 'POST',
    body: params,
  });
}

export async function uploadMultipleImage(params) {
  return requestUpload(`/api/upload/multiple`, {
    method: 'POST',
    body: params,
  });
}

// 产品选项
export async function firstProductOption(params) {
  return request(`/api/product-options/${params.id}`);
}

export async function queryProductOption(params) {
  return request('/api/product-options');
}

export async function queryProductOptionGrid(params) {
  return request(`/api/product-options/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function deleteProductOption(params) {
  return request(`/api/product-options/${params.id}`, {
    method: 'DELETE',
  });
}

export async function addProductOption(params) {
  return request(`/api/product-options`, {
    method: 'POST',
    body: params,
  });
}

export async function editProductOption(params) {
  return request(`/api/product-options/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 产品选项值

export async function queryProductOptionData(params) {
  return request(`/api/product-options/data/${params.optionId}`);
}

export async function queryProductOptionDataGrid(params) {
  return request(`/api/product-options/data/${params.optionId}/grid`, {
    method: 'POST',
    body: params,
  });
}
export async function addProductOptionData(params) {
  return request(`/api/product-options/data/${params.optionId}`, {
    method: 'POST',
    body: params,
  });
}

export async function editProductOptionData(params) {
  return request(`/api/product-options/data/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteProductOptionData(params) {
  return request(`/api/product-options/data/${params.id}`, {
    method: 'DELETE',
  });
}

// 产品属性组

export async function queryProductAGS(params) {
  return request('/api/product-attribute-groups');
}

export async function deleteProductAGS(params) {
  return request(`/api/product-attribute-groups/${params.id}`, {
    method: 'DELETE',
  });
}

export async function addProductAGS(params) {
  return request(`/api/product-attribute-groups`, {
    method: 'POST',
    body: params,
  });
}

export async function editProductAGS(params) {
  return request(`/api/product-attribute-groups/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 产品属性

export async function queryProductAttr() {
  return request('/api/product-attributes');
}

export async function queryProductAttributeGroupArray() {
  return request('/api/product-attributes/group-array');
}

export async function queryProductAttrGrid(params) {
  return request(`/api/product-attributes/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function deleteProductAttr(params) {
  return request(`/api/product-attributes/${params.id}`, {
    method: 'DELETE',
  });
}

export async function addProductAttr(params) {
  return request(`/api/product-attributes`, {
    method: 'POST',
    body: params,
  });
}

export async function editProductAttr(params) {
  return request(`/api/product-attributes/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 产品属性值

export async function queryProductAttrData(params) {
  return request(`/api/product-attributes/data/${params.attributeId}`);
}

export async function queryProductAttrDataGrid(params) {
  return request(`/api/product-attributes/data/${params.attributeId}/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function addProductAttrData(params) {
  return request(`/api/product-attributes/data/${params.attributeId}`, {
    method: 'POST',
    body: params,
  });
}

export async function editProductAttrData(params) {
  return request(`/api/product-attributes/data/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteProductAttrData(params) {
  return request(`/api/product-attributes/data/${params.id}`, {
    method: 'DELETE',
  });
}

//产品
export async function queryProductFirst(params) {
  return request(`/api/products/${params.id}`);
}

export async function queryProductGrid(params) {
  return request(`/api/products/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function addProduct(params) {
  return request(`/api/products`, {
    method: 'POST',
    body: params,
  });
}

export async function editProduct(params) {
  return request(`/api/products/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteProduct(params) {
  return request(`/api/products/${params.id}`, {
    method: 'DELETE',
  });
}

export async function publishProduct(params) {
  return request(`/api/products/${params.id}/publish`, {
    method: 'PUT',
    body: params,
  });
}

export async function unpublishProduct(params) {
  return request(`/api/products/${params.id}/unpublish`, {
    method: 'PUT',
    body: params,
  });
}

export async function copyProduct(params) {
  return request(`/api/products/${params.id}/clone`, {
    method: 'POST',
    body: params,
  });
}

// 产品属性模板

export async function firstProductAttributeTemplate(params) {
  return request(`/api/product-attribute-templates/${params.id}`);
}

export async function queryProductAttributeTemplates() {
  return request('/api/product-attribute-templates');
}

export async function queryProductAttributeTemplateGrid(params) {
  return request(`/api/product-attribute-templates/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function deleteProductAttributeTemplate(params) {
  return request(`/api/product-attribute-templates/${params.id}`, {
    method: 'DELETE',
  });
}

export async function addProductAttributeTemplate(params) {
  return request(`/api/product-attribute-templates`, {
    method: 'POST',
    body: params,
  });
}

export async function editProductAttributeTemplate(params) {
  return request(`/api/product-attribute-templates/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

//国家、省市区、、、
export async function firstCountry(params) {
  return request(`/api/countries/${params.id}`);
}

export async function queryCountryAll(params) {
  return request(`/api/countries`);
}

export async function queryCountryGrid(params) {
  return request(`/api/countries/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function addCountry(params) {
  return request(`/api/countries`, {
    method: 'POST',
    body: params,
  });
}

export async function editCountry(params) {
  return request(`/api/countries/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteCountry(params) {
  return request(`/api/countries/${params.id}`, {
    method: 'DELETE',
  });
}

// 省市区

export async function firstProvince(params) {
  return request(`/api/countries/provinces/${params.id}`);
}

export async function queryProvinceTree(params) {
  return request(`/api/countries/provinces/tree/${params.countryId}`);
}

export async function queryProvinceGrid(params) {
  return request(`/api/countries/provinces/grid/${params.countryId}`, {
    method: 'POST',
    body: params,
  });
}

export async function addProvince(params) {
  return request(`/api/countries/provinces/${params.countryId}`, {
    method: 'POST',
    body: params,
  });
}

export async function editProvince(params) {
  return request(`/api/countries/provinces/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteProvince(params) {
  return request(`/api/countries/provinces/${params.id}`, {
    method: 'DELETE',
  });
}

//用户
export async function queryUserAddresses(params) {
  return request(`/api/users/${params.userId}/addresses`);
}

export async function queryUserQuickSearch(params) {
  return request(
    `/api/users/quick-search?take=${params.take || 20}&nameOrPhone=${params.nameOrPhone || ''}`
  );
}

export async function firstUser(params) {
  return request(`/api/users/${params.id}`);
}

export async function queryUserGrid(params) {
  return request(`/api/users/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function addUser(params) {
  return request(`/api/users`, {
    method: 'POST',
    body: params,
  });
}

export async function editUser(params) {
  return request(`/api/users/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteUser(params) {
  return request(`/api/users/${params.id}`, {
    method: 'DELETE',
  });
}

//仓库
export async function queryWarehouseAll() {
  return request(`/api/warehouses`);
}

export async function firstWarehouse(params) {
  return request(`/api/warehouses/${params.id}`);
}

export async function queryWarehouseGrid(params) {
  return request(`/api/warehouses/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function addWarehouse(params) {
  return request(`/api/warehouses`, {
    method: 'POST',
    body: params,
  });
}

export async function editWarehouse(params) {
  return request(`/api/warehouses/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteWarehouse(params) {
  return request(`/api/warehouses/${params.id}`, {
    method: 'DELETE',
  });
}

//库存历史
export async function queryStockHistoryGrid(params) {
  return request(`/api/stocks-histories/grid`, {
    method: 'POST',
    body: params,
  });
}

//单位
export async function queryUnitAll() {
  return request(`/api/units`);
}

export async function addUnit(params) {
  return request(`/api/units`, {
    method: 'POST',
    body: params,
  });
}

export async function editUnit(params) {
  return request(`/api/units/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteUnit(params) {
  return request(`/api/units/${params.id}`, {
    method: 'DELETE',
  });
}

//运费模板
export async function queryFreightTemplateAll() {
  return request(`/api/shippings/freight-templates`);
}

export async function queryFreightTemplateGrid(params) {
  return request(`/api/shippings/freight-templates/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function addFreightTemplate(params) {
  return request(`/api/shippings/freight-templates`, {
    method: 'POST',
    body: params,
  });
}

export async function editFreightTemplate(params) {
  return request(`/api/shippings/freight-templates/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteFreightTemplate(params) {
  return request(`/api/shippings/freight-templates/${params.id}`, {
    method: 'DELETE',
  });
}

//运费模板 - 价格和目的地配置
export async function queryPriceAndDestinationGrid(params) {
  return request(`/api/shippings/price-destinations/grid/${params.freightTemplateId}`, {
    method: 'POST',
    body: params,
  });
}

export async function addPriceAndDestination(params) {
  return request(`/api/shippings/price-destinations/${params.freightTemplateId}`, {
    method: 'POST',
    body: params,
  });
}

export async function editPriceAndDestination(params) {
  return request(`/api/shippings/price-destinations/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deletePriceAndDestination(params) {
  return request(`/api/shippings/price-destinations/${params.id}`, {
    method: 'DELETE',
  });
}

// 订单
export async function queryOrderGrid(params) {
  return request(`/api/orders/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function firstOrder(params) {
  return request(`/api/orders/${params.id}`);
}

export async function firstOrderByNo(params) {
  return request(`/api/orders/${params.no}/no`);
}

export async function addOrder(params) {
  return request(`/api/orders`, {
    method: 'POST',
    body: params,
  });
}

export async function editOrder(params) {
  return request(`/api/orders/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function deleteOrder(params) {
  return request(`/api/orders/${params.id}`, {
    method: 'DELETE',
  });
}

export async function cancelOrder(params) {
  return request(`/api/orders/${params.id}/cancel`, {
    method: 'PUT',
    body: params,
  });
}

export async function onHoldOrder(params) {
  return request(`/api/orders/${params.id}/on-hold`, {
    method: 'PUT',
    body: params,
  });
}

export async function paymentOrder(params) {
  return request(`/api/orders/${params.id}/payment`, {
    method: 'PUT',
    body: params,
  });
}

export async function queryOrderHistory(params) {
  return request(`/api/orders/history/${params.orderId}`);
}

export async function orderShipment(params) {
  return request(`/api/orders/${params.id}/shipment`, {
    method: 'POST',
    body: params,
  });
}

// 发货
export async function shipmentGrid(params) {
  return request(`/api/shipments/grid`, {
    method: 'POST',
    body: params,
  });
}

// 商城配置
// 获取全部及配置
export async function appsettings() {
  return request(`/api/appsettings`);
}

// 更新配置
export async function appsettingPut(params) {
  return request(`/api/appsettings`, {
    method: 'PUT',
    body: params,
  });
}

// 系统
// 系统信息
export async function systemInfo() {
  return request(`/api/system/info`);
}

// 内容管理

// 小部件
export async function widgetInstances() {
  return request(`/api/widget-instances`);
}

export async function deleteWidgetInstance(params) {
  return request(`/api/widget-instances/${params.id}`, {
    method: 'DELETE',
  });
}


// 分类部件
export async function getWidgetCategory(params) {
  return request(`/api/widget-categories/${params.id}`);
}
export async function addWidgetCategory(params) {
  return request(`/api/widget-categories`, {
    method: 'POST',
    body: params,
  });
}
export async function editWidgetCategory(params) {
  return request(`/api/widget-categories/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// html部件
export async function getWidgetHtml(params) {
  return request(`/api/widget-html/${params.id}`);
}
export async function addWidgetHtml(params) {
  return request(`/api/widget-html`, {
    method: 'POST',
    body: params,
  });
}
export async function editWidgetHtml(params) {
  return request(`/api/widget-html/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 最近浏览部件
export async function getWidgetRecentlyViewed(params) {
  return request(`/api/widget-recently-viewed/${params.id}`);
}
export async function addWidgetRecentlyViewed(params) {
  return request(`/api/widget-recently-viewed`, {
    method: 'POST',
    body: params,
  });
}
export async function editWidgetRecentlyViewed(params) {
  return request(`/api/widget-recently-viewed/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 简单商品部件
export async function getWidgetSimpleProduct(params) {
  return request(`/api/widget-simple-products/${params.id}`);
}
export async function addWidgetSimpleProduct(params) {
  return request(`/api/widget-simple-products`, {
    method: 'POST',
    body: params,
  });
}
export async function editWidgetSimpleProduct(params) {
  return request(`/api/widget-simple-products/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 商品部件
export async function getWidgetProduct(params) {
  return request(`/api/widget-products/${params.id}`);
}
export async function addWidgetProduct(params) {
  return request(`/api/widget-products`, {
    method: 'POST',
    body: params,
  });
}
export async function editWidgetProduct(params) {
  return request(`/api/widget-products/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 轮播部件
export async function getWidgetCarousel(params) {
  return request(`/api/widget-carousels/${params.id}`);
}
export async function addWidgetCarousel(params) {
  return request(`/api/widget-carousels`, {
    method: 'POST',
    body: params,
  });
}
export async function editWidgetCarousel(params) {
  return request(`/api/widget-carousels/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

// 评论管理
export async function reviewsGrid(params) {
  return request(`/api/admin-reviews/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function reviewsUpdate(params) {
  return request(`/api/admin-reviews/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function reviewsDelete(params) {
  return request(`/api/admin-reviews/${params.id}`, {
    method: 'DELETE',
  });
}

// 回复管理
export async function repliesGrid(params) {
  return request(`/api/admin-replies/grid`, {
    method: 'POST',
    body: params,
  });
}

export async function repliesUpdate(params) {
  return request(`/api/admin-replies/${params.id}`, {
    method: 'PUT',
    body: params,
  });
}

export async function repliesDelete(params) {
  return request(`/api/admin-replies/${params.id}`, {
    method: 'DELETE',
  });
}

// 样本数据
export async function resetSimpleData() {
  return request(`/api/sample-data`, {
    method: 'POST',
    body: {},
  });
}