import request from '@/utils/request';

export async function query() {
  return request('/api/users');
}

export async function queryCurrent() {
  return request('/api/account');
}

export async function updateCurrent(params) {
  return request(`/api/account`, {
    method: 'PUT',
    body: params,
  });
}
