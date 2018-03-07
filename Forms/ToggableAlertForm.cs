using System;
using System.Windows.Forms;

namespace ACSE
{
    public partial class ToggableAlertForm : Form
    {
        public event EventHandler<AlertToggledEventArgs> AlertToggled;
        public bool AlertDisabled {
            get
            {
                return toggleCheckBox.Checked;
            }
            set
            {
                toggleCheckBox.Checked = value;
                OnAlertToggled(new AlertToggledEventArgs(value));
            }
        }
        public string Message
        {
            get
            {
                return infoLabel.Text;
            }
            set
            {
                infoLabel.Text = value;
            }
        }

        public ToggableAlertForm(string Message, string Title = null)
        {
            InitializeComponent();
            okayButton.Click += OkayButton_Click;
            this.Message = Message;
            Text = Title ?? "Alert";
        }

        public new bool Close()
        {
            bool Disabled = AlertDisabled;
            base.Close();
            return Disabled;
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected virtual void OnAlertToggled(AlertToggledEventArgs e)
        {
            AlertToggled?.Invoke(this, e);
        }
    }
}
