using System;
using System.Collections.Generic;
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

namespace jpgRemover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> jpgsToRemove = new List<string>();
        string Output = "";

        int jpegsToRemoveCount = 0;

        public Dictionary<string, string> AppKeys
        {
            get
            {
                Dictionary<string, string> Keys = new Dictionary<string, string>();
                Keys.Add("CR2", "Canon RAW Picture files (.CR2)");
                Keys.Add("DNG", "Nikon RAW Picture files (.DNG)");
                return Keys;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            InitializeSytem();         
        }

        public void InitializeSytem()
        {

            var dir = "";  Directory.GetCurrentDirectory();
            //var Output = "";
            var jpgCount = 0;
            var rawCount = 0;
            long TotalJPGFileSizeToRemove = 0;

            //Clear values incase this is the second run through from changing RAW file type combo box.
            Output = "";
            jpegsToRemoveCount = 0;
            jpgsToRemove.Clear();

            //Get Current working directory
            dir = Directory.GetCurrentDirectory();

            var RAWFile = RAWPicTypeTextComboBox.SelectedValue;

            AddToOutput("Currently working in " + dir, true);

            //Get count of RAW files.
            rawCount = Directory.GetFiles(dir, "*." + RAWFile).Count();

            //Get list of all JPG files
            string[] fileEntries = Directory.GetFiles(dir, "*.jpg");

            //Get number of JPGs found
            jpgCount = fileEntries.Count();

            AddToOutput("Found " + fileEntries.Count() + " JPG Files in the directory");

            //Loop over all JPGs found in current running directory
            foreach (string fileName in fileEntries)
            {
                // Test if the jpg has the a raw file with it. 
                //Remove the end JPG file extension and add on RAW
                var fileNameRAW = fileName.Substring(0, fileName.Length - 3) + RAWFile;

                //If there is a RAW File with the same file name as the JPG then its safee to remove the jpeg.
                if (File.Exists(fileNameRAW))
                    jpgsToRemove.Add(fileName);

                //Removed as the list of files found can be HUGE!
                //AddToOutput("Processed file: " + System.IO.Path.GetFileName(fileName));

                //TODO: Add up all the jpg file size bytes and display how much space will be saved.
                TotalJPGFileSizeToRemove += new System.IO.FileInfo(fileName).Length;
                var Bytes = "";
            }

            //Set jpg to remove count for future use
            jpegsToRemoveCount = jpgsToRemove.Count();

            AddToOutput("Total Size in Bytes: " + TotalJPGFileSizeToRemove);
            AddToOutput("Total Size in MBs: " + GetBytesToMBs(TotalJPGFileSizeToRemove), true);

            AddToOutput("Found " + jpgCount + " JPG Files.");
            AddToOutput("Found " + rawCount + " RAW " + RAWFile + " Files.");

            AddToOutput("Found " + jpgsToRemove.Count() + " JPGs that have matching RAW " + RAWFile + " Files that can be removed.");

        }

        private void DeleteJPGs_Click(object sender, RoutedEventArgs e)
        {
            //Show Yes no Box
            var result = MessageBox.Show("Do you want to delete all " + jpegsToRemoveCount + " JPG pictures from the working directory?", "Are you sure you want to delete?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {

                //Show Yes no Box
                var resultDoubleCheck = MessageBox.Show("Just double checking do you really want to delete all of your JPGs?", "Just double checking", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    //TODO: add check to see if there is any files to remove and if there isnt tell the user.

                    //TODO: Move this logic to its own method?
                    foreach (var jpgFile in jpgsToRemove)
                    {

                        //TODO: Check on if the file is read only or protected?
                        File.Delete(jpgFile);

                    }
                    AddToOutput("Removed " + jpegsToRemoveCount + " JPG files from the directory.", true);
                }              
            }
        }

        private void RAWPicTypeTextComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            InitializeSytem();
        }

        private void AddToOutput(string input, Boolean DoubleNewLine = false)
        {

            Output += input;

            if (!DoubleNewLine)
                Output += Environment.NewLine;
            else
                Output += Environment.NewLine + Environment.NewLine;

            MainOutputTextBox.Text = Output;
        }

        private string GetBytesToMBs(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };

            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", bytes, sizes[order]);

            return result;
        }

        private void HelpAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("About and help info goes in this box.", "Help / About", MessageBoxButton.OK);
        }
    }

 
}
