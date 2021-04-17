import {
    queryProductOption,
    queryBrandAll,
    queryProductAttributeTemplates,
    queryProductAttr,
    queryProductAttributeGroupArray,
    firstProductAttributeTemplate,
    queryProductAttrData,
    queryProductOptionData,
    categories,
    queryUnitAll,
    queryProductAGS,
    queryProductGrid
} from '@/services/api';

export default {
    namespace: 'catalog',
    state: {

    },

    effects: {
        *templates({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(queryProductAttributeTemplates);
            !!resolve && resolve(response);
        },

        *templateFirst({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(firstProductAttributeTemplate, params);
            !!resolve && resolve(response);
        },

        *attributes({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(queryProductAttr);
            !!resolve && resolve(response);
        },

        *attributeData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductAttrData, params);
            !!resolve && resolve(response);
        },

        *attributesGroupArray({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(queryProductAttributeGroupArray);
            !!resolve && resolve(response);
        },

        *options({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(queryProductOption);
            !!resolve && resolve(response);
        },

        *optionData({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductOptionData, params);
            !!resolve && resolve(response);
        },

        *brands({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryBrandAll, params);
            !!resolve && resolve(response);
        },

        *categories({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(categories, params);
            !!resolve && resolve(response);
        },

        *units({ payload }, { call, put }) {
            const { resolve } = payload;
            const response = yield call(queryUnitAll);
            !!resolve && resolve(response);
        },

        *attrGroups({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductAGS, params);
            !!resolve && resolve(response);
        },

        *products({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryProductGrid, params);
            !!resolve && resolve(response);
        },
    },

    reducers: {

    },
};