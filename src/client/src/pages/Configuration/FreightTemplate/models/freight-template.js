import {
    queryFreightTemplateGrid, addFreightTemplate, editFreightTemplate, deleteFreightTemplate
} from '@/services/api';

export default {
    namespace: 'freight-template',
    state: {
    },
    effects: {
        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryFreightTemplateGrid, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteFreightTemplate, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addFreightTemplate, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editFreightTemplate, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};