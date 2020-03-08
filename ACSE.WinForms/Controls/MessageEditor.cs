using System.Windows.Forms;
using ACSE.Core.Messages.Mail;

namespace ACSE.WinForms.Controls
{
    public partial class MessageEditor : UserControl
    {
        private PlayerMailBase _mailReference;

        public MessageEditor()
        {
            InitializeComponent();
        }

        public void SetMailReference(PlayerMailBase mail)
        {
            _mailReference = mail;
            MessageContents.Text = mail.Contents;
        }
    }
}
