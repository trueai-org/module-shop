import { resetSimpleData } from '@/services/api';

export default {
    namespace: 'simpledata',
    state: {
    },
    effects: {
        *reset({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(resetSimpleData);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};