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
    public partial class FormEdit : Form
    {
        Form1 form;
        int index;
        public FormEdit(Form1 form)
        {
            InitializeComponent();
            this.form = form;
            index = form.comboBox1.SelectedIndex;
            richTextBox1.Text = form.ab.Symbols;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = richTextBox1.Text;
            List<char> symb = text.ToList<char>();
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

            text = "";
            foreach (var s in symb)
                text += s.ToString();

            form.ab.Symbols = text;
            form.abs[index].Symbols = text;
            form.serialize();
            form.alphSize();
            form.mesSize();
            this.Close();
        }
    }
}
