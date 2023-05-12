import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class PosService {
  async LetetZarolas(id) {
    // console.log('PosService.LetetZarolas');
    // await timeout(500);
    // return {
    //   Id: 0,
    //   Text: 'Ok',
    // };
    let params = { id };
    let result = await httpContext.post(`Pos/LetetZarolas`, null, {
      params,
    });
    return result.data;
  }
  async Fizetes(id) {
    // console.log('PosService.Fizetes');
    // await timeout(500);
    // return {
    //   Id: 0,
    //   Text: 'Ok',
    // };
    let params = { id };
    let result = await httpContext.post(`Pos/Fizetes`, null, {
      params,
    });
    return result.data;
  }
}

let instance = new PosService();
export { instance as PosService };
