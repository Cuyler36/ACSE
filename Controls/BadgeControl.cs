using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACSE
{
    class BadgeControl : OffsetablePictureBox
    {
        private static string[] BadgeNames = new string[]
        {
            "Fish Caught", "Insects Caught", "Marine Creatures Caught", "Fish Collection", "Insect Collection", "Marine Collection", "Balloons",
            "Towns Visited", "Town Visits", "Watering Flowers", "Savings", "Turnips", "Medals", "StreetPass", "Weeds Removed", "Shopping", "Letters",
            "Refurbishments", "Catalog", "K.K. Slider Listens", "HRA Points", "Time Played", "Helping Neighbors", "Dream Suite"
        };

        private static string[] BadgeLevels = new string[4] { "None", "Bronze", "Silver", "Gold" };

        private static int[][] BadgeValues = new int[][]
        {
            new int[] { 0, 500, 2000, 5000 },
            new int[] { 0, 500, 2000, 5000 },
            new int[] { 0, 100, 200, 1000 },
            new int[] { 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0 },
            new int[] { 0, 30, 100, 200 },
            new int[] { 0, 100, 250, 500 },
            new int[] { 0, 50, 200, 500 },
            new int[] { 0, 100, 250, 500 },
            new int[] { 0, 0, 0, 0 },
            new int[] { 0, 500000, 3000000, 10000000 },
            new int[] { 0, 300, 1500, 5000 },
            new int[] { 0, 100, 300, 1000 },
            new int[] { 0, 500, 2000, 5000 },
            new int[] { 0, 500000, 2000000, 5000000 },
            new int[] { 0, 50, 100, 200 },
            new int[] { 0, 30, 100, 200 },
            new int[] { 0, 0, 0, 0 },
            new int[] { 0, 20, 50, 100 },
            new int[] { 0, 0, 0, 0 },
            new int[] { 0, 0, 0, 0 },
            new int[] { 0, 50, 100, 300 },
            new int[] { 0, 50, 200, 500 }
        };

        internal bool Loaded = false;
        readonly string BadgeName;
        readonly int Index = -1;
        readonly int DataOffset = -1;
        readonly int ValueOffset = -1;
        readonly ToolTip BadgeNameToolTip;
        public byte Stage = 0;
        public NL_Int32 Value;

        private Save SaveFile;

        public BadgeControl(Save save, int index, int offset, int valueOffset) : base()
        {
            if (index >= BadgeNames.Length)
            {
                throw new Exception($"Badge index was invalid! Got {index} when {BadgeNames.Length - 1} was the maximum possible!");
            }

            SaveFile = save;
            Index = index;
            DataOffset = offset;
            ValueOffset = valueOffset;
            Stage = SaveFile.ReadByte(offset);
            Value = new NL_Int32(SaveFile.ReadUInt32(valueOffset), SaveFile.ReadUInt32(valueOffset + 4));
            BadgeName = BadgeNames[index];

            BadgeNameToolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 100,
                ReshowDelay = 500,
                ShowAlways = true
            };

            BadgeNameToolTip.SetToolTip(pictureBox, BadgeName + $" - [{BadgeLevels[Stage]}] - " + Value.Value);

            Size = new Size(28, 28);
            ImageMaskingType = MaskingType.None;
            SetImageCrop();
            pictureBox.MouseClick += OnClick;

            Loaded = true;
        }

        private void SetImageCrop()
        {
            if (Index > -1 && Index < BadgeNames.Length && Stage < 4)
            {
                if (Stage == 0)
                {
                    Image = Properties.Resources.Animal_Crossing_NL_NoBadge_28x28;
                    Offset = new Point();
                }
                else
                {
                    Image = Properties.Resources.Animal_Crossing_NL_Badges_28x28;
                    Offset = new Point((Stage - 1) * 28, Index * 28);
                }
            }
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (Loaded && DataOffset > -1 && ValueOffset > -1 && Index > -1 && Index < BadgeNames.Length)
            {
                Stage++;
                if (Stage > 3)
                {
                    Stage = 0;
                }

                Value = new NL_Int32((uint)BadgeValues[Index][Stage]);

                if (SaveFile != null)
                {
                    SaveFile.Write(DataOffset, Stage);
                    SaveFile.Write(ValueOffset, Value.Int_1);
                    SaveFile.Write(ValueOffset + 4, Value.Int_2);
                }

                SetImageCrop();
                BadgeNameToolTip.SetToolTip(pictureBox, BadgeName + $" - [{BadgeLevels[Stage]}] - " + Value.Value);
            }
        }

        public new void Dispose()
        {
            BadgeNameToolTip.Dispose();
            base.Dispose();
        }
    }
}
