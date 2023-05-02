import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';

class AutomataService {
  async RekeszNyitas(rekeszSzama) {
    console.log(rekeszSzama);
    await timeout(500);
    return {
      Id: 0,
      Text: 'Ok',
    };
    let data = { rekeszSzama };
    let result = await httpContext.post(`Automata/RekeszNyitas`, data);
    return result.data;
  }
}

let instance = new AutomataService();
export { instance as AutomataService };
