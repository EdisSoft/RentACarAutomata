import { LanguageList } from '@/data/languages';
import { CarPickupWizard } from '@/enums/CarPickupWizard';

class WizardFunctions {
  HandleNavigationForReservation(wizard, reservation) {
    wizard.SetFormValue('Reservation', reservation);
    let step = reservation.UtolsoVarazsloLepes;
    if (step) {
      this.HandleNavigationForPayment(wizard, reservation, step);
    } else if (reservation.EmailFl) {
      wizard.Goto(CarPickupWizard.SignStep);
    } else {
      wizard.Goto(CarPickupWizard.EmailAdressStep);
    }
  }
  HandleNavigationForPayment(wizard, reservation, toStep) {
    if (toStep == CarPickupWizard.PayDepositStep) {
      if (reservation.Zarolando > 0) {
        wizard.Goto(CarPickupWizard.PayDepositStep);
      } else {
        this.HandleNavigationForPayment(
          wizard,
          reservation,
          CarPickupWizard.PayRentalFeeStep
        );
      }
    } else if (toStep == CarPickupWizard.PayRentalFeeStep) {
      if (reservation.Fizetendo > 0) {
        wizard.Goto(CarPickupWizard.PayRentalFeeStep);
      } else {
        wizard.Goto(CarPickupWizard.FinalStep);
      }
    } else {
      wizard.Goto(toStep);
    }
  }
}

let instance = new WizardFunctions();
export { instance as WizardFunctions };
