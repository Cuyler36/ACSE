using System;
using System.IO;
using ACSE.Core.Items;
using ACSE.Core.Saves;
using ACSE.Core.Town.Acres;

namespace ACSE.WinForms.Utilities
{
    public static class DataUtility
    {
        // Export/Import Methods
        public static void ExportAcres(WorldAcre[] acres, SaveGeneration saveGeneration, string saveFileName)
        {
            using (var saveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveDialog.Filter = "ACSE Acre Save (*.aas)|*.aas";
                saveDialog.FileName = saveFileName + " Acre Data.aas";

                if (saveDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(new byte[] { 0x41, 0x41, 0x53 }); // "AAS" Identifier
                            writer.Write((byte)acres.Length); // Total Acre Count
                            writer.Write((byte)saveGeneration); // Save Generation
                            writer.Write(new byte[] { 0, 0, 0 }); // Padding
                            foreach (var t in acres)
                            {
                                writer.Write(BitConverter.GetBytes(t.AcreId));
                            }

                            writer.Flush();
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Acre exportation failed!", "Acre Export Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        public static void ImportAcres(ref WorldAcre[] acres, SaveGeneration saveGeneration)
        {
            using (var openDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openDialog.Filter = "ACSE Acre Save (*.aas)|*.aas";
                openDialog.FileName = "";

                if (openDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            if (!System.Text.Encoding.ASCII.GetString(reader.ReadBytes(3)).Equals("AAS") ||
                                reader.ReadByte() != acres.Length ||
                                (SaveGeneration)reader.ReadByte() != saveGeneration) return;
                            reader.BaseStream.Seek(8, SeekOrigin.Begin);
                            foreach (var t in acres)
                            {
                                t.AcreId = reader.ReadUInt16();
                                t.BaseAcreId = (ushort)(t.AcreId & 0xFFFC);
                            }
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Acre importation failed!", "Acre Import Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        public static void ExportTown(WorldAcre[] acres, SaveGeneration saveGeneration, string saveFileName)
        {
            using (var saveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveDialog.Filter = "ACSE Town Save (*.ats)|*.ats";
                saveDialog.FileName = saveFileName + " Town Data.ats";

                if (saveDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(new byte[] { 0x41, 0x54, 0x53 }); // "ATS" Identifier
                            writer.Write((byte)acres.Length); // Total Acre Count
                            writer.Write((byte)saveGeneration); // Save Generation
                            writer.Write(new byte[] { 0, 0, 0 }); // Padding

                            if (saveGeneration == SaveGeneration.N3DS)
                            {
                                foreach (var acre in acres)
                                {
                                    foreach (var item in acre.AcreItems)
                                    {
                                        writer.Write(BitConverter.GetBytes(item.ToUInt32()));
                                    }
                                }
                            }
                            else
                            {
                                foreach (var acre in acres)
                                {
                                    foreach (var item in acre.AcreItems)
                                    {
                                        writer.Write(BitConverter.GetBytes(item.ItemId));
                                    }
                                }
                            }

                            writer.Flush();
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Town exportation failed!", "Town Export Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        public static void ImportTown(ref WorldAcre[] acres, SaveGeneration saveGeneration)
        {
            using (var openDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openDialog.Filter = "ACSE Town Save (*.ats)|*.ats";
                openDialog.FileName = "";

                if (openDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            if (!System.Text.Encoding.ASCII.GetString(reader.ReadBytes(3)).Equals("ATS") ||
                                reader.ReadByte() != acres.Length ||
                                (SaveGeneration)reader.ReadByte() != saveGeneration) return;
                            reader.BaseStream.Seek(8, SeekOrigin.Begin);
                            if (saveGeneration == SaveGeneration.N3DS)
                            {
                                foreach (var acre in acres)
                                {
                                    for (var x = 0; x < acre.AcreItems.Length; x++)
                                    {
                                        acre.AcreItems[x] = new WorldItem(reader.ReadUInt32(), acre.AcreItems[x].Index);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var acre in acres)
                                {
                                    for (var x = 0; x < acre.AcreItems.Length; x++)
                                    {
                                        acre.AcreItems[x] = new WorldItem(reader.ReadUInt16(), acre.AcreItems[x].Index);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Acre importation failed!", "Acre Import Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
    }
}
