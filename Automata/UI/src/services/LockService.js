import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class LockService {
  async OpenLock(rekeszId) {
    // await timeout(500);
    // return {
    //   Id: 0,
    //   Text: 'Ok',
    // };
    let params = { rekeszId };
    let result = await httpContext.post(`Lock/OpenCompartment`, null, {
      params,
    });
    return result.data;
  }
  async LockStatuses() {
    //let r = [];
    //for (let i = 0; i < 9; i++) {
    //  r.push({ RekeszId: i + 1, IsOpen: i % 2 == 0 });
    //}
    //await timeout(500);
    //console.log('LockStatuses');
    //return r;
    let result = await httpContext.post(`Lock/CompartmentStatuses`);
    return result.data;
  }
  async OpenLockByBookingId(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: '5',
    };
    let params = { id };
    let result = await httpContext.post(`Lock/OpenLockByBookingId`, null, {
      params,
    });
    return result.data;
  }
}

let instance = new LockService();
export { instance as LockService };
