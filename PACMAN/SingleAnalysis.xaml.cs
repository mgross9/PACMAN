//using CorrosionModels;
using Microsoft.Win32;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace PACMAN
{
    /// <summary>
    /// Interaction logic for SingleAnalysis.xaml
    /// </summary>
    public partial class SingleAnalysis : Window
    {
        public int locationValue = 0;
        public string locationString = "";
        public bool canceled = true;
        public string selectedFileName { get; set; }

        public SingleAnalysis()
        {
            InitializeComponent();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV Files (.csv)|*.csv|Text Files (.txt)|*.txt";
            openFileDialog1.Title = "Select an Input File";
            if (openFileDialog1.ShowDialog() == true)
            {
                selectedFileName = openFileDialog1.FileName;
                SelectedFileText.Text = selectedFileName;
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFileName == "" || selectedFileName == null) {
                MessageBoxResult result = MessageBox.Show("Please select a file", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else{
                RadComboBoxItem selectedValue = locationSelect.SelectedItem as RadComboBoxItem;
                canceled = false;
                if (selectedValue == null || selectedValue.Content.ToString() == "")
                {
                    MessageBoxResult result = MessageBox.Show("Please select a location", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (selectedValue.Content.ToString() == "Singapore")
                    {
                        locationValue = 9;
                        locationString = selectedValue.Content.ToString();
                    }
                    else if (selectedValue.Content.ToString() == "Industrial (Hill Air Force Base)")
                    {
                        locationValue = 6;
                        locationString = selectedValue.Content.ToString();
                    }
                    else if (selectedValue.Content.ToString() == "Industrial/Agricultural (Wright Patt Air Force Base)")
                    {
                        locationValue = 4;
                        locationString = selectedValue.Content.ToString();
                    }
                    else if (selectedValue.Content.ToString() == "Marine (Key West Air Force Base)")
                    {
                        locationValue = 8;
                        locationString = selectedValue.Content.ToString();
                    }
                    else if (selectedValue.Content.ToString() == "Marine/Agricultural (Eglin Air Force Base)")
                    {
                        locationValue = 2;
                        locationString = selectedValue.Content.ToString();
                    }
                    else if (selectedValue.Content.ToString() == "Marine Sheltered (Hickam Air Force Base)")
                    {
                        locationValue = 5;
                    }
                    else if (selectedValue.Content.ToString() == "Rural (Tinker Air Force Base)")
                    {
                        locationValue = 3;
                        locationString = selectedValue.Content.ToString();
                    }
                    else if (selectedValue.Content.ToString() == "Rural/Arid (Luke Air Force Base)")
                    {
                        locationValue = 7;
                        locationString = selectedValue.Content.ToString();
                    }
                    else if (selectedValue.Content.ToString() == "Severe Marine (Guam Air Force Base)")
                    {
                        locationValue = 1;
                        locationString = selectedValue.Content.ToString();
                    }
                    else
                    {
                        locationValue = 9;
                        locationString = "Singapore";
                    }
                    Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            canceled = true;
            Close();
        }
    }
}
