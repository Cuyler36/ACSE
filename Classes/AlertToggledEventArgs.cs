using System;

namespace ACSE
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
