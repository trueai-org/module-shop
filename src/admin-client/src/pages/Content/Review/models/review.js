import { reviewsGrid, reviewsUpdate, reviewsDelete } from '@/services/api';

export default {
    namespace: 'reviews',
    state: {
    },
    effects: {
        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(reviewsGrid, params);
            !!resolve && resolve(response);
        },
        *put({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(reviewsUpdate, params);
            !!resolve && resolve(response);
        },
        *del({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(reviewsDelete, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};