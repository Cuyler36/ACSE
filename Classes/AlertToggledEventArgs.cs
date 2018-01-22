using System;

namespace ACSE.Classes
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
