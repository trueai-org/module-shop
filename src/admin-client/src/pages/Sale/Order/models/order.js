import {
  queryOrderGrid,
  addOrder,
  editOrder,
  deleteOrder,
  firstOrder,
  queryOrderHistory,
  cancelOrder,
  onHoldOrder,
  paymentOrder,
  firstOrderByNo,
  orderShipment,
} from '@/services/api';

export default {
  namespace: 'order',
  state: {},
  effects: {
    *get({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(firstOrder, params);
      !!resolve && resolve(response);
    },

    *grid({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(queryOrderGrid, params);
      !!resolve && resolve(response);
    },

    *add({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(addOrder, params);
      !!resolve && resolve(response);
    },

    *edit({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(editOrder, params);
      !!resolve && resolve(response);
    },

    *delete({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(deleteOrder, params);
      !!resolve && resolve(response);
    },

    *cancel({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(cancelOrder, params);
      !!resolve && resolve(response);
    },

    *onHold({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(onHoldOrder, params);
      !!resolve && resolve(response);
    },

    *payment({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(paymentOrder, params);
      !!resolve && resolve(response);
    },

    *histories({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(queryOrderHistory, params);
      !!resolve && resolve(response);
    },

    *getByNo({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(firstOrderByNo, params);
      !!resolve && resolve(response);
    },

    *shipment({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(orderShipment, params);
      !!resolve && resolve(response);
    },
  },
  reducers: {},
};
