import {
    queryCategory, categoryInMenuSwitch, deleteCategory, categories, addCategory,
    firstCategory, editCategory, uploadImage
} from '@/services/api';

export default {
    namespace: 'category',
    state: {},

    effects: {
        *queryCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryCategory, params);
            !!resolve && resolve(response);
        },

        *categories({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(categories);
            !!resolve && resolve(response);
        },

        *switchCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(categoryInMenuSwitch, params);
            !!resolve && resolve(response);
        },

        *deleteCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteCategory, params);
            !!resolve && resolve(response);
        },

        *addCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addCategory, params);
            !!resolve && resolve(response);
        },

        *editCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editCategory, params);
            !!resolve && resolve(response);
        },

        *uploadImage({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(uploadImage, params);
            !!resolve && resolve(response);
        },

        *firstCategory({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstCategory, params);
            !!resolve && resolve(response);
        },
    },

    reducers: {

    },
};