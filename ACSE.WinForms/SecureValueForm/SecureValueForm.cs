using System;
using System.Globalization;
using System.Windows.Forms;
using ACSE.Core.Saves;

namespace ACSE.WinForms
{
    public partial class SecureValueForm : Form
    {
        public SecureValueForm()
        {
            InitializeComponent();
        }

        public void Set_Secure_NAND_Value(ulong value)
        {
            textBox1.Text = value.ToString("X16");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MainForm.SaveFile != null && (MainForm.SaveFile.SaveGeneration == SaveGeneration.N3DS))
            {
                if (ulong.TryParse(textBox1.Text, NumberStyles.AllowHexSpecifier, null, out var secureNandValue))
                {
                    MainForm.SaveFile.Write(0, secureNandValue);
                }
            }
            Hide();
        }
    }
}
