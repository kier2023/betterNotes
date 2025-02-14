﻿using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BetterNotes
{
    public partial class MainWindow : Window
    {
        private string currentFilePath = "untitled.txt";

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;

            if (File.Exists(currentFilePath))
            {
                NoteTextBox.Text = File.ReadAllText(currentFilePath);
            }
        }

        private void NoteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CharCountText.Text = $"Characters: {NoteTextBox.Text.Length}";
        }

        private void NoteTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int index = NoteTextBox.CaretIndex;
            int line = NoteTextBox.GetLineIndexFromCharacterIndex(index) + 1;
            int col = index - NoteTextBox.GetCharacterIndexFromLineIndex(line - 1) + 1;

            LineColText.Text = $"Ln {line}, Col {col}";
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.WriteAllText(currentFilePath, NoteTextBox.Text);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = currentFilePath
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, NoteTextBox.Text);
            }
        }
    }
}
