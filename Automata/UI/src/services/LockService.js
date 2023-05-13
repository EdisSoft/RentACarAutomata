import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class LockService {
  async OpenLock(lockNo) {
    await timeout(500);
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { lockNo };
    let result = await httpContext.post(`Lock/OpenLock`, null, {
      params,
    });
    return result.data;
  }
  async LockStatuses() {
    let r = [];
    for (let i = 0; i < 9; i++) {
      r.push({ RekeszId: i + 1, IsOpen: i % 2 == 0 });
    }
    await timeout(500);
    console.log('LockStatuses');
    return r;
    let result = await httpContext.get(`Lock/LockStatuses`);
    return result.data;
  }
}

let instance = new LockService();
export { instance as LockService };
