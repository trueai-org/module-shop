import { repliesGrid, repliesUpdate, repliesDelete } from '@/services/api';

export default {
    namespace: 'replies',
    state: {
    },
    effects: {
        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(repliesGrid, params);
            !!resolve && resolve(response);
        },
        *put({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(repliesUpdate, params);
            !!resolve && resolve(response);
        },
        *del({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(repliesDelete, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};