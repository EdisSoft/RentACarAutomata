import { LanguageList } from '@/data/languages';
import { CarPickupWizard } from '@/enums/CarPickupWizard';

class WizardFunctions {
  HandleNavigationForReservation(wizard, reservation) {
    wizard.SetFormValue('Reservation', reservation);
    let step = reservation.UtolsoVarazsloLepes;
    if (step) {
      wizard.Goto(step);
    } else if (reservation.EmailFl) {
      wizard.Goto(CarPickupWizard.SignStep);
    } else {
      wizard.Goto(CarPickupWizard.EmailAdressStep);
    }
  }
}

let instance = new WizardFunctions();
export { instance as WizardFunctions };
