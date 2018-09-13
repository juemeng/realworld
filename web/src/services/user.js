import request from '@/utils/request';
import config from './config';

const {apiUrl} = config;

export async function userRegister(params) {
  return request(`${apiUrl}/account`, {
    method: 'POST',
    body: params,
  })
}

export async function query() {
  return request('/api/users');
}

export async function queryCurrent() {
  return request('/api/currentUser');
}
