using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DiscordCoopCodes {
    public class ArgumentsHelper {
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
