using FunctionsCore;

namespace Automata.Functions
{
    public class MoneraReceipt
    {
        public string TerminalID = "";
        public string AuthCode = "";
        public string RetNum = "";
        public string RetText = "";
        public string Amount = "";
        public string DateTime = "";
        public string CardNum = "";
        public string RefNo = "";
        public string ACQ = "";
        public string CardType = "";
        public string Location = "";
        public string AID = "";
        public string TC = "";
        public string TranID = "";
        public string Merchant = "";
        public string Owner = "";

        public override string ToString()
        {
            return base.ToString() + ": " + "TerminalID=" + TerminalID + ", AuthCode=" + AuthCode +
                ", RetNum=" + RetNum + ", RetText=" + RetText +
                ", Amount=" + Amount + ", DateTime=" + DateTime +
                ", CardNum=" + CardNum + ", RefNo=" + RefNo +
                ", ACQ=" + ACQ + ", CardType=" + CardType +
                ", Location=" + Location + ", AID=" + AID +
                ", TC=" + TC + ", TranID=" + TranID +
                ", Merchant=" + Merchant + ", Owner=" + Owner;
        }

        public int Parse(string rawtext)
        {
            // {TID=02439406|ATH=785064 B|RETNUM=001|RETTXT=ELFOGADVA|AMT=100000,00|DATE=2023.03.29 21:56:03|CNB=525642XXXXXX7765|
            // REFNO=622|ACQ= |CTYP=Mastercard|LOC=MONERA TESZT TERMINA'L|MERCN=U:ZLET NEVE|OWN=TESZT  TESZT TESZT|
            // AID=A0000000041010|TC=53109BC9DB209170|TRID=CTID_28828}
            //
            string key;
            string value;
            int elements = 0;

            rawtext.Trim();
            if (rawtext.StartsWith('{'))
            {
                rawtext = rawtext.Substring(1);
            }
            if (rawtext.EndsWith('}'))
            {
                rawtext = rawtext.Substring(0, rawtext.Length - 1);
            }
            string[] subs = rawtext.Split('|');
            foreach (string s in subs)
            {
                int pos = s.IndexOf('=');
                if (pos == -1)
                {
                    return -1;
                }
                key = s.Substring(0, pos);
                value = s.Substring(pos + 1);
                switch (key.ToUpper())
                {
                    case "TID":
                        TerminalID = value;
                        elements++;
                        break;
                    case "ATH":
                        AuthCode = value;
                        elements++;
                        break;
                    case "RETNUM":
                        RetNum = value;
                        elements++;
                        break;
                    case "RETTXT":
                        RetText = value;
                        elements++;
                        break;
                    case "AMT":
                        Amount = value;
                        elements++;
                        break;
                    case "DATE":
                        DateTime = value;
                        elements++;
                        break;
                    case "CNB":
                        CardNum = value;
                        elements++;
                        break;
                    case "REFNO":
                        RefNo = value;
                        elements++;
                        break;
                    case "ACQ":
                        ACQ = value;
                        elements++;
                        break;
                    case "CTYP":
                        CardType = value;
                        elements++;
                        break;
                    case "LOC":
                        Location = value;
                        elements++;
                        break;
                    case "MERCN":
                        Merchant = value;
                        elements++;
                        break;
                    case "OWN":
                        Owner = value;
                        elements++;
                        break;
                    case "AID":
                        AID = value;
                        elements++;
                        break;
                    case "TC":
                        TC = value;
                        elements++;
                        break;
                    case "TRID":
                        TranID = value;
                        elements++;
                        break;
                    default:
                        Log.Debug("Unknown receipt element: " + s);
                        break;
                }
            }
            // number of found elements
            return elements;
        }
    }
}
