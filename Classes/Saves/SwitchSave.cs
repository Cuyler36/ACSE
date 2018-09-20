using System;

namespace ACSE.Saves
{
    /// <summary>
    /// Nintendo Switch Save File
    /// </summary>
    class SwitchSave : SaveBase
    {
        public SwitchSave(string path) : base(path, false)
        {
            Generation = SaveGeneration.Switch;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
