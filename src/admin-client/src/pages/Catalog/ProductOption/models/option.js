import {
    firstProductOption,
    queryProductOptionGrid, deleteProductOption, addProductOption, editProductOption,
    queryProductOptionDataGrid, addProductOptionData, editProductOptionData, deleteProductOptionData,
} from '@/services/api';

export default {
    namespace: 'option',
    state: {
    },
    effects: {
        *get({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstProductOption, params);
            !!resolve && resolve(response);
        },

        *queryProductOptionGrid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductOptionGrid, params);
            !!resolve && resolve(response);
        },

        *deleteProductOption({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProductOption, params);
            !!resolve && resolve(response);
        },

        *addProductOption({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProductOption, params);
            !!resolve && resolve(response);
        },

        *editProductOption({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProductOption, params);
            !!resolve && resolve(response);
        },

        *queryProductOptionDataGrid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductOptionDataGrid, params);
            !!resolve && resolve(response);
        },

        *addProductOptionData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProductOptionData, params);
            !!resolve && resolve(response);
        },

        *editProductOptionData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProductOptionData, params);
            !!resolve && resolve(response);
        },

        *deleteProductOptionData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProductOptionData, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};