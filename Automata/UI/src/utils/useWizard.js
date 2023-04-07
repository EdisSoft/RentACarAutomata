import { set } from 'lodash';
import { reactive, ref } from 'vue';

export const useWizard = (initStep) => {
  let step = ref(initStep);
  let form = reactive({});

  const Goto = (s) => {
    step.value = s;
  };
  const SetFormValue = (path, value) => {
    set(form, path, value);
  };

  return { form, step, Goto, SetFormValue };
};
