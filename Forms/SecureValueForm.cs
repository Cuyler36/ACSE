using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ACSE
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (NewMainForm.Save_File != null && (NewMainForm.Save_File.Save_Type == SaveType.New_Leaf || NewMainForm.Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                if (ulong.TryParse(textBox1.Text, NumberStyles.AllowHexSpecifier, null, out ulong Secure_NAND_Value))
                {
                    NewMainForm.Save_File.Write(0, Secure_NAND_Value);
                }
            }
            Hide();
        }
    }
}
