import axios from 'axios';
import { getToken, isTokenValid } from '@/utils/token';
import { i18n } from '@/plugins/i18n';
import { settings } from '@/settings';

export const httpContext = axios.create({
  baseURL: settings.baseUrl,
  timeout: 160000,
});
httpContext.interceptors.request.use(function (config) {
  config.headers['Accept-Language'] = i18n.locale;
  return config;
});
export const setAutTokenInHeader = function (token) {
  httpContext.defaults.headers['Authorization'] = 'Bearer ' + token;
};
let token = getToken();
if (token && isTokenValid()) {
  setAutTokenInHeader(token);
}
