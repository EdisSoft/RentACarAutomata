import addMinutes from 'date-fns/addMinutes';
import getUnixTime from 'date-fns/getUnixTime';

var tokenName = 'AmAuthToken';

export function setToken(token) {
  if (token) {
    localStorage.setItem(tokenName, token);
  } else {
    deleteToken();
  }
}

export function getToken() {
  return localStorage.getItem(tokenName);
}

export function deleteToken() {
  localStorage.removeItem(tokenName);
}

export function isTokenValid() {
  var token = getToken();
  if (!token) return false;
  var tokenData = parseJwt(token);
  return getUnixTime(new Date()) < tokenData.exp;
}
export function shouldUpdateToken() {
  var token = getToken();
  if (!token) return false;
  var tokenData = parseJwt(token);
  let valid = getUnixTime(new Date()) < tokenData.exp;
  let willExpire = getUnixTime(addMinutes(new Date(), 30)) > tokenData.exp;
  return valid && willExpire;
}
export function parseJwt(token) {
  var base64Url = token.split('.')[1];
  var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  var jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join('')
  );

  return JSON.parse(jsonPayload);
}
