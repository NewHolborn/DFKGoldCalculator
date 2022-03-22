using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeeklyGain
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public string result = "";
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            result = textWallet.Text;
        }

        private void textWallet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) this.Close();
        }
    }
}
