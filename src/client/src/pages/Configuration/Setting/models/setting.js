import { appsettings, appsettingPut } from '@/services/api';

export default {
    namespace: 'appsetting',
    state: {
    },
    effects: {
        *get({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(appsettings);
            !!resolve && resolve(response);
        },

        *put({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(appsettingPut, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};