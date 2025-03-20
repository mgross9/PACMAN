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
    /// Interaction logic for LongTermAnalysis.xaml
    /// </summary>
    public partial class LongTermAnalysis : Window
    {
        public bool canceled = true;
        public bool fleetAnalysis = false;
        public List<string> selectedTails = new List<string>();
        public string selectedTail = "";
        private DateTime? diagnosticStart;
        private DateTime? diagnosticEnd;
        public string MDS = "";
        public string individualOrFleet = "";
        public string scenarioType = "Diagnostic";
        public List<string> materials = new List<string>();
        public bool pr = false;
        public bool ti = false;
        public bool cp = false;

        public DateTime? DiagnosticStart
        {
            get
            {
                return this.diagnosticStart;
            }
            set
            {
                if (this.diagnosticStart != value)
                {
                    this.diagnosticStart = value;
                }
            }
        }

        public DateTime? DiagnosticEnd
        {
            get
            {
                return this.diagnosticEnd;
            }
            set
            {
                if (this.diagnosticEnd != value)
                {
                    this.diagnosticEnd = value;
                }
            }
        }

        public LongTermAnalysis(List<string> allTails)
        {
            StyleManager.ApplicationTheme = new VisualStudio2013Theme();
            InitializeComponent();
            foreach (string name in allTails)
            {
                RadComboBoxItem comboBoxItem = new RadComboBoxItem();
                comboBoxItem.Content = name;
                this.singleTailSelect.Items.Add(comboBoxItem);
                RadComboBoxItem comboBoxItem2 = new RadComboBoxItem();
                comboBoxItem2.Content = name;
                this.multiTailSelect.Items.Add(comboBoxItem2);
            }
            singleAircraftButton.IsChecked = true;
            DiagnosticStartDate.Culture = new System.Globalization.CultureInfo("en-US");
            DiagnosticStartDate.Culture.DateTimeFormat.ShortDatePattern = "MM-dd-yyyy";
            DiagnosticStartDate.Culture.DateTimeFormat.ShortTimePattern = "";
            DiagnosticEndDate.Culture = new System.Globalization.CultureInfo("en-US");
            DiagnosticEndDate.Culture.DateTimeFormat.ShortDatePattern = "MM-dd-yyyy";
            DiagnosticEndDate.Culture.DateTimeFormat.ShortTimePattern = "";
            DiagnosticStartDate.SelectedValue = DateTime.Today.AddYears(-3);
            DiagnosticEndDate.SelectedValue = DateTime.Today;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            
            if (singleAircraftButton.IsChecked == true)
            {

                RadComboBoxItem selectedValue = singleTailSelect.SelectedItem as RadComboBoxItem;
                if (selectedValue != null)
                {
                    selectedTail = selectedValue.Content.ToString();
                    if(DiagnosticStartDate.SelectedValue != null && DiagnosticEndDate.SelectedValue != null)
                    {
                        RadComboBoxItem selectedMDS = mdsSelect.SelectedItem as RadComboBoxItem;
                        if (selectedMDS != null) {
                            MDS = selectedMDS.Content.ToString();
                            RadComboBoxItem selectedMaterial = materialSelect.SelectedItem as RadComboBoxItem;
                            if(selectedMaterial != null)
                            {
                                materials.Add(selectedMaterial.Content.ToString());
                            }
                            if (pr)
                            {
                                materials.Add("Pr Inhibitor");
                            }
                            if (ti)
                            {
                                materials.Add("Ti fastners");
                            }
                            if (cp)
                            {
                                materials.Add("Composite Panels");
                            }
                            individualOrFleet = "Individual Aircraft";
                            DiagnosticStart = DiagnosticStartDate.SelectedValue.Value;
                            DiagnosticEnd = DiagnosticEndDate.SelectedValue.Value;
                            canceled = false;
                            Close();
                        }else
                        {
                            MessageBoxResult result = MessageBox.Show("Please select an MDS for the analysis", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Please enter a start and end date for the analysis", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }else
                {
                    MessageBoxResult result = MessageBox.Show("Please select a tail", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (fleetAircraftButton.IsChecked == true)
            {
                fleetAnalysis = true;
                var selectedValues = multiTailSelect.SelectedItems;
                foreach (RadComboBoxItem item in selectedValues)
                {
                    selectedTails.Add(item.Content.ToString());
                }
                if (selectedTails.Count != 0)
                {
                    if (DiagnosticStartDate.SelectedValue != null && DiagnosticEndDate.SelectedValue != null)
                    {
                        RadComboBoxItem selectedMDS = mdsSelect.SelectedItem as RadComboBoxItem;
                        if (selectedMDS != null)
                        {
                            MDS = selectedMDS.Content.ToString();
                            RadComboBoxItem selectedMaterial = materialSelect.SelectedItem as RadComboBoxItem;
                            if (selectedMaterial != null)
                            {
                                materials.Add(selectedMaterial.Content.ToString());
                            }
                            if (pr)
                            {
                                materials.Add("Pr Inhibitor");
                            }
                            if (ti)
                            {
                                materials.Add("Ti fastners");
                            }
                            if (cp)
                            {
                                materials.Add("Composite Panels");
                            }
                            individualOrFleet = "Fleet Aircraft";
                            DiagnosticStart = DiagnosticStartDate.SelectedValue.Value;
                            DiagnosticEnd = DiagnosticEndDate.SelectedValue.Value;
                            canceled = false;
                            Close();
                        }else
                        {
                            MessageBoxResult result = MessageBox.Show("Please select an MDS for the analysis", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Please enter a start and end date for the analysis", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Please select one or more tail", "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            else
            {
                canceled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            canceled = true;
            Close();
        }

        private void singleAircraftButton_Checked(object sender, RoutedEventArgs e)
        {
            individualGroup.Visibility = Visibility.Visible;
            fleetGroup.Visibility = Visibility.Hidden;
        }

        private void fleetAircraftButton_Checked(object sender, RoutedEventArgs e)
        {
            individualGroup.Visibility = Visibility.Hidden;
            fleetGroup.Visibility = Visibility.Visible;
        }

        private void prSelect_Checked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            pr = !pr;
        }

        private void tiSelect_Checked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            ti = !ti;
        }

        private void cpSelect_Checked(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            cp = !cp;
        }


    }
}
