using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionsCore.Utilities.Extension
{
    public static class CurrencyExtensions
    {
        #region SzamValtasaBeture
        private static string[] aOne = { "", "egy", "kettő", "három", "négy", "öt", "hat", "hét", "nyolc", "kilenc" };
        private static string[] aTwo = { "", "tíz", "húsz", "harminc", "negyven", "ötven", "hatvan", "hetven", "nyolcvan", "kilencven" };
        private static int[] ai = new int[3];

        private static string S1()
        {
            return aOne[ai[0]];
        }

        private static string S2()
        {
            if (ai[0] == 0)
            {
                return aTwo[ai[1]] + S1();
            }
            else
            {
                switch (ai[1])
                {
                    case 1:
                        return "tizen" + S1();
                    case 2:
                        return "huszon" + S1();
                    default:
                        return aTwo[ai[1]] + S1();
                }
            }
        }

        private static string S3()
        {
            if (ai[2] == 0)
            {
                return S2();
            }
            else
            {
                return aOne[ai[2]] + "száz" + S2();
            }
        }

        public static string SzamValtasaBeture(this int CurrencyValue)
        {
            if (CurrencyValue == 0)
            {
                return "nulla";
            }
            else
            {
                string[] szorzo = { "", "ezer", "millió", "milliárd" };
                var ezresek = new List<string>();
                bool minusz = false;
                if (CurrencyValue < 0)
                {
                    CurrencyValue = Math.Abs(CurrencyValue);
                    minusz = true;
                }
                List<int> szamok = new List<int>();
                int l = CurrencyValue;
                while (l > 0)
                {
                    szamok.Add(l % 10);
                    l /= 10;
                }

                for (int e = 0; e < 3; e++)
                {
                    var s = "";
                    ai = new int[3];
                    if (szamok.Count > 0)
                    {
                        int i = e * 3;
                        for (; i < (e + 1) * 3 && i <= szamok.Count - 1; i++)
                        {
                            ai[i - e * 3] = szamok[i];
                        }
                        switch (i - e * 3)
                        {
                            case 3:
                                s = s + S3();
                                break;
                            case 2:
                                s = s + S2();
                                break;
                            case 1:
                                s = s + S1();
                                break;
                        }
                        ezresek.Add(s);
                    }
                }
                var szam = "";
                for (int i = 0; i < ezresek.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ezresek[i]))
                    {
                        if (CurrencyValue > 2000 && szam != "")
                        {
                            szam = "-" + szam;
                        }
                        szam = ezresek[i] + szorzo[i] + szam;
                    }
                }
                if (minusz)
                {
                    szam = "mínusz " + szam;
                }
                return szam;
            }
        }

        #endregion SzamValtasaBeture
    }
}
