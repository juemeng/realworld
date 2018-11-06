import { queryNotices } from '@/services/user';

export default {
  namespace: 'project',

  state: {
    notices: [],
  },

  effects: {
    *fetchNotice(_, { call, put }) {
      const response = yield call(queryNotices);
      yield put({
        type: 'saveNotice',
        payload: Array.isArray(response) ? response : [],
      });
    },
  },

  reducers: {
    saveNotice(state, action) {
      return {
        ...state,
        notices: action.payload,
      };
    },
  },
};
