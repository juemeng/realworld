import { fakeRegister } from '@/services/api';
import { userRegister} from '@/services/user';

export default {
  namespace: 'register',

  state: {},

  effects: {
    *submit({ payload }, { call, put }) {
      const response = yield call(userRegister, {user:payload});
      yield put({
        type: 'registerHandle',
        payload: response,
      });
    },
  },

  reducers: {
    registerHandle(state, { payload }) {
      return {
        ...state,
        user:payload.user
      };
    },
  },
};
