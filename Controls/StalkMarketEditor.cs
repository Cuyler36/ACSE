using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACSE
{
    internal sealed class StalkMarketEditor : FlowLayoutPanel
    {
        internal enum GameCubeStalkMarketTrend
        {
            Spike, Random, Falling
        }

        public int Days = 7;
        public int[] Prices;
        public int Trend;

        private readonly NumericTextBox[] _priceBoxes;
        private readonly ComboBox _trendComboBox;
        private readonly List<FlowLayoutPanel> _panels;

        private readonly Save _saveData;
        private readonly int _offset;
        private readonly int _trendOffset;

        private static readonly string[] DayNames = {
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
        };

        public StalkMarketEditor(Save saveData)
        {
            AutoSize = true;
            FlowDirection = FlowDirection.TopDown;

            _saveData = saveData;
            Prices = new int[Days];
            _priceBoxes = new NumericTextBox[Days];
            _panels = new List<FlowLayoutPanel>();

            _offset = _trendOffset = -1;
            switch (_saveData.SaveType)
            {
                case SaveType.AnimalCrossing:
                    _offset = saveData.SaveDataStartOffset + 0x20480;
                    _trendOffset = _offset + 0xE;
                    break;
                case SaveType.NewLeaf:
                    Days = 6;
                    _offset = saveData.SaveDataStartOffset + 0x6535C;
                    break;
                case SaveType.WelcomeAmiibo:
                    Days = 6;
                    _offset = saveData.SaveDataStartOffset + 0x6AD60;
                    break;
            }

            if (_offset < 0)
            {
                Dispose();
                return;
            }

            // Header Label
            Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Stalk Market"
            });

            // Trend
            if (_trendOffset > -1)
            {
                var trendPanel = new FlowLayoutPanel
                {
                    AutoSize = true,
                    FlowDirection = FlowDirection.LeftToRight
                };

                var trendLabel = new Label
                {
                    AutoSize = false,
                    Size = new Size(130, 22),
                    TextAlign = ContentAlignment.MiddleRight,
                    Text = "Trend:"
                };
                trendPanel.Controls.Add(trendLabel);

                Trend = saveData.ReadUInt16(_trendOffset, saveData.IsBigEndian);

                _trendComboBox = new ComboBox
                {
                    AutoSize = false,
                    Size = new Size(60, 20)
                };

                foreach (var trend in Enum.GetNames(typeof(GameCubeStalkMarketTrend)))
                {
                    _trendComboBox.Items.Add(trend);
                }

                _trendComboBox.SelectedIndex = Trend;
                _trendComboBox.SelectedIndexChanged += (s, e) => TrendChanged(_trendComboBox.SelectedIndex);
                trendPanel.Controls.Add(_trendComboBox);

                _panels.Add(trendPanel);
                Controls.Add(trendPanel);
            }

            // Prices
            var offset = _offset;
            for (var day = 0; day < Days; day++)
            {
                switch (_saveData.SaveGeneration)
                {
                    case SaveGeneration.N64:
                    case SaveGeneration.GCN:
                    case SaveGeneration.iQue:
                    {
                        Prices[day] = saveData.ReadUInt16(offset, saveData.IsBigEndian);
                        offset += 2;

                        var pricePanel = new FlowLayoutPanel
                        {
                            AutoSize = true,
                            FlowDirection = FlowDirection.LeftToRight
                        };

                        var priceLabel = new Label
                        {
                            AutoSize = false,
                            Size = new Size(130, 22),
                            TextAlign = ContentAlignment.MiddleRight,
                            Text = $"{DayNames[day]}'s Price:"
                        };
                        pricePanel.Controls.Add(priceLabel);

                        _priceBoxes[day] = new NumericTextBox
                        {
                            AutoSize = false,
                            Size = new Size(60, 20),
                            Location = new Point(5, day * 24),
                            Text = Prices[day].ToString(),
                            MaxLength = 4
                        };

                        var currentDay = day;
                        _priceBoxes[day].TextChanged += (s, e) =>
                            PriceChanged(currentDay, int.Parse(_priceBoxes[currentDay].Text));
                        pricePanel.Controls.Add(_priceBoxes[day]);
                        _panels.Add(pricePanel);
                        Controls.Add(pricePanel);
                        break;
                    }
                    case SaveGeneration.N3DS:
                    {
                        var amPrice = new NewLeafInt32(saveData.ReadUInt32(offset), saveData.ReadUInt32(offset + 4)).Value;
                        var pmPrice = new NewLeafInt32(saveData.ReadUInt32(offset + 8), saveData.ReadUInt32(offset + 0xC)).Value;
                        offset += 0x10;

                        for (var i = 0; i < 2; i++)
                        {
                            var pricePanel = new FlowLayoutPanel
                            {
                                AutoSize = true,
                                FlowDirection = FlowDirection.LeftToRight
                            };

                            var priceLabel = new Label
                            {
                                AutoSize = false,
                                Size = new Size(130, 22),
                                TextAlign = ContentAlignment.MiddleRight,
                                Text = $"{DayNames[day + 1]}'s Price [{(i == 0 ? "AM" : "PM")}]:"
                            };
                            pricePanel.Controls.Add(priceLabel);

                            var priceBox = new NumericTextBox
                            {
                                AutoSize = false,
                                Size = new Size(60, 20),
                                Location = new Point(5, day * 24),
                                Text = (i == 0 ? amPrice : pmPrice).ToString(),
                                MaxLength = 9
                            };

                            var currentDay = day;
                            priceBox.TextChanged += (s, e) =>
                                PriceChanged(currentDay, int.Parse(priceBox.Text));
                            pricePanel.Controls.Add(priceBox);
                            _panels.Add(pricePanel);
                            Controls.Add(pricePanel);
                        }

                        break;
                    }
                }
            }
        }

        private void PriceChanged(int index, int newPrice, bool pm = false)
        {
            if (_saveData == null) return;

            Prices[index] = newPrice;
            
            // Save price
            switch (_saveData.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    _saveData.Write(_offset + index * 2, (ushort) newPrice, _saveData.IsBigEndian);
                    break;
                case SaveGeneration.N3DS:
                    var writeOffset = _offset + index * 0x10 + (pm ? 8 : 0);
                    var encryptedValue = new NewLeafInt32((uint) newPrice);
                    _saveData.Write(writeOffset, encryptedValue.Int1);
                    _saveData.Write(writeOffset + 4, encryptedValue.Int2);
                    break;
            }
        }

        private void TrendChanged(int trend)
        {
            if (trend < 0) return;

            Trend = trend;

            switch (_saveData.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    _saveData.Write(_trendOffset, (ushort) trend, _saveData.IsBigEndian);
                    break;
            }
        }
    }
}
