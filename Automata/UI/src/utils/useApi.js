import { NotificationFunctions } from '@/functions/NotificationFunctions';
import { ref } from 'vue';

export const useApi = (getter, { silentError = false } = {}) => {
  let isLoading = ref(false);
  let GetData = async () => {
    let result = [false, null];
    isLoading.value = true;
    try {
      let data = await getter();
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
