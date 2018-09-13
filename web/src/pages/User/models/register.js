import { fakeRegister } from '@/services/api';
import { userRegister} from '@/services/user';
import { setAuthority } from '@/utils/authority';
import { reloadAuthorized } from '@/utils/Authorized';

export default {
  namespace: 'register',

  state: {},

  effects: {
    *submit({ payload }, { call, put }) {
      const response = yield call(userRegister, {user:payload});
      console.log(response);
      yield put({
        type: 'registerHandle',
        payload: response,
      });
    },
  },

  reducers: {
    registerHandle(state, { payload }) {
      setAuthority('user');
      reloadAuthorized();
      return {
        ...state,
        user:payload.user
      };
    },
  },
};
