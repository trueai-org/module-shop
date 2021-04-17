import {
    queryProductFirst, queryProductGrid, addProduct, editProduct, deleteProduct,
    publishProduct, unpublishProduct, copyProduct,
    queryStockHistoryGrid

} from '@/services/api';

export default {
    namespace: 'product',
    state: {
    },
    effects: {
        *get({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductFirst, params);
            !!resolve && resolve(response);
        },

        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductGrid, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProduct, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProduct, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProduct, params);
            !!resolve && resolve(response);
        },

        *publish({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(publishProduct, params);
            !!resolve && resolve(response);
        },

        *unpublish({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(unpublishProduct, params);
            !!resolve && resolve(response);
        },

        *copy({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(copyProduct, params);
            !!resolve && resolve(response);
        },

        *stockHistories({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryStockHistoryGrid, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};