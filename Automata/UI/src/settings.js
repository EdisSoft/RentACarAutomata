const isProd =
  process.env.NODE_ENV === 'production' &&
  !location.host.startsWith('localhost:808');
let baseUrl = isProd
  ? location.pathname.replace(/(\/$)/gi, '/api')
  : location.protocol + '//' + location.hostname + ':' + 56130 + '/api';

export const settings = {
  appName: 'game-automata',
  isProd: isProd,
  baseUrl: baseUrl,
};
