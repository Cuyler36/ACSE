using System;
using System.Windows.Forms;
using System.Drawing;

namespace ACSE
{
    class PlaceholderTextBox : TextBox
    {
        private Label PlaceholderLabel;
        
        public Color PlaceholderTextColor
        {
            get
            {
                return PlaceholderLabel.ForeColor;
            }
            set
            {
                PlaceholderLabel.ForeColor = value;
            }
        }

        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                PlaceholderLabel.Size = value;
            }
        }

        public new Point Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
                PlaceholderLabel.Location = value;
            }
        }

        public string PlaceholderText
        {
            get
            {
                return PlaceholderLabel.Text;
            }
            set
            {
                PlaceholderLabel.Text = value;
            }
        }

        public PlaceholderTextBox()
        {
            PlaceholderLabel = new Label();
            PlaceholderLabel.Text = "Placeholder Text";
            PlaceholderLabel.BackColor = Color.Transparent;
            PlaceholderLabel.ForeColor = Color.Gray;
            PlaceholderLabel.AutoSize = false;
            PlaceholderLabel.MouseDown += LabelClicked;

            Controls.Add(PlaceholderLabel);
            LostFocus += FocusLost;
        }

        private void LabelClicked(object sender, MouseEventArgs e)
        {
            PlaceholderLabel.Visible = false;
            Focus();
        }

        private void FocusLost(object sender, EventArgs e)
        {
            PlaceholderLabel.Visible = string.IsNullOrEmpty(Text);
        }

        public new void Dispose()
        {
            PlaceholderLabel.Dispose();
            base.Dispose();
        }
    }
}
