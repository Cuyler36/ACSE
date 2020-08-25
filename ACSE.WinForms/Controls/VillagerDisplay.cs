using System;
using System.Drawing;
using System.Windows.Forms;
using ACSE.Core.Villagers;

namespace ACSE.WinForms.Controls
{
    public partial class VillagerDisplay : UserControl
    {
        private static Image villagerImage;
        public VillagerDisplay()
        {
            InitializeComponent();
        }

        public VillagerDisplay(SimpleVillager villager)
        {
            InitializeComponent();
            if (villagerImage == null)
                villagerImage = Image.FromFile("Resources/Images/Villagers.jpg");
            RestoreFromVillager(villager);
        }

        public SimpleVillager Villager { get; internal set; }

        public static VillagerDisplay CreateFromVillagerObject(SimpleVillager villager)
        {
            return new VillagerDisplay(villager);
        }

        public void RestoreFromVillager(SimpleVillager villager)
        {
            villagerPreview.Image = villagerImage;
            villagerPreview.Offset =
                    (villager.VillagerId < 0xE000 || villager.VillagerId > 0xE0EB)
                        ? new Point(64 * 6, 64 * 23)
                        : new Point(64 * ((villager.VillagerId & 0xFF) % 10),
                            64 * ((villager.VillagerId & 0xFF) / 10));
            nameLabel.Text = villager.Name;            
            Villager = villager;
        }

        private void wikiButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://nookipedia.com/wiki/{Villager.Name}");          
        }

        private void nameLabel_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void villagerPreview_Click_1(object sender, EventArgs e)
        {
            OnClick(e);
        }
    }
}
