import { queryUnitAll, addUnit, editUnit, deleteUnit } from '@/services/api';

export default {
    namespace: 'unit',
    state: {
    },
    effects: {
        *list({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryUnitAll, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteUnit, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addUnit, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editUnit, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};