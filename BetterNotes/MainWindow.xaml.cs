using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System;
using System.Windows.Documents;

namespace BetterNotes
{
    public partial class MainWindow : Window
    {
        private string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string currentFilePath;
        private bool isUnsaved = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;

            currentFilePath = Path.Combine(documentsFolder, "untitled.txt");

            if (File.Exists(currentFilePath))
            {
                LoadFile(currentFilePath);
            }
        }

        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, NoteTextBox);
        }

        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, NoteTextBox);
        }

        private void Underline_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, NoteTextBox);
        }

        private void NoteTextBox_TextChanged(object sender, RoutedEventArgs e)
        {

            TextRange textRange = new TextRange(NoteTextBox.Document.ContentStart, NoteTextBox.Document.ContentEnd);

            if (textRange.Text == null)
            {
                if (CharCountText != null)
                {
                    CharCountText.Text = "Characters: 0";
                }
                return;
            }

            if (CharCountText != null)
            {
                CharCountText.Text = $"Characters: {textRange.Text.Trim().Length}";
            }

            isUnsaved = true;
            if (SaveStatusText != null)
            {
                SaveStatusText.Text = "Unsaved Changes";
            }
        }


        private void NoteTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int index = new TextRange(NoteTextBox.Document.ContentStart, NoteTextBox.CaretPosition).Text.Length;
            int line = NoteTextBox.CaretPosition.GetLineStartPosition(0) == null ? 1 : 2;
            int col = index;

            LineColText.Text = $"Ln {line}, Col {col}";
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath) || currentFilePath == Path.Combine(documentsFolder, "untitled.rtf"))
            {
                SaveAs_Click(sender, e);
            }
            else
            {
                SaveFile(currentFilePath);
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = currentFilePath,
                InitialDirectory = documentsFolder
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                SaveFile(currentFilePath);
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Open File",
                InitialDirectory = documentsFolder
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentFilePath = openFileDialog.FileName;
                LoadFile(currentFilePath);
            }
        }

        private void SaveFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                TextRange range = new TextRange(NoteTextBox.Document.ContentStart, NoteTextBox.Document.ContentEnd);
                range.Save(fs, System.Windows.DataFormats.Rtf);
            }
            isUnsaved = false;
            SaveStatusText.Text = "Saved";
        }

        private void LoadFile(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    TextRange range = new TextRange(NoteTextBox.Document.ContentStart, NoteTextBox.Document.ContentEnd);
                    range.Load(fs, System.Windows.DataFormats.Rtf);
                }
            }
            catch (ArgumentException)
            {
                System.Windows.MessageBox.Show("The file is not in a valid RTF format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isUnsaved)
            {
                var result = System.Windows.MessageBox.Show("You have unsaved changes. Do you want to save them before exiting?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile(currentFilePath);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
