const isProd =
  process.env.NODE_ENV === 'production' &&
  !location.host.startsWith('localhost:808');
debugger;
let baseUrl = isProd
  ? location.pathname.replace(/(\/$)/gi, '/api')
  : location.protocol + '//' + location.hostname + ':' + 44338 + '/api'; //61443

export const settings = {
  isProd: isProd,
  baseUrl: baseUrl,
};
