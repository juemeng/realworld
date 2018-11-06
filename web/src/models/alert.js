import { fetchAlerts } from '@/services/alert';

export default {
  namespace: 'alert',

  state: {
    alerts: [],
  },

  effects: {
    *fetch({ payload }, { call, put }) {
      const response = yield call(fetchAlerts);
      yield put({
        type: 'fetchAlertSuccess',
        payload: Array.isArray(response) ? response : [],
      });
    },
  },

  reducers: {
    fetchAlertSuccess(state, action) {
      return {
        ...state,
        alerts: action.payload,
      };
    },
    addAlert(state, action) {
      return {
        ...state,
        alerts: state.alerts.concat([action.payload]),
      };
    },
  },
};
