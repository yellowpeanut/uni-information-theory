using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendedNumerics;

namespace crypt4
{
    public partial class Form1 : Form
    {
        private string fileContent = String.Empty;
        Arithmetic ari = null;

        public Form1()
        {
            InitializeComponent();
            //BigDecimal.Precision = 100000;
            //BigDecimal.AlwaysTruncate = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileStream = dialog.OpenFile();
                fileContent = File.ReadAllText(dialog.FileName);
                ari = new Arithmetic(fileContent);
                // size considering only used symbols ( binary )
                //label4.Text = $"Initial file size: {getFileSize(fileContent, ari.bitsPerSymbol)} bytes";

                // utf-8 file size
                label4.Text = $"Initial file size: {getFileSize(fileContent, 8)} bytes";
                richTextBox1.Text = fileContent;
                string text = "";
                foreach (var symb in ari.SymbolsFrequency)
                {
                    text += $"{symb.Key}\t{symb.Value}\n";
                }
                richTextBox3.Text = text;
            }
            else
            {
                MessageBox.Show("Error opening the file", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        //compress
        private void button2_Click(object sender, EventArgs e)
        {
            if (fileContent == String.Empty)
            {
                MessageBox.Show("Initial file is empty!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string compressedFile = ari.Compress(fileContent);
            richTextBox4.Text = compressedFile;
            File.WriteAllText("2.txt", compressedFile);
            // file size considering file as one binary
            /*            HashSet<char> hs = new HashSet<char> { };
                        for (int i = 0; i < compressedFile.Length; i++)
                            hs.Add(compressedFile[i]);
                        int bps = Convert.ToInt32(Math.Ceiling(Math.Log(hs.Count, 2)));
                        int compSize = compressedFile.Length * bps / 8 + 1;

                        label5.Text = $"Compressed size: {compSize} bytes";*/

            //file size considering file as two binary files
            label5.Text = $"Compressed size: {ari.headerSize + ari.contentSize} bytes";

            string text = "";
            foreach (var symb in ari.SymbolsFrequency)
            {
                text += $"{symb.Key}\t{symb.Value}\t{ari.SymbolsProbability[symb.Key]}\n";
            }
            richTextBox3.Text = text;
        }

        //decompress
        private void button3_Click(object sender, EventArgs e)
        {
            string compressedFile = File.ReadAllText("2.txt");
            if (compressedFile == String.Empty)
            {
                MessageBox.Show("Compressed file is empty!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (ari is null)
                ari = new Arithmetic();
            string decompressedFile = ari.Decompress(compressedFile);
            File.WriteAllText("3.txt", decompressedFile);
            richTextBox2.Text = decompressedFile;
            // size considering only used symbols ( binary )
            //label6.Text = $"Decompressed size: {getFileSize(decompressedFile, ari.bitsPerSymbol)} bytes";

            // utf-8 file size
            label6.Text = $"Decompressed size: {getFileSize(decompressedFile, 8)} bytes";
        }

        private int getFileSize(string file, int bitsPerSymbol)
        {
            return Convert.ToInt32(Math.Ceiling(Convert.ToDouble(file.Length * bitsPerSymbol) / 8.0));
        }
    }

}
