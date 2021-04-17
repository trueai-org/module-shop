import {
    widgetInstances, deleteWidgetInstance,
    getWidgetCategory, addWidgetCategory, editWidgetCategory,
    getWidgetHtml, addWidgetHtml, editWidgetHtml,
    getWidgetRecentlyViewed, addWidgetRecentlyViewed, editWidgetRecentlyViewed,
    getWidgetSimpleProduct, addWidgetSimpleProduct, editWidgetSimpleProduct,
    getWidgetProduct, addWidgetProduct, editWidgetProduct,
    getWidgetCarousel, addWidgetCarousel, editWidgetCarousel,
} from '@/services/api';

export default {
    namespace: 'widget',
    state: {},
    effects: {
        *list({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(widgetInstances, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteWidgetInstance, params);
            !!resolve && resolve(response);
        },

        *getWidgetCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(getWidgetCategory, params);
            !!resolve && resolve(response);
        },

        *addWidgetCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addWidgetCategory, params);
            !!resolve && resolve(response);
        },

        *editWidgetCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editWidgetCategory, params);
            !!resolve && resolve(response);
        },

        *getWidgetHtml({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(getWidgetHtml, params);
            !!resolve && resolve(response);
        },

        *addWidgetHtml({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addWidgetHtml, params);
            !!resolve && resolve(response);
        },

        *editWidgetHtml({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editWidgetHtml, params);
            !!resolve && resolve(response);
        },

        *getWidgetRecentlyViewed({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(getWidgetRecentlyViewed, params);
            !!resolve && resolve(response);
        },

        *addWidgetRecentlyViewed({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addWidgetRecentlyViewed, params);
            !!resolve && resolve(response);
        },

        *editWidgetRecentlyViewed({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editWidgetRecentlyViewed, params);
            !!resolve && resolve(response);
        },

        *getWidgetSimpleProduct({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(getWidgetSimpleProduct, params);
            !!resolve && resolve(response);
        },

        *addWidgetSimpleProduct({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addWidgetSimpleProduct, params);
            !!resolve && resolve(response);
        },

        *editWidgetSimpleProduct({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editWidgetSimpleProduct, params);
            !!resolve && resolve(response);
        },

        *getWidgetProduct({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(getWidgetProduct, params);
            !!resolve && resolve(response);
        },

        *addWidgetProduct({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addWidgetProduct, params);
            !!resolve && resolve(response);
        },

        *editWidgetProduct({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editWidgetProduct, params);
            !!resolve && resolve(response);
        },

        *getWidgetCarousel({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(getWidgetCarousel, params);
            !!resolve && resolve(response);
        },

        *addWidgetCarousel({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addWidgetCarousel, params);
            !!resolve && resolve(response);
        },

        *editWidgetCarousel({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editWidgetCarousel, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {},
};