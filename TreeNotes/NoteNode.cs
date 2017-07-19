using System.Collections.Generic;
using System.Linq;

namespace TreeNotes
{
    class NoteNode
    {
        public string Title { get; set; }
        public string Contents { get; set; }
        public NoteNode Parent { get; set; }
        public List<NoteNode> Children { get; private set; }

        public NoteNode(NoteNode parent)
        {
            Title = "UNTITLED";
            Contents = "";
            Children = new List<NoteNode>();
            Parent = parent;
        }

        public NoteNode AddChild()
        {
            var child = new NoteNode(this);
            Children.Add(child);
            return child;
        }

        public NoteNode Copy()
        {
            var copy = new NoteNode(Parent)
            {
                Title = Title,
                Contents = Contents,
                Children = new List<NoteNode>(Children)
            };
            return copy;
        }

        public bool Find(NoteNode node)
        {
            return this == node || Children.Any(x => x.Find(node));
        }
    }
}
