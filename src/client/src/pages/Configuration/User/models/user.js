import {
    firstUser, queryUserGrid, addUser, editUser, deleteUser
} from '@/services/api';

export default {
    namespace: 'userManager',
    state: {
    },
    effects: {
        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryUserGrid, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteUser, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addUser, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editUser, params);
            !!resolve && resolve(response);
        },

        *get({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstUser, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};