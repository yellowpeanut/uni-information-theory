using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypt3
{
    public class Huffman
    {
        public int bitsPerSymbol = 0;
        public Dictionary<char, int> SymbolsFrequency = new Dictionary<char, int> { };
        public Dictionary<char?, string> SymbolsCode = new Dictionary<char?, string> { };
        public string FileContent = "";

        public int headerSize = 0;
        public int contentSize = 0;
        public Huffman(string FileContent)
        {
            this.FileContent = FileContent;
            FindSymbolsFrequency(FileContent);
        }
        public Huffman() { }

        // ---------- COMPRESSION ---------- //
        private List<Node> CreatePriorityQueue(Dictionary<char, int> symbolsFrequency)
        {
            List<Node> pq = new List<Node> { };
            foreach(var symb in symbolsFrequency)
                pq.Add(new Node(symb.Key, symb.Value));
            pq.Sort();
            pq.Reverse();
            return pq;
        }

        private Node BuildBinaryTree(List<Node> pq)
        {
            while (pq.Count > 1)
            {
                Node node1 = pq.First();
                pq.RemoveAt(0);
                Node node2 = pq.First();
                pq.RemoveAt(0);

                Node mergedNode = new Node(null, node1.Frequency + node2.Frequency);
                mergedNode.Left = node1;
                mergedNode.Right = node2;

                pq.Add(mergedNode);
            }
            Node root = pq.First();
            return root;
        }

        private Dictionary<char?, string> SetCodes(Node root)
        {
            Dictionary<char?, string> sc = new Dictionary<char?, string> { };
            foreach (var symb in SymbolsFrequency)
                sc.Add(symb.Key, "");
            SetCode(root, "");
            return sc;
            
            void SetCode(Node node, string code)
            {
                if (node.Left == null)
                    sc[node.Character] = code;
                else
                {
                    SetCode(node.Left, code + "0");
                    SetCode(node.Right, code + "1");
                }
            }
        }

        private string EncodeText(string file)
        {
            string header = "";
            string content = "";

            foreach (var symb in SymbolsFrequency)
                header += $"{symb.Key}{symb.Value} ";
            header += "\n\n";

            HashSet<char> hs = new HashSet<char> { };
            for (int i = 0; i < header.Length; i++)
                hs.Add(header[i]);
            int bps = Convert.ToInt32(Math.Ceiling(Math.Log(hs.Count, 2)));
            headerSize = header.Length * bps / 8 + 1;

            for (int i = 0; i < file.Length; i++)
                content += SymbolsCode[file[i]];

            contentSize = content.Length/8 + 1;

            string text = header + content;
            return text;
        }

        public string Compress(string file)
        {
            if (this.SymbolsFrequency.Count == 0)
                FindSymbolsFrequency(file);
            // make a heap - priority queue
            List<Node> PriorityQueue = CreatePriorityQueue(this.SymbolsFrequency);
            // build huffman tree
            Node BinaryTreeRoot = BuildBinaryTree(PriorityQueue);
            // assign codes to the characters
            SymbolsCode = SetCodes(BinaryTreeRoot);
            // encode the file contents
            string encodedFile = EncodeText(file);

            return encodedFile;
        }
        // -------------------- //

        // ---------- DECOMPRESSION ---------- //
        private Dictionary<char, int> CreateFreqDictionary(string header)
        {
            Dictionary<char, int> sf = new Dictionary<char, int> { };
            List<string> symbols = (header.Split(' ')).ToList();
            for (int i = 0; i < symbols.Count; i++)
            {
                if(symbols[i] != "")
                {
                    string s = symbols[i];
                    string value = "";
                    for (int j = 1; j < s.Length; j++)
                        value += s[j];
                    sf.Add(s[0], Convert.ToInt32(value));
                }
                else
                {
                    if (i < symbols.Count - 1)
                    {
                        symbols[i] = $" {symbols[i + 1]}";
                        symbols.RemoveAt(i + 1);
                        i--;
                    }
                    else
                        symbols.RemoveAt(i);
                }
            }

            return sf;
        }
        private string DecodeText(string file, Dictionary<char?, string> symbCode)
        {
            string text = "";
            string symbol = "";
            for (int i = 0; i < file.Length; i++)
            {
                symbol += file[i];
                if (symbCode.ContainsValue(symbol))
                {
                    text += symbCode.FirstOrDefault(x => x.Value == symbol).Key;
                    symbol = "";
                }
            }
            return text;
        }

        public string Decompress(string file)
        {
            // split the header and encoded text
            string header = "";
            string content = "";
            int i = 0;
            while ($"{file[i]}{file[i + 1]}" != "\n\n")
            {
                header += file[i];
                i++;
            }
            i += 2;
            for (int j = i; j < file.Length; j++)
                content += file[j];
            // create symbols frequency dictionary
            Dictionary<char, int> symbFreq = CreateFreqDictionary(header);
            // create priority queue
            List<Node> PriorityQueue = CreatePriorityQueue(symbFreq);
            // create binary tree
            Node BinaryTreeRoot = BuildBinaryTree(PriorityQueue);
            // create symbols code dictionary
            Dictionary<char?, string> symbCode = SetCodes(BinaryTreeRoot);
            //decode encoded text
            string decodedText = DecodeText(content, symbCode);

            return decodedText;
        }
        // -------------------- //

        private void FindSymbolsFrequency(string file)
        {
            for (int i = 0; i < file.Length; i++)
            {
                if (SymbolsFrequency.ContainsKey(file[i]))
                    SymbolsFrequency[file[i]] += 1;
                else
                    SymbolsFrequency[file[i]] = 1;
            }
            bitsPerSymbol = Convert.ToInt32(Math.Ceiling(Math.Log(SymbolsFrequency.Count, 2)));
        }
    }
}
