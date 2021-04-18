import { query as queryUsers, queryCurrent, updateCurrent } from '@/services/user';
import {
  confirmEmail,
  getForgotPassword, forgotPasswordSendEmail, resetPasswordByEmail, forgotPasswordSendPhone, resetPasswordByPhone,
  removePhone, removeEmail,
  addPhoneGetCaptcha, addPhone,
  addEmailSendToken, addEmail,
  sendConfirmEmail,
  changePassword
} from '@/services/api';

export default {
  namespace: 'user',

  state: {
    list: [],
    currentUser: {},
  },

  effects: {
    *fetch(_, { call, put }) {
      const response = yield call(queryUsers);
      yield put({
        type: 'save',
        payload: response,
      });
    },

    *fetchCurrent(_, { call, put }) {
      const response = yield call(queryCurrent);
      yield put({
        type: 'saveCurrentUser',
        payload: response.data,
      });
    },

    *updateCurrent({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(updateCurrent, params);
      !!resolve && resolve(response);

      const res = yield call(queryCurrent);
      yield put({
        type: 'saveCurrentUser',
        payload: res.data,
      });
    },

    *confirmEmail({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(confirmEmail, params);
      !!resolve && resolve(response);
    },

    *getForgotPassword({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(getForgotPassword, params);
      !!resolve && resolve(response);
    },

    *forgotPasswordSendEmail({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(forgotPasswordSendEmail, params);
      !!resolve && resolve(response);
    },

    *resetPasswordByEmail({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(resetPasswordByEmail, params);
      !!resolve && resolve(response);
    },

    *forgotPasswordSendPhone({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(forgotPasswordSendPhone, params);
      !!resolve && resolve(response);
    },

    *resetPasswordByPhone({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(resetPasswordByPhone, params);
      !!resolve && resolve(response);
    },

    *removePhone({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(removePhone, params);
      !!resolve && resolve(response);

      const res = yield call(queryCurrent);
      yield put({
        type: 'saveCurrentUser',
        payload: res.data,
      });
    },

    *removeEmail({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(removeEmail, params);
      !!resolve && resolve(response);

      const res = yield call(queryCurrent);
      yield put({
        type: 'saveCurrentUser',
        payload: res.data,
      });
    },

    *addPhoneGetCaptcha({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(addPhoneGetCaptcha, params);
      !!resolve && resolve(response);
    },

    *addPhone({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(addPhone, params);
      !!resolve && resolve(response);

      const res = yield call(queryCurrent);
      yield put({
        type: 'saveCurrentUser',
        payload: res.data,
      });
    },

    *addEmailSendToken({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(addEmailSendToken, params);
      !!resolve && resolve(response);
    },

    *sendConfirmEmail({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(sendConfirmEmail, params);
      !!resolve && resolve(response);
    },

    *addEmail({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(addEmail, params);
      !!resolve && resolve(response);
    },

    *changePassword({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(changePassword, params);
      !!resolve && resolve(response);
    },
  },

  reducers: {
    save(state, action) {
      return {
        ...state,
        list: action.payload,
      };
    },
    saveCurrentUser(state, action) {
      return {
        ...state,
        currentUser: action.payload || {},
      };
    },
    changeNotifyCount(state, action) {
      return {
        ...state,
        currentUser: {
          ...state.currentUser,
          notifyCount: action.payload,
        },
      };
    },
  },
};
