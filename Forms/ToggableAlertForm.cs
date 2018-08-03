using System;
using System.Windows.Forms;

namespace ACSE
{
    public sealed partial class ToggableAlertForm : Form
    {
        public event EventHandler<AlertToggledEventArgs> AlertToggled;
        public bool AlertDisabled {
            get => toggleCheckBox.Checked;
            set
            {
                toggleCheckBox.Checked = value;
                OnAlertToggled(new AlertToggledEventArgs(value));
            }
        }
        public string Message
        {
            get => infoLabel.Text;
            set => infoLabel.Text = value;
        }

        public ToggableAlertForm(string message, string title = null)
        {
            InitializeComponent();
            okayButton.Click += OkayButton_Click;
            Message = message;
            Text = title ?? "Alert";
        }

        public new bool Close()
        {
            var disabled = AlertDisabled;
            base.Close();
            return disabled;
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnAlertToggled(AlertToggledEventArgs e)
        {
            AlertToggled?.Invoke(this, e);
        }
    }
}
