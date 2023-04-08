import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class AutoleadasService {
  async GetFoglalas(rendszam) {
    await timeout(500);
    return {
      Id: 1,
      Nev: 'John Doe ' + 1,
      KezdDatum: new Date().toISOString(),
      VegeDatum: new Date().toISOString(),
      EmailFl: false,
    };

    let result = await httpContext.post(`Autoleadas/GetFoglalas`);
    return result.data;
  }
  async TaxiRendeles(id) {
    await timeout(500);
    return {
      Id: 0,
      Text: 'Ok',
    };

    let params = { id };
    let result = await httpContext.post(`Autoleadas/TaxiRendeles`, null, {
      params,
    });
    return result.data;
  }
  async UresRekeszNyitas(id) {
    await timeout(500);
    return {
      Id: 0,
      Text: '17',
    };
    let params = { id };
    let result = await httpContext.post(`Autoleadas/UresRekeszNyitas`, null, {
      params,
    });
    return result.data;
  }
}

let instance = new AutoleadasService();
export { instance as AutoleadasService };
