using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;


namespace crypt1
{
    public partial class Form1 : Form
    {
        public List<Alphabet> abs = new List<Alphabet>();
        public Alphabet ab = new Alphabet();
        public int symbSize;
        public Form1()
        {
            InitializeComponent();
            string file = File.ReadAllText($"{Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString())}/alphabets.json");
            abs = JsonSerializer.Deserialize<List<Alphabet>>(file);
            foreach (var i in abs) comboBox1.Items.Add(i.Name);
            comboBox1.SelectedIndex = 0;
            alphSize();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            mesSize();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            alphSize();
            mesSize();
        }

/*        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            string s = getSymbols();
            //foreach (char c in s)
            //    Debug.WriteLine(c);
            char[] cs = s.ToCharArray();
            richTextBox2.Text = String.Join(" ", cs);
        }*/

        public void alphSize()
        {
            ab = abs[comboBox1.SelectedIndex];
            char[] symb = ab.Symbols.ToCharArray();
            richTextBox2.Text = String.Join(" ", symb);
            label4.Text = $"Alphabet size: {ab.Symbols.Length} symbols";
            symbSize = Convert.ToInt32(Math.Ceiling(Math.Log(ab.Symbols.Length, 2)));
            label5.Text = $"Symbol size: {symbSize} bits";
        }

        public void mesSize()
        {
            string txt = richTextBox1.Text.ToLower();
            int size = 0;
            for (int i = 0; i < txt.Length; i++)
            {
                if (ab.Symbols.Contains(txt[i])) size += symbSize;
            }
            label6.Text = $"Message size: {size} bits";
        }

        /*        private string getSymbols()
                {
                    //List<string> symbols = (richTextBox2.Text.Split(' ')).ToList();

                    string text = "";
                    *//*
                                for (int i = 0; i < symbols.Count; i++)
                                {
                                    if(symbols[i] == "")
                                    {
                                        if (i < symbols.Count - 1)
                                        {
                                            symbols[i] = " ";
                                            symbols.RemoveAt(i + 1);
                                            i--;
                                        }
                                        else
                                            symbols.RemoveAt(i);
                                    }
                                }
                    */
        /*            symbols = symbols.Distinct().ToList();

                    for (int i = 0; i < symbols.Count; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if(symbols[j] == symbols[i])
                            {
                                symbols.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }*//*

        HashSet<string> symbols = (richTextBox2.Text.Split(' ')).ToHashSet();
        symbols.Remove("");
*//*            string[] str = new string[] { };
            foreach (var s in symbols)
                if (s.Length > 1)
                    str.Append(s);

            for (int i = 0; i < str.Count(); i++)
            {
                for (int j = 0; j < str[i].Length; j++)
                    symbols.Add(str[i][j].ToString());
                symbols.Remove(str[i]);
            }*//*

            //foreach (var s in symbols)
             //   Debug.WriteLine(s);

            foreach (string s in symbols)
                text += s;

            HashSet<char> sbs = text.ToCharArray().ToHashSet();
            foreach (var s in sbs)
                Debug.WriteLine(s);

            text = "";
            foreach (var s in sbs)
                text += s.ToString();

            return text;
        }*/

        public void serialize()
        {
            string jsonString = JsonSerializer.Serialize(abs);
            File.WriteAllText($"{Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString())}/alphabets.json", jsonString);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FormEdit fe = new FormEdit(this);
            fe.Show();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            FormAdd fa = new FormAdd(this);
            fa.Show();
        }
    }


    public class Alphabet
    {
        public string Name { get; set; }
        public string Symbols { get; set; }
    }
}
