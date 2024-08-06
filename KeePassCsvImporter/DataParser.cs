using System.Collections.Generic;
using System.Linq;
using KeePassLib;
using KeePassLib.Security;

namespace KeePassCsvImporter
{
    internal sealed class DataParser
    {
        private string GroupName { get; set; }
        private string SubGroupName { get; set; }
        private string Title { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string Url { get; set; }
        private string Notes { get; set; }
        private string Model { get; set; }
        private string Serial { get; set; }
        private string Os { get; set; }
        private string Cpu { get; set; }
        private string Ram { get; set; }
        private string Memory { get; set; }

        internal DataParser(List<string> entryData)
        {
            GroupName = entryData[0].Split('/')[0];
            SubGroupName = entryData[0].Substring(entryData[0].IndexOf('/') + 1);
            Title = entryData[1];
            Username = entryData[2];
            Password = entryData[3];
            Url = entryData[4];
            Notes = entryData[5];
            Model = entryData[6];
            Serial = entryData[7];
            Os = entryData[8];
            Cpu = entryData[9];
            Ram = entryData[10];
            Memory = entryData[11];
        }

        internal void Parse(PwGroup root)
        {
            var group = root.Groups.FirstOrDefault(g => g.Name == GroupName);
            
            if (group == null)
            {
                group = new PwGroup(true, true, GroupName, PwIcon.Folder);
                root.AddGroup(group, true);
            }
            
            var subgroup = group.Groups.FirstOrDefault(g => g.Name == SubGroupName);
            
            if (subgroup == null)
            {
                subgroup = new PwGroup(true, true, SubGroupName, PwIcon.Folder);
                group.AddGroup(subgroup, true);
            }
            
            // Pierwszy wpis z polami w zakładce Advanced
            var firstEntry = new PwEntry(true, true);
            firstEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, Title));
            
            firstEntry.Strings.Set("CPU", new ProtectedString(false, Cpu));
            firstEntry.Strings.Set("HDD/SDD", new ProtectedString(false, Memory));
            firstEntry.Strings.Set("Model", new ProtectedString(false, Model));
            firstEntry.Strings.Set("OS", new ProtectedString(false, Os));
            firstEntry.Strings.Set("RAM", new ProtectedString(false, Ram));
            firstEntry.Strings.Set("S/N", new ProtectedString(false, Serial));
            
            // Drugi wpis z Username: full username
            var secondEntry = new PwEntry(true, true);
            secondEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, "Username"));
            secondEntry.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, Username));
            
            // Trzeci wpis z Url: adres IP
            var thirdEntry = new PwEntry(true, true);
            thirdEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, "URL"));
            thirdEntry.Strings.Set(PwDefs.UrlField, new ProtectedString(false, Url));
            
            subgroup.AddEntry(firstEntry,true);
            subgroup.AddEntry(secondEntry,true);
            subgroup.AddEntry(thirdEntry,true);
        }

        
    }
}