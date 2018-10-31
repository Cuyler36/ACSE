using System;

namespace ACSE.WinForms
{
    public class AlertToggledEventArgs : EventArgs
    {
        public bool Disabled;

        public AlertToggledEventArgs(bool disabled)
        {
            Disabled = disabled;
        }
    }
}
