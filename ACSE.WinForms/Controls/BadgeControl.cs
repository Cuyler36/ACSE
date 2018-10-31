using System;
using System.Drawing;
using System.Windows.Forms;
using ACSE.Core.Encryption;
using ACSE.Core.Saves;

namespace ACSE.WinForms.Controls
{
    public sealed class BadgeControl : OffsetablePictureBox
    {
        private static readonly Bitmap BadgeImage = Properties.Resources.Animal_Crossing_NL_Badges_28x28;
        private static readonly Bitmap NoBadgeImage = Properties.Resources.Animal_Crossing_NL_NoBadge_28x28;

        private static readonly string[] BadgeNames = {
            "Fish Caught", "Insects Caught", "Marine Creatures Caught", "Fish Collection", "Insect Collection", "Marine Collection", "Balloons",
            "Towns Visited", "Town Visits", "Watering Flowers", "Savings", "Turnips", "Medals", "StreetPass", "Weeds Removed", "Shopping", "Letters",
            "Refurbishments", "Catalog", "K.K. Slider Listens", "HRA Points", "Time Played", "Helping Neighbors", "Dream Suite"
        };

        private static readonly string[] BadgeLevels = { "None", "Bronze", "Silver", "Gold" };

        private static readonly int[][] BadgeValues = {
            new[] { 0, 500, 2000, 5000 },
            new[] { 0, 500, 2000, 5000 },
            new[] { 0, 100, 200, 1000 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 30, 100, 200 },
            new[] { 0, 100, 250, 500 },
            new[] { 0, 50, 200, 500 },
            new[] { 0, 100, 250, 500 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 500000, 3000000, 10000000 },
            new[] { 0, 300, 1500, 5000 },
            new[] { 0, 100, 300, 1000 },
            new[] { 0, 500, 2000, 5000 },
            new[] { 0, 500000, 2000000, 5000000 },
            new[] { 0, 50, 100, 200 },
            new[] { 0, 30, 100, 200 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 20, 50, 100 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 50, 100, 300 },
            new[] { 0, 50, 200, 500 }
        };

        internal bool Loaded;
        private readonly string _badgeName;
        private readonly int _index;
        private readonly int _dataOffset;
        private readonly int _valueOffset;
        private readonly ToolTip _badgeNameToolTip;
        public byte Stage;
        public NewLeafInt32 Value;

        private readonly Save _saveFile;

        public BadgeControl(Save save, int index, int offset, int valueOffset)
        {
            if (index >= BadgeNames.Length)
            {
                throw new Exception($"Badge index was invalid! Got {index} when {BadgeNames.Length - 1} was the maximum possible!");
            }

            _saveFile = save;
            _index = index;
            _dataOffset = offset;
            _valueOffset = valueOffset;
            Stage = _saveFile.ReadByte(offset);
            Value = new NewLeafInt32(_saveFile.ReadUInt32(valueOffset), _saveFile.ReadUInt32(valueOffset + 4));
            _badgeName = BadgeNames[index];

            _badgeNameToolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 100,
                ReshowDelay = 500,
                ShowAlways = true
            };

            _badgeNameToolTip.SetToolTip(PictureBox, _badgeName + $" - [{BadgeLevels[Stage]}] - " + Value.Value);

            Size = new Size(28, 28);
            ImageMaskingType = MaskingType.None;
            SetImageCrop();
            PictureBox.MouseClick += OnClick;

            Loaded = true;
        }

        private void SetImageCrop()
        {
            if (_index > -1 && _index < BadgeNames.Length && Stage < 4)
            {
                if (Stage == 0)
                {
                    Image = NoBadgeImage;
                    Offset = new Point();
                }
                else
                {
                    Image = BadgeImage;
                    Offset = new Point((Stage - 1) * 28, _index * 28);
                }
            }
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (Loaded && _dataOffset > -1 && _valueOffset > -1 && _index > -1 && _index < BadgeNames.Length)
            {
                Stage++;
                if (Stage > 3)
                {
                    Stage = 0;
                }

                Value = new NewLeafInt32((uint)BadgeValues[_index][Stage]);

                if (_saveFile != null)
                {
                    _saveFile.Write(_dataOffset, Stage);
                    _saveFile.Write(_valueOffset, Value.Int1);
                    _saveFile.Write(_valueOffset + 4, Value.Int2);
                }

                SetImageCrop();
                _badgeNameToolTip.SetToolTip(PictureBox, _badgeName + $" - [{BadgeLevels[Stage]}] - " + Value.Value);
            }
        }

        public new void Dispose()
        {
            _badgeNameToolTip.Dispose();
            base.Dispose();
        }
    }
}
