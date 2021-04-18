import { routerRedux } from 'dva/router';
import { stringify } from 'qs';
import { fakeAccountLogin, loginAdmin, loginPhone, loginPhoneGetCaptcha, loginVerifyTwoFactor, loginTwoFactor, logoutAdmin } from '@/services/api';
import { setAuthority } from '@/utils/authority';
import { getPageQuery } from '@/utils/utils';
import { reloadAuthorized } from '@/utils/Authorized';
import { message } from 'antd';
import token from '../utils/token';

export default {
  namespace: 'login',

  state: {
    status: undefined,
    requiresTwoFactor: undefined,
    providers: []
  },

  effects: {
    *loginAdmin({ payload }, { call, put }) {
      // console.log(payload);
      let response = {};
      if (payload.type === 'account') {
        response = yield call(loginAdmin, payload);
      } else {
        response = yield call(loginPhone, payload);
      }

      if (!response) {
        return;
      }
      if (response.success === true) {
        response.status = 'ok';
        response.type = 'account';
        response.currentAuthority = 'admin'; //response.data.name;//'admin';//
        yield put({
          type: 'changeLoginStatus',
          payload: response,
        });
      } else {
        yield put({
          type: 'changeFactor',
          payload: response.data,
        });
        message.warning(response.message)
      }

      // Login successfully
      if (response.success === true) {
        reloadAuthorized();
        token.save(response.data.token);

        const urlParams = new URL(window.location.href);
        const params = getPageQuery();
        let { redirect } = params;
        if (redirect) {
          const redirectUrlParams = new URL(redirect);
          if (redirectUrlParams.origin === urlParams.origin) {
            redirect = redirect.substr(urlParams.origin.length);
            if (redirect.startsWith('/#')) {
              redirect = redirect.substr(2);
            }
          } else {
            window.location.href = redirect;
            return;
          }
        }
        yield put(routerRedux.replace(redirect || '/'));
      }
    },

    *login({ payload }, { call, put }) {
      const response = yield call(fakeAccountLogin, payload);
      yield put({
        type: 'changeLoginStatus',
        payload: response,
      });
      // Login successfully
      if (response.status === 'ok') {
        reloadAuthorized();
        const urlParams = new URL(window.location.href);
        const params = getPageQuery();
        let { redirect } = params;
        if (redirect) {
          const redirectUrlParams = new URL(redirect);
          if (redirectUrlParams.origin === urlParams.origin) {
            redirect = redirect.substr(urlParams.origin.length);
            if (redirect.startsWith('/#')) {
              redirect = redirect.substr(2);
            }
          } else {
            window.location.href = redirect;
            return;
          }
        }
        yield put(routerRedux.replace(redirect || '/'));
      }
    },

    // *getCaptcha({ payload }, { call, put }) {
    //   const { resolve, params } = payload;
    //   const response = yield call(loginPhoneGetCaptcha, params);
    //   return response;
    // },

    *getCaptcha({ payload }, { call, put }) {
      const res = yield call(loginPhoneGetCaptcha, payload);
      if (res.success === true) {
        message.info('发送成功');
      } else {
        message.warning(res.message);
      }
    },

    *getCaptchaTwoFactor({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(loginVerifyTwoFactor, params);
      !!resolve && resolve(response);
    },

    *loginTwoFactor({ payload }, { call, put }) {
      const { resolve, params } = payload;
      const response = yield call(loginTwoFactor, params);

      if (response.success === true) {
        response.status = 'ok';
        response.type = 'account';
        response.currentAuthority = 'admin'; //response.data.name;//'admin';//
        yield put({
          type: 'changeLoginStatus',
          payload: response,
        });
      } else {
        // yield put({
        //   type: 'changeFactor',
        //   payload: response.data,
        // });
        message.warning(response.message)
      }

      // Login successfully
      if (response.success === true) {
        reloadAuthorized();
        token.save(response.data.token);

        const urlParams = new URL(window.location.href);
        const params = getPageQuery();
        let { redirect } = params;
        if (redirect) {
          const redirectUrlParams = new URL(redirect);
          if (redirectUrlParams.origin === urlParams.origin) {
            redirect = redirect.substr(urlParams.origin.length);
            if (redirect.startsWith('/#')) {
              redirect = redirect.substr(2);
            }
          } else {
            window.location.href = redirect;
            return;
          }
        }
        yield put(routerRedux.replace(redirect || '/'));
      }
    },

    *factor({ payload }, { call, put }) {
      yield put({
        type: 'changeFactor',
        payload: {
          requiresTwoFactor: payload,
          providers: [],
        },
      });
    },

    *logout(_, { call, put }) {
      const response = yield call(logoutAdmin);
      if (response.success === true) {
        yield put({
          type: 'changeLoginStatus',
          payload: {
            status: false,
            currentAuthority: 'guest',
          },
        });
        reloadAuthorized();
        yield put(
          routerRedux.push({
            pathname: '/user/login',
            search: stringify({
              redirect: window.location.href,
            }),
          })
        );
      } else {
        message.error(response.message);
      }
    },
  },

  reducers: {
    changeLoginStatus(state, { payload }) {
      setAuthority(payload.currentAuthority);
      return {
        ...state,
        status: payload.status,
        type: payload.type,
      };
    },

    changeFactor(state, { payload }) {
      let requiresTwoFactor = undefined;
      let providers = [];
      if (payload && payload.requiresTwoFactor === true) {
        requiresTwoFactor = payload.requiresTwoFactor
        providers = payload.providers;
      }

      return {
        ...state,
        requiresTwoFactor: requiresTwoFactor,
        providers: providers
      };
    },
  },
};
