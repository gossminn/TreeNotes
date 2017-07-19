using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace TreeNotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TreeEditor _editor;

        public MainPage()
        {
            // Initialize component
            InitializeComponent();

            // Tree editor
            _editor = new TreeEditor();

            // Initialize UI
            DisplayEditorLocation();

            // Events
            KeyDown += ProcessKeys;
            TitleBox.TextChanged += ProcessTitle;
            EditorBox.TextChanged += ProcessText;
            CommandBox.TextChanged += ProcessCommandAsync;
        }

        private void ProcessKeys(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Control:
                    CommandBox.Focus(FocusState.Programmatic);
                    break;

                default:
                    break;
            }
        }

        private void ProcessTitle(object sender, TextChangedEventArgs e)
        {
            _editor.SetTitle(TitleBox.Text);
            DisplayEditorLocation();
        }

        private void ProcessText(object sender, TextChangedEventArgs e)
        {
            _editor.SetContents(EditorBox.Text);
            DisplayEditorLocation();
        }


        private void DisplayEditorLocation()
        {
            PathBlock.Text = _editor.FormatPath();
            ChildBlock.Text = _editor.FormatChildren();
        }

        private void ClearBuffer()
        {
            TitleBox.Text = EditorBox.Text = "";
        }

        private void FillBuffer()
        {
            TitleBox.Text = _editor.GetTitle();
            EditorBox.Text = _editor.GetContents();
        }

        private async void ProcessCommandAsync(object sender, TextChangedEventArgs e)
        {
            switch (CommandBox.Text)
            {
                // S: save
                case "S":
                    _editor.WriteToDiscAsync();
                    break;

                // O: open
                case "O":
                    await _editor.OpenFromDiscAsync();
                    FillBuffer();
                    CommandBox.Text = "";
                    break;

                // Z: undo
                case "Z":
                    _editor.Undo();
                    FillBuffer();
                    CommandBox.Text = "";
                    break;

                // c: new child
                case "c":
                    _editor.CreateNode();
                    ClearBuffer();
                    TitleBox.Focus(FocusState.Programmatic);
                    CommandBox.Text = "";
                    break;

                // s: new sibling
                case "s":
                    _editor.LevelUp();
                    _editor.CreateNode();
                    ClearBuffer();
                    TitleBox.Focus(FocusState.Programmatic);
                    CommandBox.Text = "";
                    break;

                // p: new parent
                case "p":
                    _editor.ReplaceParent();
                    ClearBuffer();
                    TitleBox.Focus(FocusState.Programmatic);
                    CommandBox.Text = "";
                    break;

                // u: up
                case "u":
                    _editor.LevelUp();
                    FillBuffer();
                    EditorBox.Focus(FocusState.Programmatic);
                    CommandBox.Text = "";
                    break;

                // r: remove current
                case "r":
                    if (_editor.RemoveChild())
                    {
                        FillBuffer();
                        EditorBox.Focus(FocusState.Programmatic);
                    }
                    CommandBox.Text = "";
                    break;

                default:
                    // Multi-word commands
                    if (CommandBox.Text.StartsWith("d") && CommandBox.Text.EndsWith(" "))
                    {
                        var downArgs = CommandBox.Text.Split();
                        if (downArgs.Length > 1 && int.TryParse(downArgs[1], out int res))
                        {
                            _editor.LevelDown(res);
                            FillBuffer();
                            EditorBox.Focus(FocusState.Programmatic);
                            CommandBox.Text = "";
                        }
                    }
                    break;
            }

            DisplayEditorLocation();
        }
    }
}
