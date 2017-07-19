using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TreeNotes
{
    class TreeEditor
    {
        private NoteNode _tree, _level;
        private Stack<NoteNode> _treeHistory;

        // Constructor
        public TreeEditor()
        {
            _tree = _level = new NoteNode(null);
            _treeHistory = new Stack<NoteNode>();
        }

        // Edit operations
        public void CreateNode()
        {
            SaveHistory();
            _level = _level.AddChild();
        }

        public bool RemoveChild()
        {
            SaveHistory();
            if (_level.Parent != null)
            {
                // Remove current level from parent's children
                _level.Parent.Children.Remove(_level);

                // Current level's children are added to parent's children
                foreach (var child in _level.Children)
                {
                    _level.Parent.Children.Add(child);
                }

                // Switch current layer to parent
                _level = _level.Parent;

                return true;
            }

            // Return value indicates whether removal was succesful
            return false;
        }
        public void ReplaceParent()
        {
            SaveHistory();
            NoteNode newParent;
            if (_level.Parent != null)
            {
                _level.Parent.Children.Remove(_level);
                newParent = _level.Parent.AddChild();
            }
            else
            {
                newParent = new NoteNode(null);
                _tree = newParent;
            }
            newParent.Children.Add(_level);
            _level.Parent = newParent;
            _level = newParent;
        }

        // Navigation
        public void LevelUp()
        {
            if (_level.Parent != null)
            {
                _level = _level.Parent;
            }
        }

        public void LevelDown(int res)
        {
            res--;
            if (res < _level.Children.Count && res >= 0)
            {
                _level = _level.Children[res];
            }
        }

        // Formatting
        public string FormatPath()
        {
            var result = "";
            for (var current = _level; current != null; current = current.Parent)
            {
                result = $"> {current.Title} " + result;
            }
            return result;
        }

        public string FormatChildren()
        {
            return _level.Children.Count > 0
                ? $"{_level.Children.Count} sub(s): " + string.Join(" - ", _level.Children.Select(c => c.Title))
                : "No subs yet";
        }

        // Getters/setters for _level
        public void SetTitle(string title)
        {
            _level.Title = title;
        }

        public void SetContents(string contents)
        {
            _level.Contents = contents;
        }

        public string GetTitle()
        {
            return _level.Title;
        }

        public string GetContents()
        {
            return _level.Contents;
        }

        // Read/write operations
        public async void WriteToDiscAsync()
        {
            var writer = new TreeWriter();
            writer.MakeXML(_tree);
            await writer.SaveAsync();
        }

        public async Task OpenFromDiscAsync()
        {
            var reader = new TreeReader();
            await reader.OpenFromDiscAsync();
            _tree = _level = reader.ParseXML();
        }

        // For undo functionality
        private void SaveHistory()
        {
            _treeHistory.Push(_tree.Copy());
        }

        internal void Undo()
        {
            _tree = _treeHistory.Pop();
            if (!_tree.Find(_level))
            {
                _level = _tree;
            }
        }
    }
}
