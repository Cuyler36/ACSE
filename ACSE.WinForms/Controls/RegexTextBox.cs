using System.ComponentModel;
using System.Text.RegularExpressions;

namespace System.Windows.Forms
{
    public class RegexTextBox : TextBox
    {
        [Category("Behavior")]
        public string RegexPattern { get; set; }
        private bool _delOrBack;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                _delOrBack = true;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!Regex.IsMatch(Text + e.KeyChar, RegexPattern) && !_delOrBack)
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyPress(e);
            }

            _delOrBack = false;
        }
    }

    public class NumericTextBox : RegexTextBox
    {
        public NumericTextBox()
        {
            RegexPattern = "^[0-9]+$";
        }
    }
}