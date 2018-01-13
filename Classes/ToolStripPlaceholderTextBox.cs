using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ACSE
{
    class ToolStripPlaceholderTextBox : ToolStripItem
    {
        private PlaceholderTextBox PlaceholderBox = new PlaceholderTextBox();

        public new string Text
        {
            get
            {
                return PlaceholderBox.Text;
            }
            set
            {
                PlaceholderBox.Text = value;
            }
        }

        public string PlaceholderText
        {
            get
            {
                return PlaceholderBox.PlaceholderText;
            }
            set
            {
                PlaceholderBox.PlaceholderText = value;
            }
        }

        public Color PlaceholderTextColor
        {
            get
            {
                return PlaceholderBox.PlaceholderTextColor;
            }
            set
            {
                PlaceholderBox.PlaceholderTextColor = value;
            }
        }

        public ToolStripPlaceholderTextBox()
        {
            PlaceholderBox = new PlaceholderTextBox();
        }
    }
}
