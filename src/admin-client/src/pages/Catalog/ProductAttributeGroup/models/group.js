import { queryProductAGS, deleteProductAGS, addProductAGS, editProductAGS } from '@/services/api';

export default {
    namespace: 'group',
    state: {
    },
    effects: {
        *queryProductAGS({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(queryProductAGS);
            !!resolve && resolve(response);
        },

        *deleteProductAGS({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProductAGS, params);
            !!resolve && resolve(response);
        },

        *addProductAGS({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProductAGS, params);
            !!resolve && resolve(response);
        },

        *editProductAGS({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProductAGS, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};