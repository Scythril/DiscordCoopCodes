using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace DiscordCoopCodes {
    public static class ArgumentsHelper {
        public static List<KeyValuePair<int, string>> bignums = new List<KeyValuePair<int, string>> {
            new KeyValuePair<int, string>(3, "K"),
            new KeyValuePair<int, string>(6, "M"),
            new KeyValuePair<int, string>(9, "B"),
            new KeyValuePair<int, string>(12, "T"),
            new KeyValuePair<int, string>(15, "q"),
            new KeyValuePair<int, string>(18, "Q"),
            new KeyValuePair<int, string>(21, "s"),
            new KeyValuePair<int, string>(24, "S"),
            new KeyValuePair<int, string>(27, "O"),
            new KeyValuePair<int, string>(30, "N"),
            new KeyValuePair<int, string>(33, "d"),
            new KeyValuePair<int, string>(36, "u"),
            new KeyValuePair<int, string>(39, "D"),
            new KeyValuePair<int, string>(42, "Td"),
            new KeyValuePair<int, string>(45, "qd"),
            new KeyValuePair<int, string>(48, "Qd"),
            new KeyValuePair<int, string>(51, "sd"),
            new KeyValuePair<int, string>(54, "Sd"),
            new KeyValuePair<int, string>(57, "Od"),
            new KeyValuePair<int, string>(60, "Nd"),
            new KeyValuePair<int, string>(63, "V")
        };

        public static string ToEggString(this double number)
        {
            return NumberToString(number);
        }

        public static string NumberToString(double number)
        {
            var nums = bignums.OrderByDescending(x => x.Key);
            foreach(var num in nums)
            {
                if (number > Math.Pow(10.0, num.Key))
                    return (number / Math.Pow(10.0, num.Key)).ToString("0.00") + num.Value;
            }
            return number.ToString();
        }

        public static BigInteger NumberFromString(string arg) {
            var size = arg[arg.Length - 1];
            var numberPortion = arg.Substring(0, arg.Length - 1);
            Console.WriteLine($"Size: {size} NumberPortion: {numberPortion} ");
            BigInteger number;

            if(BigInteger.TryParse(numberPortion, out number)) {
                 switch(size) {
                    case 'B':
                        number *= BigInteger.Pow(10, 9);
                        break;
                    case 'T':
                        number *= BigInteger.Pow(10, 12);
                        break;
                    case 'q':
                        number *= BigInteger.Pow(10, 15);
                        break;
                    default:
                        throw new UnableToParseNumberExecption();
                }
            } else {
                throw new UnableToParseNumberExecption();
            }
            return number;
        }

        //public static string NunberToString(BigInteger number) {

        //}
    }

    public class UnableToParseNumberExecption : Exception {

    }
}
