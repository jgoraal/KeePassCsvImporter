using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.Plugins;

namespace KeePassCsvImporter
{
    internal sealed class CsvReader
    {
        private readonly IPluginHost _host;
        private string Path { get; set; }

        private readonly List<DataParser> _parser = new List<DataParser>();

        public CsvReader(IPluginHost host)
        {
            _host = host;
        }

        internal async Task Import()
        {
            if (!OpenCsvFile())
                return;


            try
            {
                using (var reader = new StreamReader(Path))
                {
                    var firstLine = await reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(firstLine))
                        throw new ApplicationException(ErrorMessages.ReadError);

                    var replaceFirst = firstLine.Replace("\"", "");
                    var tags = new List<string>(replaceFirst.Split(';'));
                    if (!tags.Any() || !HasFileCorrectTemplate(tags) || tags.Count != 12)
                        throw new ApplicationException(ErrorMessages.CsvDataError);

                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();

                        if (string.IsNullOrEmpty(line)) throw new ApplicationException(ErrorMessages.EmptyRecord);
                        var replace = line.Replace("\"", "");
                        _parser.Add(new DataParser(new List<string>(replace.Split(';'))));
                    }
                }

                ParseData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas czytania pliku: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool OpenCsvFile()
        {
            using (var file = new OpenFileDialog())
            {
                file.CheckFileExists = true;
                file.Filter = "Csv Files (*.csv)|*.csv";
                file.Title = "Otwórz plik csv z danymi do importu";

                if (file.ShowDialog() == DialogResult.OK)
                {
                    Path = file.FileName;
                    return true;
                }
            }

            return false;
        }

        private bool HasFileCorrectTemplate(List<string> tags)
        {
            return tags[0].Equals("Group") && tags[1].Equals("Title") && tags[2].Equals("Username") &&
                   tags[3].Equals("Password") && tags[4].Equals("URL") && tags[5].Equals("Notes") &&
                   tags[6].Equals("Model") && tags[7].Equals("S\\N") && tags[8].Equals("OS") &&
                   tags[9].Equals("CPU") && tags[10].Equals("RAM") && tags[11].Equals("HDD/SSD");
        }

        private void ParseData()
        {
            MessageCreator.CreateQuestionMessage("Kontynuwać?", $"Znaleziono {_parser.Count} wpisy");
            
            foreach (var data in _parser)
            {
                data.Parse(_host.Database.RootGroup);
            }

            _host.MainWindow.UpdateUI(false, null, true, null, true, null, true);
            _host.MainWindow.RefreshEntriesList();
        }
    }
}