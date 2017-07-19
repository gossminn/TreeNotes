using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace TreeNotes
{
    internal class TreeReader
    {
        private string _raw;
        private int _position;
        private NoteNode _tree, _level;
        
        public TreeReader()
        {
            _raw = "";
            _position = 0;
            _tree = _level = null;
        }

        public async Task OpenFromDiscAsync()
        {
            var picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
            };
            picker.FileTypeFilter.Add(".xml");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenReadAsync();
                var read = await FileIO.ReadTextAsync(file);
                _raw = read;
                stream.Dispose();
            }
        }

        public NoteNode ParseXML()
        {
            if (ParseOpenTag() == "node")
            {
                ParseNode();
                ParseCloseTag();
            }
            else
            {
                _tree = _level = null;
            }
            return _tree;
        }

        private void ParseNode()
        {
            // Root note
            if (_tree == null && _level == null)
            {
                _tree = _level = new NoteNode(null);
            }
            else
            {
                _level = _level.AddChild();
            }

            // <title> section
            if (ParseOpenTag() != "title")
            {
                _tree = _level = null;
                return;
            }
            ParseTitle();
            ParseCloseTag();

            // <content> section
            if (ParseOpenTag() != "content")
            {
                _tree = _level = null;
                return;
            }
            ParseContent();
            ParseCloseTag();

            // <children> section
            if (ParseOpenTag() != "children")
            {
                _tree = _level = null;
                return;
            }
            ParseChildren();
            ParseCloseTag();

            // Level up
            if (_level.Parent != null)
            {
                _level = _level.Parent;
            }
        }

        // Parse string between open and closing tags
        private string ParseInnerXML()
        {
            var result = "";
            while (_raw[_position] != '<')
            {
                result += _raw[_position];
                _position++;
            }
            return result;
        }

        // Parse title
        private void ParseTitle()
        {
            _level.Title = ParseInnerXML();
        }

        // Parse contents
        private void ParseContent()
        {
            _level.Contents = ParseInnerXML();
        }

        // Parse child nodes
        private void ParseChildren()
        {
            while (ParseOpenTag() == "node")
            {
                ParseNode();
                ParseCloseTag();
            }
        }

        // Skip past closing tag
        private void ParseCloseTag()
        {
            while (_raw[_position] != '>')
            {
                _position++;
            }
            _position++;
        }

        // Move past opening tag and return the tag
        private string ParseOpenTag()
        {
            var tag = "";
            if (_raw[_position] == '<')
            {
                _position++;
                while (_raw[_position] != '>')
                {
                    tag += _raw[_position];
                    _position++;
                }
                _position++;
            }
            return tag;
        }
    }
}