// use localStorage to store the authority info, which might be sent from server in actual project.
export function getAuthority() {
  // return localStorage.getItem('antd-pro-authority') || ['admin', 'user'];
  let authority = localStorage.getItem('user');
  if (authority) {
    if (authority.includes('[')) {
      authority = JSON.parse(authority);
    } else {
      authority = [JSON.parse(authority)];
    }
  } else {
    authority = ['guest'];
  }
  return authority;
}

export function setAuthority(authority) {
  return localStorage.setItem('user', JSON.stringify(authority));
}
