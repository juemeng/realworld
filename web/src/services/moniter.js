import request from '@/utils/request';
import config from './config';

const {apiUrl} = config;

export async function fetchBindings() {
  return request(`${apiUrl}/deploy/bindings`);
}

export async function bindDevice(params) {
  return request(`${apiUrl}/deploy`, {
    method: 'POST',
    body: params,
  });
}


