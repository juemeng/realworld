import { routerRedux } from 'dva/router';
import { stringify } from 'qs';
import { fakeAccountLogin, getFakeCaptcha } from '@/services/api';
import { userLogin } from '@/services/user';
import { setAuthority } from '@/utils/authority';
import { getPageQuery } from '@/utils/utils';
import { reloadAuthorized } from '@/utils/Authorized';

const roles = ['admin', 'user'];

export default {
  namespace: 'login',

  state: {
    status: undefined,
    type: 'account',
    errorMessage: '',
  },

  effects: {
    * login({ payload }, { call, put }) {
      const response = yield call(userLogin, { user: payload });
      if (response.error) {
        let data = yield response.response.json();
        let errorContent = Object.keys(data.errors).map(x => {
          return data.errors[x];
        });
        let message = errorContent.join('\n');
        yield put({
          type: 'changeLoginStatus',
          payload: { error: true, message: message },
        });
      } else {
        localStorage.setItem('token', response.token);
        yield put({
          type: 'user/saveCurrentUser',
          payload: response,
        });

        setAuthority(roles[response.role]);
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

    * getCaptcha({ payload }, { call }) {
      yield call(getFakeCaptcha, payload);
    },

    * logout(_, { put }) {
      // yield put({
      //   type: 'changeLoginStatus',
      //   payload: {
      //     status: false,
      //     currentAuthority: 'guest',
      //   },
      // });
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      reloadAuthorized();
      yield put(
        routerRedux.push({
          pathname: '/user/login',
          search: stringify({
            redirect: window.location.href,
          }),
        }),
      );
    },
  },

  reducers: {
    changeLoginStatus(state, { payload }) {
      return {
        ...state,
        status: 'error',
        errorMessage: payload.message,
      };
    },
  },
};
