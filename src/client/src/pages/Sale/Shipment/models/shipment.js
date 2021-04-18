import {
    shipmentGrid
} from '@/services/api';

export default {
    namespace: 'shipment',
    state: {
    },
    effects: {
        *grid({ payload }, { call, put }) {
            const { resolve, params } = payload;
            const response = yield call(shipmentGrid, params);
            !!resolve && resolve(response);
        },


    },
    reducers: {
    },
};