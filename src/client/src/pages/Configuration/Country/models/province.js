import {
    firstProvince, queryProvinceGrid, addProvince, editProvince, deleteProvince, queryProvinceTree
} from '@/services/api';

export default {
    namespace: 'province',
    state: {
    },
    effects: {
        *get({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstProvince, params);
            !!resolve && resolve(response);
        },

        *tree({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProvinceTree, params);
            !!resolve && resolve(response);
        },

        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProvinceGrid, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProvince, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProvince, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProvince, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};