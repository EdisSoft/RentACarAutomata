using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionsCore.Utilities
{
    public static class FisherYates
    {
        private const int Key = 19881229;

        public static int Shuffle(int target, int length)
        {
            if (target >= (int)Math.Pow(10, length) || target < 0) throw new Exception("Hibás érték!!");
            string targetString = (target).ToString().PadLeft(length, '0');

            List<int> exchanges = GetExchanges(Key % (int)Math.Pow(10, length), targetString.Length);
            List<char> characters = targetString.ToList();

            for (int i = 0; i < targetString.Length; i++)
            {
                char temp = characters[i];
                characters[i] = characters[exchanges[i]];
                characters[exchanges[i]] = temp;
            }

            string result = new string(characters.ToArray());
            return Convert.ToInt32(result);
        }

        public static int Restore(int target, int length)
        {
            if (target >= (int)Math.Pow(10, length) || target < 0) throw new Exception("Hibás érték!!");
            string targetString = target.ToString().PadLeft(length, '0');

            List<int> exchanges = GetExchanges(Key % (int)Math.Pow(10, length), targetString.Length);
            List<char> characters = targetString.ToList();

            for (int i = targetString.Length - 1; i >= 0; --i)
            {
                char temp = characters[i];
                characters[i] = characters[exchanges[i]];
                characters[exchanges[i]] = temp;
            }

            string result = new string(characters.ToArray());
            return Convert.ToInt32(result);
        }

        private static List<int> GetExchanges(int key, int size)
        {
            Random source = new Random(key);
            List<int> exchanges = new List<int>();

            if (size < 2)
                return new List<int> { 0, 0, 0, 0 };

            for (int i = 0; i < size; i++)
            {
                exchanges.Add(source.Next(0, size));
            }

            return exchanges;
        }
    }
}
