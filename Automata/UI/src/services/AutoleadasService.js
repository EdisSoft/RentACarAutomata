import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class AutoleadasService {
  async Leadas(rendszam) {
    // await timeout(500);
    // return {
    //   Id: 1,
    //   Nev: 'John Doe ' + 1,
    //   KezdDatum: new Date().toISOString(),
    //   VegeDatum: new Date().toISOString(),
    //   EmailFl: false,
    // };

    let params = { rendszam };
    let result = await httpContext.post(`Foglalas/Leadas`, null, {
      params,
    });
    return result.data;
  }
  async KulcsLeadas(id, taxiFl) {
    let params = { id, taxiFl };
    let result = await httpContext.post(`Foglalas/KulcsLeadas`, null, {
      params,
    });
    return result.data;
  }
  async UresRekeszNyitas(id) {
    await timeout(500);
    debugger;
    return {
      Id: 0,
      Text: '17',
    };
    let params = { id };
    let result = await httpContext.post(`Autoleadas/OpenLock`, null, {
      params,
    });
    return result.data;
  }
}

let instance = new AutoleadasService();
export { instance as AutoleadasService };
