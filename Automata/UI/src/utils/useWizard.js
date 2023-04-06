import { ref } from 'vue';

export const useWizard = (initStep) => {
  let step = ref(initStep);

  const Goto = (s) => {
    step.value = s;
  };

  return { step, Goto };
};
