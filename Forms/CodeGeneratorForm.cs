using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACSE.Forms
{
    public partial class CodeGeneratorForm : Form
    {
        public CodeGeneratorForm()
        {
            InitializeComponent();
            codeTypeComboBox.SelectedIndex = 0;
        }

        private void CodeTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (codeTypeComboBox.SelectedIndex > -1)
            {

            }
        }
    }
}
