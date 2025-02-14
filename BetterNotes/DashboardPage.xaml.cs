using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetterNotes
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public ObservableCollection<NoteFile> NoteFiles { get; set; }
        public DashboardPage()
        {
            InitializeComponent();
            NoteFiles = new ObservableCollection<NoteFile>();
            NotesDataGrid.ItemsSource = NoteFiles;

            LoadFiles();

        }

        private void LoadFiles()
        {
            string directoryPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BetterNotes"); // This isn't going to work lol, need to rework the saving methods in mainWindows cs. 

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.txt");

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    NoteFiles.Add(new NoteFile
                    {
                        FileName = fileInfo.Name,
                        LastModified = fileInfo.LastWriteTime
                    });
                }
            }
        }
    }

    public class NoteFile
    {
        public string FileName { get; set; }
        public DateTime LastModified { get; set; }
    }
}
