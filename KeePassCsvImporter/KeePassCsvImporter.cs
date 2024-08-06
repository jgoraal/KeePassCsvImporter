using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeePassCsvImporter
{
    public sealed class KeePassCsvImporterExt : Plugin
    {
        private IPluginHost Host { get; set; }

        public override bool Initialize(IPluginHost host)
        {
            if (host == null)
            {
                MessageCreator.CreateErrorMessage(ErrorMessages.HostError);
                return false;
            }

            Host = host;

            var tsMenuItem = new ToolStripMenuItem("Importuj dane z pliku .csv");
            tsMenuItem.Click += async (sender, e) => await ToolsMenuItemClickAsync();
            
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("KeePassCsvImporter.Resources.import_icon.png"))
            {
                if (stream != null)
                {
                    tsMenuItem.Image = Image.FromStream(stream);
                }
            }
            
            foreach (ToolStripMenuItem menuItem in host.MainWindow.MainMenuStrip.Items)
            {
                if (menuItem.Text == "&File")
                {
                    int importIndex = -1;
                    int exportIndex = -1;
                    for (int i = 0; i < menuItem.DropDownItems.Count; i++)
                    {
                        switch (menuItem.DropDownItems[i].Text)
                        {
                            case "&Import...":
                                importIndex = i;
                                break;
                            case "&Export...":
                                exportIndex = i;
                                break;
                        }
                    }
                    
                    if (importIndex != -1 && exportIndex != -1 && importIndex < exportIndex)
                    {
                        menuItem.DropDownItems.Insert(exportIndex, tsMenuItem);
                    }
                    else
                    {
                        menuItem.DropDownItems.Add(tsMenuItem);
                    }

                    break;
                }
            }
            

            return true;
        }

        
        public override void Terminate()
        {
        }
        

        private async Task ToolsMenuItemClickAsync()
        {
            try
            {
                var importer = new CsvReader(Host);
                await importer.Import();
            }
            catch (ArgumentNullException nullException)
            {
                MessageCreator.CreateErrorMessage(nullException.Message);
            }
            catch (ApplicationException applicationException)
            {
                MessageCreator.CreateErrorMessage(applicationException.Message);
            }
            catch (Exception others)
            {
                MessageCreator.CreateErrorMessage(others.Message);
            }
        }
    }
}