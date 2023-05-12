import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class PosService {
  async LetetZarolas() {
    // console.log('PosService.LetetZarolas');
    // await timeout(500);
    // return {
    //   Id: 0,
    //   Text: 'Ok',
    // };
    let result = await httpContext.post(`Pos/LetetZarolas`);
    return result.data;
  }
  async Fizetes() {
    // console.log('PosService.Fizetes');
    // await timeout(500);
    // return {
    //   Id: 0,
    //   Text: 'Ok',
    // };
    let result = await httpContext.post(`Pos/Fizetes`);
    return result.data;
  }
}

let instance = new PosService();
export { instance as PosService };
