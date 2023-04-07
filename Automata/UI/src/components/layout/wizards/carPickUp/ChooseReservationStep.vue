<template>
  <div class="wizard-step">
    <v-sheet class="wizard-content">
      <wizard-select-input v-model="selectedItem" :options="reservations">
        <template #title>
          {{ $t('wizards.carPickUp.chooseReservation.title') }}
        </template>
      </wizard-select-input>
    </v-sheet>
    <wizard-footer @next="Next()" :nextBtnDisabled="nextBtnDisabled">
    </wizard-footer>
  </div>
</template>

<script>
import { CarPickupWizard } from '@/enums/CarPickupWizard';
import { DateFunctions } from '@/functions/DateFunctions';
import { WizardFunctions } from '@/functions/WizardFunctions';

export default {
  name: 'choose-reservation-step',
  inject: ['wizard', 'wizardForm'],
  data() {
    return {
      selectedItem: null,
      CarPickupWizard,
    };
  },
  created() {},
  methods: {
    Next() {
      let reservation = this.wizardForm.Reservations.find(
        (f) => f.Id == this.selectedItem?.id
      );
      if (!reservation) {
        return;
      }
      WizardFunctions.HandleNavigationForReservation(this.wizard, reservation);
    },
  },
  computed: {
    nextBtnDisabled() {
      return !this.selectedItem;
    },
    reservations() {
      let form = this.wizardForm;
      let reservations = form.Reservations || [];
      return reservations.map((m) => {
        return {
          id: m.Id,
          title: m.Nev,
          text:
            DateFunctions.ToShortDate(new Date(m.KezdDatum)) +
            ' - ' +
            DateFunctions.ToShortDate(new Date(m.VegeDatum)),
        };
      });
    },
  },
  watch: {},
  components: {},
};
</script>
<style scoped></style>
