using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedNumerics;

namespace crypt4
{
    class Arithmetic
    {
        public int bitsPerSymbol = 0;
        public Dictionary<char, int> SymbolsFrequency = new Dictionary<char, int> { };
        public Dictionary<char, decimal> SymbolsProbability = new Dictionary<char, decimal> { };
        public string FileContent = "";

        public int headerSize = 0;
        public int contentSize = 0;

        //msg size needs testing
        // 
        int encodingMsgSize = 18;
        public Arithmetic(string FileContent)
        {
            this.FileContent = FileContent;
            FindSymbolsFrequency(FileContent);
            //Debug.WriteLine(BigDecimal.Precision.ToString());
        }
        public Arithmetic() { }

        // ---------- COMPRESSION ---------- //
        private Dictionary<char, decimal> CreateProbabilityTable(Dictionary<char, int> symbolsFrequency, int fileLength)
        {
            //BigDecimal fileLen = Convert.ToDecimal(fileLength);
            int len = 0;
            foreach (var s in symbolsFrequency)
                len += s.Value;
            Dictionary<char, decimal> pt = new Dictionary<char, decimal> { };
            foreach (var symb in symbolsFrequency)
                pt.Add(symb.Key, Convert.ToDecimal(symb.Value)/len);

            return pt;
        }

        private Dictionary<char, Symbol> SetSymbolBounds(Dictionary<char, decimal> pt)
        {
            Dictionary<char, Symbol> symbols = new Dictionary<char, Symbol> { };
            decimal low = 0.0m;
            decimal high = 0.0m;
            foreach (var symb in pt)
            {
                high += symb.Value;
                symbols.Add(symb.Key, new Symbol(symb.Key, low, high));
                low = high;
            }

            return symbols;
        }


        private string EncodeText(string file, Dictionary<char, Symbol> symbols)
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

            int n = 0;
            if (file.Length % encodingMsgSize != 0)
                n = file.Length / encodingMsgSize + 1;
            else
                n = file.Length / encodingMsgSize;

            for (int i = 0; i < n; i++)
            {
                decimal low = 0.0m;
                decimal high = 1.0m;
                decimal currentRange = high - low;
                Symbol s = null;
                for (int j = 0; j < encodingMsgSize; j++)
                {
                    if (j + encodingMsgSize*i >= file.Length)
                        break;
                    s = symbols[file[j + encodingMsgSize * i]];
                    high = low + (currentRange * s.High);
                    low = low + (currentRange * s.Low);
                    currentRange = high - low;
                    //Debug.WriteLine($"({low}:{high})");
                }
                content += (((high + low) / 2.0m).ToString() + "\n");
            }
            contentSize = content.Length * 4 / 8 + 1;

            string text = header + content;
            return text;
        }

        public string Compress(string file)
        {
            if (this.SymbolsFrequency.Count == 0)
                FindSymbolsFrequency(file);
            // create probability table
            SymbolsProbability = CreateProbabilityTable(this.SymbolsFrequency, file.Length);
            // set bounds to symbols
            Dictionary<char, Symbol> symbols = SetSymbolBounds(SymbolsProbability);
            // encode the file contents
            string encodedFile = EncodeText(file, symbols);

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
                if (symbols[i] != "")
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
        private string DecodeText(string file, int fileLength, Dictionary<char, Symbol> symbols)
        {
            string text = "";
            /*            BigDecimal value = 0d;  // Convert.ToDecimal(file);
                        for (int i = 0; i < file.Length-2; i++)
                        {
                            BigDecimal dig = BigDecimal.Parse(Convert.ToDouble(file[i + 2]));
                            value += dig / (Math.Pow(10, i + 1));
                        }*/

            string[] fileVals = file.Split('\n');
            decimal value = 0.0m;

            for (int i = 0; i < fileVals.Length-1; i++)
            {
                value = Decimal.Parse(fileVals[i]);

                //Debug.WriteLine(file);
                //Debug.WriteLine(value.ToString());
                decimal low = 0.0m;
                decimal high = 1.0m;
                decimal currentRange = high - low;
                for (int j = 0; j < encodingMsgSize; j++)
                {
                    if (j + i * encodingMsgSize >= fileLength)
                        break;
                    foreach (var symb in symbols)
                    {
                        if (symb.Value.InRange(value))
                        {
                            text += symb.Key;
                            currentRange = symb.Value.High - symb.Value.Low;
                            value = (value - symb.Value.Low) / currentRange;
                            //Debug.WriteLine(value.ToString());
                            break;
                        }
                    }
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
            for (int j = 0; j < symbFreq.Count; j++)
            {
                Debug.WriteLine($"({symbFreq.ElementAt(j).Key} {symbFreq.ElementAt(j).Value}) - [{SymbolsFrequency.ElementAt(j).Key} {SymbolsFrequency.ElementAt(j).Value}]");
            }
            // create probability table
            int fileLength = 0;
            foreach (var s in symbFreq)
                fileLength += s.Value;
            Dictionary<char, decimal> SymbolsProb = CreateProbabilityTable(this.SymbolsFrequency, fileLength);
            // set bounds to symbols
            Dictionary<char, Symbol> symbols = SetSymbolBounds(SymbolsProb);
            // encode the file contents
            string decodedText = DecodeText(content, fileLength, symbols);

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
