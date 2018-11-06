import request from '@/utils/request';
import config from './config';

const {apiUrl} = config;

export async function fetchAlerts() {
  return request(`${apiUrl}/alerts`);
}


