using FunctionsCore.Models;
using System;

namespace Automata.Functions
{
    public interface IPrinterFunctions
    {
        bool PrintReceiptHun(string agreementNumber, string plateNumber, DateTime endOfRental, int money, string preAuthorizationNumber);
        bool PrintReceiptEng(string agreementNumber, string plateNumber, DateTime endOfRental, int money, string preAuthorizationNumber);
        public bool PrintOtpResult(MoneraReceiptModel receipt);

    }
}
