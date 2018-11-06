import { fetchBindings,bindDevice } from '@/services/moniter';

export default {
  namespace: 'moniter',

  state: {
    bindings: [],
    finished:false
  },

  effects: {
    *fetch({ payload }, { call, put }) {
      const response = yield call(fetchBindings);
      yield put({
        type: 'fetchBindingsSuccess',
        payload: Array.isArray(response) ? response : [],
      });
    },

    *bind({ payload }, { call, put }) {
      const response = yield call(bindDevice,payload);
      yield put({
        type: 'bindDeviceSuccess',
        payload: {...response,floorId:payload.deployInfo.floorId},
      });
    },
  },

  reducers: {
    fetchBindingsSuccess(state, action) {
      return {
        ...state,
        bindings: action.payload,
      };
    },
    bindDeviceSuccess(state, action) {
      let newBindings = _.clone(state.bindings);
      let matched = _.find(newBindings,x=>x.floorId == action.payload.floorId);
      matched.masterId = action.payload.masterId;
      return {
        ...state,
        bindings: newBindings,
        finished:true
      };
    }
  },
};
