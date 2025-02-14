using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System;

namespace BetterNotes
{
    public partial class MainWindow : Window
    {
        private readonly string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string currentFilePath;
        private bool isUnsaved = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;

            currentFilePath = Path.Combine(documentsFolder, "untitled.txt");

            if (File.Exists(currentFilePath))
            {
                NoteTextBox.Text = File.ReadAllText(currentFilePath);
            }
        }

        private void NoteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CharCountText.Text = $"Characters: {NoteTextBox.Text.Length}";

            isUnsaved = true;
            SaveStatusText.Text = "Unsaved Changes";
        }

        private void NoteTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int index = NoteTextBox.CaretIndex;
            int line = NoteTextBox.GetLineIndexFromCharacterIndex(index) + 1;
            int col = index - NoteTextBox.GetCharacterIndexFromLineIndex(line - 1) + 1;

            LineColText.Text = $"Ln {line}, Col {col}";
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath) || currentFilePath == Path.Combine(documentsFolder, "untitled.txt"))
            {
                SaveAs_Click(sender, e);
            }
            else
            {
                File.WriteAllText(currentFilePath, NoteTextBox.Text);
                isUnsaved = false;
                SaveStatusText.Text = "Saved";
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = currentFilePath,
                InitialDirectory = documentsFolder
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, NoteTextBox.Text);
                isUnsaved = false;
                SaveStatusText.Text = "Saved";
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Open File",
                InitialDirectory = documentsFolder
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentFilePath = openFileDialog.FileName;
                NoteTextBox.Text = File.ReadAllText(currentFilePath);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.WriteAllText(currentFilePath, NoteTextBox.Text);
        }
    }
}
