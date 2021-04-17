import {
    firstWarehouse, queryWarehouseGrid, addWarehouse, editWarehouse, deleteWarehouse
} from '@/services/api';

export default {
    namespace: 'warehouse',
    state: {
    },
    effects: {
        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryWarehouseGrid, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteWarehouse, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addWarehouse, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editWarehouse, params);
            !!resolve && resolve(response);
        },

        *get({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstWarehouse, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};