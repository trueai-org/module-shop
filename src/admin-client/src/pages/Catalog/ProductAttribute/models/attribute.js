import {
    queryProductAttrGrid, deleteProductAttr, addProductAttr, editProductAttr,
    queryProductAttrDataGrid, addProductAttrData, editProductAttrData, deleteProductAttrData
} from '@/services/api';

export default {
    namespace: 'attribute',
    state: {
    },
    effects: {
        *queryProductAttrGrid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductAttrGrid, params);
            !!resolve && resolve(response);
        },

        *deleteProductAttr({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProductAttr, params);
            !!resolve && resolve(response);
        },

        *addProductAttr({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProductAttr, params);
            !!resolve && resolve(response);
        },

        *editProductAttr({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProductAttr, params);
            !!resolve && resolve(response);
        },

        *queryProductAttrDataGrid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductAttrDataGrid, params);
            !!resolve && resolve(response);
        },

        *addProductAttrData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProductAttrData, params);
            !!resolve && resolve(response);
        },

        *editProductAttrData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProductAttrData, params);
            !!resolve && resolve(response);
        },

        *deleteProductAttrData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProductAttrData, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};