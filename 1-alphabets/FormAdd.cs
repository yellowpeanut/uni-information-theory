using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace crypt1
{
    public partial class FormAdd : Form
    {
        Form1 form;
        public FormAdd(Form1 form)
        {
            InitializeComponent();
            this.form = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string abName = textBox1.Text.Trim();
            string abSymb = richTextBox1.Text;
            List<char> symb = abSymb.ToList<char>();
            for (int i = 0; i < symb.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (symb[j] == symb[i])
                    {
                        symb.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            abSymb = "";
            foreach (var s in symb)
                abSymb += s.ToString();

            Alphabet newAb = new Alphabet();
            newAb.Name = abName; newAb.Symbols = abSymb;
            form.abs.Add(newAb);
            form.comboBox1.Items.Add(newAb.Name);
            form.serialize();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
