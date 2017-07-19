using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace TreeNotes
{
    class TreeWriter
    {
        private string _xmlString;

        public TreeWriter()
        {
            _xmlString = "";
        }

        public void MakeXML(NoteNode tree)
        {
            if (_xmlString != "")
            {
                _xmlString = "";
            }

            _xmlString = MakeTag(tree);
        }

        private string MakeTag(NoteNode node)
        {
            var result = "";
            result += $"<node><title>{node.Title}</title><content>{node.Contents}</content><children>";

            foreach (var child in node.Children)
            {
                result += MakeTag(child);
            }

            result += "</children></node>";
            return result;
        }

        public async Task SaveAsync()
        {
            var picker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SuggestedFileName = "Notes_" + DateTime.Now
            };
            picker.FileTypeChoices.Add("TreeNote XML Format", new List<string>() { ".xml" });

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, _xmlString);
            }
        }
    }
}