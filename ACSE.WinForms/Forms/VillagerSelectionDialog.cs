using ACSE.Core.Villagers;
using ACSE.WinForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACSE.WinForms.Forms
{
    /// <summary>
    /// A gallery of all the selectable Villagers in the game
    /// </summary>
    public partial class VillagerSelectionDialog : Form
    {
        /// <summary>
        /// The currently selected Villager
        /// </summary>
        public SimpleVillager SelectedVillager
        {
            get => _villager; 
            set
            {
                _villager = value;
                selectionLabel.Text = SelectedVillager.Name;
                SelectedIndex = VillagerIndices.First(x => x.Value == value).Key;
                selectedControl = DisplayCache[value.Name];
            }
        }
        public ushort SelectedIndex //the id of the selected Villager
        {
            get; private set;
        }
        /// <summary>
        /// The currently selected VillagerControl
        /// </summary>
        private VillagerDisplay selectedControl
        {
            get => _control;
            set
            {
                if (_control != null)
                    _control.BackColor = Color.White;
                value.BackColor = Color.LightBlue;
                _control = value;
            }
        }

        private Dictionary<string, VillagerDisplay> DisplayCache = new Dictionary<string, VillagerDisplay>();
        private Dictionary<ushort, SimpleVillager> VillagerIndices = new Dictionary<ushort, SimpleVillager>();
        private VillagerDisplay _control;
        private SimpleVillager _villager;

        public VillagerSelectionDialog(IEnumerable<SimpleVillager> DisplayVillagers)
        {
            InitializeComponent();
            ushort index = 0;
            foreach (var villager in DisplayVillagers)
            {
                if (villager.VillagerId == 0) { index++; continue; }
                VillagerIndices.Add(index, villager);
                index++;
            }
            Display();
        }

        private void Display()
        {
            villagerSelectionPanel.Controls.DisposeChildren();
            DisplayCache.Clear();
            foreach(var villager in VillagerIndices)
            {
                var display = new VillagerDisplay(villager.Value)
                {
                    Tag = villager.Key
                };
                display.Click += VillagerSelected;
                DisplayCache.Add(villager.Value.Name, display);
                villagerSelectionPanel.Controls.Add(display);
            }
        }

        private void VillagerSelected(object sender, EventArgs e)
        {
            VillagerDisplay selected = sender as VillagerDisplay;
            SelectedVillager = selected.Villager;                        
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) //search feature
        {
            villagerSelectionPanel.Controls.Clear();
            string searchterm = ((TextBox)sender).Text;
            IEnumerable<VillagerDisplay> result = DisplayCache.Values;
            if (!string.IsNullOrWhiteSpace(searchterm))
            {
                Task.Run(() => // async search so the program won't lock up on slower pcs
                { 
                    result = DisplayCache.Where(x => x.Key.Contains(searchterm)).Select(x => x.Value);
                    Action a = delegate
                    {
                        if (result.Any())
                            foreach (var display in result)
                                villagerSelectionPanel.Controls.Add(display);
                        else villagerSelectionPanel.Controls.Add(new Label()
                        {
                            Text = $"Couldn't find anything for: {searchterm}",
                            AutoSize = true,
                            Font = new Font(new FontFamily("Microsoft Sans Serif"), 12.0f, FontStyle.Regular),
                            Margin = new Padding(20)
                        });
                    };
                    Invoke(a); // thread-safe call
                });
            }
            else
            {
                foreach (var display in result)
                    villagerSelectionPanel.Controls.Add(display);
            }
        }
    }
}
