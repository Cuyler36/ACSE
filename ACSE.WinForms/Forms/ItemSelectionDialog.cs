using ACSE.Core.Items;
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
    /// A gallery of all the selectable items in the game
    /// </summary>
    public partial class ItemSelectionDialog : Form
    {
        public enum ItemCategory
        {
            Buildings,
            Furniture,
            Gyroids,
            WallpaperFloor,
            Nature,
            Clothes,
            Catchable,
            Items,
            Misc,
            System,
        }

        public Item SelectedItem
        {
            get; set;
        } = new Item(0);

        List<KeyValuePair<ItemCategory, Item>> CategoryItems = new List<KeyValuePair<ItemCategory, Item>>();
        List<KeyValuePair<string, Item>> ItemCache = new List<KeyValuePair<string, Item>>();
        string[] categoryNames =
        {
            null, null, null, "Wallpapers/Flooring", null, "Clothing", null, null, null, null
        };

        public ItemSelectionDialog(List<KeyValuePair<ushort, string>> database)
        {
            InitializeComponent();
            foreach (var item in database)
            {
                var _item = new Item(item.Key);
                CategoryItems.Add(new KeyValuePair<ItemCategory, Item>(GetCategory(_item.Type), _item));
                ItemCache.Add(new KeyValuePair<string, Item>(_item.Name, _item));
            }
            Populate();
            Display();
        }

        private void Display(int page = 0)
        {
            var tab = categorySwitcher.SelectedTab;
            tab.Controls.DisposeChildren();
            var category = (ItemCategory)tab.Tag;
            var flowLayout = new FlowLayoutPanel()
            {
                BackColor = Color.White,
                WrapContents = true,
                AutoScroll = true,
                Dock = DockStyle.Fill,
            };
            EventHandler d = null;
            d = delegate
            {
                DisplaySource(flowLayout, CategoryItems.Where(x => x.Key == category).Select(x => x.Value), page);
                flowLayout.HandleCreated -= d;
            };
            flowLayout.HandleCreated += d;
            tab.Controls.Add(flowLayout);
        }

        private void DisplaySource(Panel Destination, IEnumerable<Item> source, int page = 0)
        {
            Panel flowLayout = Destination;            
            int sourceCount = 0;
            pageFlow.Enabled = false;
            loadingPanel.Visible = true;
            loadingProgress.Value = 0;
            loadingProgress.Maximum = 200;     
            Destination.Controls.DisposeChildren();       
            Task.Run(() =>
            {                
                sourceCount = source.Count();
                foreach (var item in source.Skip(page * 200).Take(200))
                {
                    ItemDisplay display = null;
                    Action a = delegate
                    {
                        display = new ItemDisplay(item);
                        display.Click += ItemSelected;
                        flowLayout.Controls.Add(display);
                        loadingProgress.Value++;
                    };
                    if (!flowLayout.IsDisposed)
                    {
                        if (flowLayout.InvokeRequired)
                            flowLayout.Invoke(a);
                        else
                            a.Invoke();
                    }
                }
            }).ContinueWith(delegate
            {
                pageFlow.Invoke((Action)delegate
                {                    
                    int pages = (sourceCount / 200) + 1;
                    pageFlow.Controls.DisposeChildren();
                    Label label = new Label()
                    {
                        Text = sourceCount + " Items. Page: (" + (page + 1) + "/" + pages + ")",
                        Margin = new Padding(20, 5, 10, 0),
                        AutoSize = true
                    };
                    pageFlow.Controls.Add(label);
                    if (pages > 1)
                    {
                        for (int i = 0; i < pages; i++)
                        {
                            Button button = new Button()
                            {
                                Text = (i + 1).ToString(),
                                Tag = i
                            };
                            button.Enabled = i != page;
                            button.Click += (object sender, EventArgs e) =>
                            {
                                pageFlow.Enabled = false;
                                DisplaySource(Destination, source, (int)(sender as Button).Tag);
                            };
                            pageFlow.Controls.Add(button);
                        }
                    }
                    pageFlow.Enabled = true;
                    loadingPanel.Visible = false;
                });
            });
        }

        private void ItemSelected(object sender, EventArgs e)
        {
            SelectedItem = (sender as ItemDisplay).Item;
            selectionLabel.Text = SelectedItem.Name;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Populate()
        {
            var enumNames = Enum.GetNames(typeof(ItemCategory));
            for (int i = 0; i < categoryNames.Length; i++)
            {
                string name = categoryNames[i];
                if (categoryNames[i] == null)
                    name = enumNames[i];
                categorySwitcher.TabPages.Add(new TabPage(name)
                {
                    Tag = (ItemCategory)i
                });
            }
        }

        private ItemCategory GetCategory(ItemType type)
        {
            switch (type)
            {
                case ItemType.Building:
                case ItemType.Signboard:
                    return ItemCategory.Buildings;
                case ItemType.Catchable:
                    return ItemCategory.Catchable;
                case ItemType.Clothes:
                    return ItemCategory.Clothes;
                case ItemType.Empty:
                    return ItemCategory.System;
                case ItemType.Fossil:
                case ItemType.Fruit:
                case ItemType.ParchedFlower:
                case ItemType.Rock:
                case ItemType.Tree:
                case ItemType.WiltedFlower:
                case ItemType.Weed:
                case ItemType.WateredFlower:
                case ItemType.MoneyRock:
                case ItemType.Flower:
                case ItemType.Shell:
                    return ItemCategory.Nature;
                case ItemType.Money:
                case ItemType.Tool:
                case ItemType.Item:
                case ItemType.QuestItem:
                case ItemType.RaffleTicket:
                case ItemType.Turnip:
                case ItemType.Paper:
                case ItemType.Diary:
                    return ItemCategory.Items;
                case ItemType.HouseObject:
                case ItemType.Furniture:
                case ItemType.Trash:
                    return ItemCategory.Furniture;
                case ItemType.Gyroid:
                    return ItemCategory.Gyroids;
                case ItemType.WallpaperCarpet:
                    return ItemCategory.WallpaperFloor;
                case ItemType.Song:
                case ItemType.Pattern:
                default:
                    return ItemCategory.Misc;
            }
        }

        private void categorySwitcher_TabIndexChanged(object sender, EventArgs e)
        {
            Display();
        }

        private void categorySwitcher_Selected(object sender, TabControlEventArgs e)
        {
            Display();
        }

        private void Search()
        {
            IEnumerable<Item> query = null;
            string searchTerm = searchBox.Text;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Display();
                return;
            }
            Task.Run(delegate
            {
                query = ItemCache.Where(x => x.Key.Contains(searchTerm)).Select(x => x.Value);
            }).ContinueWith(delegate
            {
                Invoke((Action)delegate
                {
                    var flowLayout = new FlowLayoutPanel()
                    {
                        BackColor = Color.White,
                        WrapContents = true,
                        AutoScroll = true,
                        Dock = DockStyle.Fill,
                    };
                    EventHandler handler = null;
                    handler = delegate
                    {
                        DisplaySource(flowLayout, query, 0);
                        categorySwitcher.SelectedTab.Controls.DisposeChildren();
                        categorySwitcher.SelectedTab.Controls.Add(flowLayout);
                        flowLayout.HandleCreated -= handler;
                    };
                    flowLayout.HandleCreated += handler;
                    _ = flowLayout.Handle; // force the handle to be created
                });
            });
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                Search();
        }
    }
}
