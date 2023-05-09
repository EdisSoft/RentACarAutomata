import { NotificationFunctions } from '@/functions/NotificationFunctions';
import { ref } from 'vue';

export const useApi = (
  getter,
  { silentError = false, loadingKey = true } = {}
) => {
  let isLoading = ref(false);
  let GetData = async (d) => {
    let result = [false, null];
    isLoading.value = loadingKey;
    try {
      let data = await getter(d);
      result = [true, data];
    } catch (error) {
      if (silentError) {
        console.error(error);
      } else {
        NotificationFunctions.AjaxError(error);
      }
    }
    isLoading.value = false;
    return result;
  };

  return [isLoading, GetData];
};
