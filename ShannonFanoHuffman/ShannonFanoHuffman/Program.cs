using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShannonFanoHuffman
{
    class Program
    {
        public static string[] GetShannonFanoAlphabet(double[] freqAlphabet)
        {
            if (freqAlphabet.Length == 1)
                return new string[] { "" };
            var index = 0;
            var sumFreq = freqAlphabet.Sum();
            var sum = freqAlphabet[0];
            while (sum < 0.5 * sumFreq)
            {
                index++;
                sum += freqAlphabet[index];
            }
            double[] firstPart, secondPart;
            var flag = Math.Abs(sum - 0.5 * sumFreq) <= Math.Abs(sum - freqAlphabet[index] - 0.5 * sumFreq)?1:0;
                firstPart = new double[index + flag];
                secondPart = new double[freqAlphabet.Length - index - flag];
                for (var i = 0; i < freqAlphabet.Length; i++)
                    if (i < index + flag)
                        firstPart[i] = freqAlphabet[i];
                    else
                        secondPart[i - index - flag] = freqAlphabet[i];
            var firstCode = GetShannonFanoAlphabet(firstPart);
            var secondCode = GetShannonFanoAlphabet(secondPart);
            for (var i = 0; i < firstCode.Length; i++)
                firstCode[i] = "0" + firstCode[i];
            for (var i = 0; i < secondCode.Length; i++)
                secondCode[i] = "1" + secondCode[i];
            return firstCode.Concat(secondCode).ToArray();
        }
        static void Main(string[] args)
        {
            var baseText = "Meet my family. There are five of us – my parents, my elder brother, my baby sister and me. First, meet my mum and dad, Jane and Michael. My mum enjoys reading and my dad enjoys playing chess with my brother Ken. My mum is slim and rather tall. She has long red hair and big brown eyes. She has a very pleasant smile and a soft voice.".ToLower();
            Console.WriteLine("Input source text: ");
            var sourceText = Console.ReadLine().ToLower();
            if (sourceText.Length == 0)
                sourceText = baseText;
            var len = sourceText.Length;
            var charsFrequencies = sourceText.GroupBy(x => x).OrderByDescending(x => x.Count()).ToDictionary(x => x.Key, x => (double)x.Count() / len);
            var chars = charsFrequencies.Keys.ToArray();
            var frequencies = charsFrequencies.Values.ToArray();
            var codes = GetShannonFanoAlphabet(frequencies);
            for (var i = 0; i < codes.Length; i++)
            {
                Console.WriteLine("{0} {1:0.000} {2}", chars[i], frequencies[i], codes[i]);
            }
            Console.ReadKey();
        }
    }
}
