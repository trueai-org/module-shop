import { registerByPhone, registerVerifyPhoneAndGetCaptcha } from '@/services/api';
import { setAuthority } from '@/utils/authority';
import { reloadAuthorized } from '@/utils/Authorized';
import { message } from 'antd';

export default {
  namespace: 'register',

  state: {
    status: undefined,
  },

  effects: {
    *submit({ payload }, { call, put }) {
      const res = yield call(registerByPhone, payload);
      if (res.success === true) {
        message.info('注册成功');

        yield put({
          type: 'registerHandle',
          payload: res,
        });
      } else {
        message.warning(res.message);
      }
    },

    *getCaptcha({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(registerVerifyPhoneAndGetCaptcha, params);
      !!resolve && resolve(response);
    },
  },

  reducers: {
    registerHandle(state, { payload }) {
      // 不自动登录处理
      // setAuthority('user');
      // reloadAuthorized();
      return {
        ...state,
        status: payload.success,
      };
    },
  },
};
