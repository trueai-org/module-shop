import { queryBrand, deleteBrand, firstBrand, addBrand, editBrand } from '@/services/api';

export default {
    namespace: 'brand',
    state: {

    },

    effects: {
        *queryBrand({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryBrand, params);
            !!resolve && resolve(response);
        },

        *deleteBrand({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteBrand, params);
            !!resolve && resolve(response);
        },

        *addBrand({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addBrand, params);
            !!resolve && resolve(response);
        },

        *editBrand({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editBrand, params);
            !!resolve && resolve(response);
        },

        *firstBrand({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstBrand, params);
            !!resolve && resolve(response);
        },
    },

    reducers: {

    },
};