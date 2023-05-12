import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class QrCodeService {
  async Start() {
    // console.log('QrCodeService.Start');
    // await timeout(500);
    // return {
    //   Id: 0,
    //   Text: 'Ok',
    // };
    let result = await httpContext.post(`QrCode/Start`);
    return result.data;
  }
}

let instance = new QrCodeService();
export { instance as QrCodeService };
