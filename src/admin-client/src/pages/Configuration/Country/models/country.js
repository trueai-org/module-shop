import {
    firstCountry, queryCountryGrid, addCountry, editCountry, deleteCountry
} from '@/services/api';

export default {
    namespace: 'country',
    state: {
    },
    effects: {
        *get({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstCountry, params);
            !!resolve && resolve(response);
        },

        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryCountryGrid, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteCountry, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addCountry, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editCountry, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};