import {
    queryProductAttributeTemplateGrid, deleteProductAttributeTemplate, addProductAttributeTemplate, editProductAttributeTemplate
} from '@/services/api';


export default {
    namespace: 'template',
    state: {
    },
    effects: {
        *queryProductAttributeTemplateGrid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductAttributeTemplateGrid, params);
            !!resolve && resolve(response);
        },

        *deleteProductAttributeTemplate({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deleteProductAttributeTemplate, params);
            !!resolve && resolve(response);
        },

        *addProductAttributeTemplate({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addProductAttributeTemplate, params);
            !!resolve && resolve(response);
        },

        *editProductAttributeTemplate({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editProductAttributeTemplate, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};