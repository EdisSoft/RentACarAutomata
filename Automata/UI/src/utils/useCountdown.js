import { DateFunctions } from '@/functions/DateFunctions';
import { useIdle, useTimeoutPoll } from '@vueuse/core';
import { ref, watch } from 'vue';

export const useCountdown = (maxIdleTime, action) => {
  let ct = ref(0);
  let remaingTime = ref('');
  const { idle, lastActive } = useIdle(maxIdleTime);
  useTimeoutPoll(
    () => {
      ct.value++;
    },
    1000,
    { immediate: true }
  );
  watch(idle, () => {
    if (idle && action) {
      action();
    }
  });
  watch(
    ct,
    () => {
      let lastActiveDate = new Date(lastActive.value);
      let deadline = DateFunctions.Add(lastActiveDate, {
        seconds: maxIdleTime / 1000,
      });
      let diff = DateFunctions.GetTimeDifference(deadline, new Date());
      if (diff) {
        let minutes = diff.minutes.toString().padStart(2, '0');
        let seconds = diff.seconds.toString().padStart(2, '0');
        remaingTime.value = `${minutes}:${seconds}`;
      }
    },
    { immediate: true }
  );
  return { remaingTime };
};
