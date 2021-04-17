import {
    queryPriceAndDestinationGrid, addPriceAndDestination, editPriceAndDestination, deletePriceAndDestination
} from '@/services/api';

export default {
    namespace: 'price-destination',
    state: {
    },
    effects: {
        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(queryPriceAndDestinationGrid, params);
            !!resolve && resolve(response);
        },

        *delete({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(deletePriceAndDestination, params);
            !!resolve && resolve(response);
        },

        *add({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(addPriceAndDestination, params);
            !!resolve && resolve(response);
        },

        *edit({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(editPriceAndDestination, params);
            !!resolve && resolve(response);
        },
    },
    reducers: {
    },
};