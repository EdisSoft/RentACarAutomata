const isProd =
  process.env.NODE_ENV === 'production' &&
  !location.host.startsWith('localhost:808');

let baseUrl = isProd
  ? location.pathname.replace(/(\/$)/gi, '/api')
  : location.protocol + '//' + location.hostname + ':' + 61443 + '/api';

export const settings = {
  isProd: isProd,
  baseUrl: baseUrl,
};
