using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShannonFanoHuffman
{
    public class Node
    {
        public string Code { get; set; }
        public double Frequency { get; set; }
        public string Character { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node()
        {
            Code = "";
        }
        public Node(string character, double frequency)
        {
            Frequency = frequency;
            Character = character;
        }
        public Node(string character, double frequency, Node left, Node right)
        {
            Frequency = frequency;
            Character = character;
            Left = left;
            Right = right;
        }
        public int Count()
        {
            var result = 1;
            if (Right != null)
                result += Right.Count();
            if (Left != null)
                result += Left.Count();
            return result;
        }
        public Node[] GetNodes(string character)
        {
            var result = new List<Node>();
            if (Character == character)
                result.Add(this);
            if (Right != null)
                result.AddRange(Right.GetNodes(character).ToList());
            if (Left != null)
                result.AddRange(Left.GetNodes(character).ToList());
            return result.ToArray();
        }
        public Node[] GetNodes()
        {
            var result = new List<Node>();
            result.Add(this);
            if (Right != null)
                result.AddRange(Right.GetNodes().ToList());
            if (Left != null)
                result.AddRange(Left.GetNodes().ToList());
            return result.ToArray();
        }
    }
    public class Tree
    {
        public Node Root { get; set; }

        public Tree()
        {
            Root = null;
        }

        public Tree(params Node[] nodes)
        {
            if (nodes.Length == 0)
            {
                Root = null;
            }
            else
            {
                Root = nodes[0];
                for (var i = 1; i < nodes.Length; i++)
                {
                    var tempNode = Root;
                    while (true)
                    {
                        if (nodes[i].Code.CompareTo(tempNode.Code) >= 0)
                        {
                            if (tempNode.Right != null)
                                tempNode = tempNode.Right;
                            else
                            {
                                tempNode.Right = nodes[i];
                                break;
                            }
                        }
                        else
                        {
                            if (tempNode.Left != null)
                                tempNode = tempNode.Left;
                            else
                            {
                                tempNode.Left = nodes[i];
                                break;
                            }
                        }
                    }
                    tempNode = nodes[i];
                }
            }
        }

        public int Count()
        {
            return Root.Count();
        }

        public Node[] GetNodes(string key)
        {
            if (Root == null)
                return null;
            return Root.GetNodes(key);
        }
        public Node[] GetNodes()
        {
            if (Root == null)
                return null;
            return Root.GetNodes();
        }

        public Dictionary<char, string> GetLeaves(Node current)
        {
            if (current.Left == null && current.Right == null)
                return new Dictionary<char, string>() { { current.Character[0], current.Code } };
            else return GetLeaves(current.Left).Union(GetLeaves(current.Right)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
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
            var flag = Math.Abs(sum - 0.5 * sumFreq) <= Math.Abs(sum - freqAlphabet[index] - 0.5 * sumFreq) ? 1 : 0;
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

        public static Dictionary<char, string> GetHuffmanAlphabet(Dictionary<char, double> alphabet)
        {
            var tree = GetHuffmanTree(alphabet);
            FillHuffmanTree(tree.Root);
            return tree.GetLeaves(tree.Root);
        }

        public static Tree GetHuffmanTree(Dictionary<char, double> alphabet)
        {
            var len = alphabet.Count;
            var nodes = new List<Node>();
            foreach (var e in alphabet)
            {
                var node = new Node(e.Key.ToString(), e.Value);
                nodes.Add(node);
            }
            while (nodes.Count > 1)
            {
                len = nodes.Count;
                var sumFreq = nodes[len - 2].Frequency + nodes[len - 1].Frequency;
                var sumChar = nodes[len - 2].Character + nodes[len - 1].Character;
                var node = new Node(sumChar, sumFreq, nodes[len - 2], nodes[len - 1]);
                nodes.RemoveAt(len - 1);
                nodes.RemoveAt(len - 2);
                nodes.Add(node);
                nodes = nodes.OrderByDescending(x => x.Frequency).ThenBy(x => x.Character.Length).ThenBy(x => x.Character).ToList();
            }
            nodes[0].Code = "";
            return new Tree() { Root = nodes[0] };
        }

        public static void FillHuffmanTree(Node root)
        {
            if (root.Left == null & root.Right == null)
                return;
            root.Left.Code = root.Code + "0";
            root.Right.Code = root.Code + "1";
            FillHuffmanTree(root.Right);
            FillHuffmanTree(root.Left);
        }

        public static double GetEntropy(IEnumerable<double> probabs)
        {
            var res = 0.0;
            foreach (var e in probabs)
                res -= e * Math.Log(e, 2);
            return res;
        }

        public static double GetAverageLength(double[] probabs, string[] codes)
        {
            var res = 0.0;
            for (var i = 0; i < probabs.Length; i++)
                res += probabs[i] * codes[i].Length;
            return res;
        }

        public static string Encode(string sourceText, Dictionary<char, string> alphabet)
        {
            var result = new StringBuilder(sourceText);
            foreach (var e in alphabet)
                result = result.Replace(e.Key.ToString(), e.Value);
            return result.ToString();
        }

        public static string Decode(string sourceText, Dictionary<string, char> alphabet)
        {
            var buffer = new StringBuilder();
            var result = new StringBuilder(sourceText);
            var index = 0;
            while (true)
            {
                buffer.Append(result[index]);
                if (alphabet.ContainsKey(buffer.ToString()))
                {
                    index -= buffer.Length - 1;
                    result.Remove(index, buffer.Length);
                    result.Insert(index, alphabet[buffer.ToString()].ToString());
                    buffer.Clear();
                }
                index++;
                if (index >= result.Length)
                    break;
            }
            return result.ToString();
        }

        static void Main(string[] args)
        {
            var baseText = "Meet my family. There are five of us – my parents, my elder brother, my baby sister and me. First, meet my mum and dad, Jane and Michael. My mum enjoys reading and my dad enjoys playing chess with my brother Ken. My mum is slim and rather tall. She has long red hair and big brown eyes. She has a very pleasant smile and a soft voice.".ToLower();
            Console.WriteLine("Input source text: ");
            var sourceText = Console.ReadLine().ToLower();
            var text = "AAAAAAAA";
            if (sourceText.Length == 0)
                sourceText = baseText;
            var len = sourceText.Length;
            var charsFrequencies = sourceText.GroupBy(x => x).OrderByDescending(x => x.Count()).ToDictionary(x => x.Key, x => (double)x.Count() / len);
            var chars = charsFrequencies.Keys.ToArray();
            var frequencies = charsFrequencies.Values.ToArray();
            var codes = GetShannonFanoAlphabet(frequencies);
            var alphabetShannonFano = new Dictionary<char, string>();
            Console.WriteLine("Shannon-Fano:");
            for (var i = 0; i < codes.Length; i++)
            {
                alphabetShannonFano[chars[i]] = codes[i];
                Console.WriteLine("{0} {1:0.000} {2}", chars[i], frequencies[i], codes[i]);
            }
            var encodedTextShannonFano = Encode(sourceText, alphabetShannonFano);
            var alphabetHuffman = GetHuffmanAlphabet(charsFrequencies);
            var encodedTextHuffman = Encode(sourceText, alphabetHuffman);
            Console.WriteLine("\nHuffman:");
            foreach (var e in alphabetHuffman.OrderByDescending(x=>charsFrequencies[x.Key]).ThenBy(x => x.Key))
            {
                Console.WriteLine("{0} {1:0.000} {2}", e.Key, charsFrequencies[e.Key], e.Value);
            }
            Console.WriteLine("\nEntropy - {0:0.000}", GetEntropy(frequencies));
            Console.WriteLine("Average length Shannon-Fano - {0:0.000}", GetAverageLength(frequencies, codes));
            Console.WriteLine("Average length Huffman - {0:0.000}", GetAverageLength(frequencies, alphabetHuffman.Values.ToArray()));
            Console.WriteLine("Start length - 8");
            Console.WriteLine();
            Console.WriteLine("Shannon-Fano encoded text:\n" + encodedTextShannonFano);
            Console.WriteLine("Shannon-Fano decoded text:\n" + Decode(encodedTextShannonFano, alphabetShannonFano.ToDictionary(x => x.Value, x => x.Key)));
            Console.WriteLine();
            Console.WriteLine("Huffman encoded text:\n" + encodedTextHuffman);
            Console.WriteLine("Huffman decoded text:\n" + Decode(encodedTextHuffman, alphabetHuffman.ToDictionary(x => x.Value, x => x.Key)));
            Console.WriteLine();
            Console.WriteLine(sourceText);
            Console.ReadKey();
        }
    }
}
