import {
    queryCountryAll, queryProvinceTree, queryWarehouseAll,
    queryUserQuickSearch, queryUserAddresses, systemInfo
} from '@/services/api';

export default {
    namespace: 'system',
    state: {

    },

    effects: {
        *countries({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryCountryAll, params);
            !!resolve && resolve(response);
        },

        *provinces({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProvinceTree, params);
            !!resolve && resolve(response);
        },

        *warehouses({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryWarehouseAll, params);
            !!resolve && resolve(response);
        },

        *users({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryUserQuickSearch, params);
            !!resolve && resolve(response);
        },

        *userAddresses({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryUserAddresses, params);
            !!resolve && resolve(response);
        },

        *info({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(systemInfo);
            !!resolve && resolve(response);
        },
    },

    reducers: {

    },
};