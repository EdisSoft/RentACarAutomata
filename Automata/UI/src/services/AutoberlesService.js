import { timeout } from '@/utils/common';
import { httpContext } from '../utils/httpContext';
import settings from '../settings';

class AutoberlesService {
  async GetAlapAdatok() {
    let result = await httpContext.post(`Home/GetData`);
    return result.data;
  }
  async GetFoglalasok(nev) {
    await timeout(500);
    // let mock = [];
    // for (let i = 1; i < 11; i++) {
    //   mock.push({
    //     Id: i,
    //     Nev: 'John Doe ' + i,
    //     KezdDatum: new Date().toISOString(),
    //     VegeDatum: new Date().toISOString(),
    //     EmailFl: i % 2 == 0 ? true : false,
    //   });
    // }
    // return mock;
    let params = { nev };
    let result = await httpContext.post(`Home/GetData`, null, { params });

    // let result = await httpContext.post(`Autoberles/GetFoglalasok`);
    return result.data;
  }
  async IsQrCode() {
    await timeout(500);
    // return {
    //   Id: 1,
    //   Nev: 'John Doe ' + 1,
    //   KezdDatum: new Date().toISOString(),
    //   VegeDatum: new Date().toISOString(),
    //   EmailFl: false,
    // };
    let result = await httpContext.post(`QrCode/ReadQr`);
    return result.data;
  }
  async SaveEmail(id, email) {
    await timeout(500);
    console.log({ id, email });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id, email };
    let result = await httpContext.post(`Autoberles/SaveEmail`, null, {
      params,
    });
    return result.data;
  }
  async SaveAlairas(id, pic) {
    await timeout(500);
    console.log({ id, pic });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id, pic };
    let result = await httpContext.post(`Autoberles/SaveAlairas`, null, {
      params,
    });
    return result.data;
  }
  async ScanLicenceFront(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(`Autoberles/ScanLicenceFront`, null, {
      params,
    });
    return result.data;
  }
  async ScanLicenceBack(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(`Autoberles/ScanLicenceBack`, null, {
      params,
    });
    return result.data;
  }
  async ScanIdCardFrontOrPassport(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(
      `Autoberles/ScanIdCardFrontOrPassport`,
      null,
      {
        params,
      }
    );
    return result.data;
  }
  async ScanIdCardBack(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(`Autoberles/ScanIdCardBack`, null, {
      params,
    });
    return result.data;
  }
  async ScanCreditCardFront(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(
      `Autoberles/ScanCreditCardFront`,
      null,
      {
        params,
      }
    );
    return result.data;
  }
  async ScanCreditCardBack(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(`Autoberles/ScanCreditCardBack`, null, {
      params,
    });
    return result.data;
  }
  async LetetZarolas(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(
      `Autoberles/LetetZarolasEllenorzes`,
      null,
      {
        params,
      }
    );
    return result.data;
  }
  async BerletiDijFizetes(id) {
    await timeout(500);
    console.log({ id });
    return {
      Id: 0,
      Text: 'Ok',
    };
    let params = { id };
    let result = await httpContext.post(
      `Autoberles/BerletiDijFizetesEllenorzes`,
      null,
      {
        params,
      }
    );
    return result.data;
  }
}

let instance = new AutoberlesService();
export { instance as AutoberlesService };
