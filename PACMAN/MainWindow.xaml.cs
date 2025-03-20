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
using System.Windows.Navigation;
using System.Windows.Shapes;
//using CorrosionModels;
using System.IO;
using Microsoft.Win32;
using SharpDX.Collections;
using Telerik.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls.ChartView;
using System.Windows.Threading;
using System.Threading;
using SharpDX.Direct3D10;
using System.Diagnostics;

namespace PACMAN
{
    public partial class MainWindow : Window
    {
        private CorrosionModelEntities db = new CorrosionModelEntities();

        public static int savedindex { get; set; }

        public List<string> errorMessages = new List<string>();

        private CorrosionData cd = new CorrosionData();

        private List<string> baseNamesAndTimeStamps = new List<string>();

        public List<CorrosionDataPoint> dropletPoints { get; set; }
        public List<CorrosionDataPoint> temperaturePoints { get; set; }
        public List<CorrosionDataPoint> realativeHumidityPoints { get; set; }
        public List<CorrosionDataPoint> precipitationPoints { get; set; }
        public List<CorrosionDataPoint> surfacePollutantPoints { get; set; }
        public List<CorrosionDataPoint> timeOfWetnessPoints { get; set; }
        public List<CorrosionDataPoint> pitPoints { get; set; }
        public List<CorrosionDataPoint> steelLossPoints { get; set; }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new VisualStudio2013Theme();
            InitializeComponent();
            
            ToggleLoadingVisability(false);
            DropletsChart.HorizontalAxis.Title = "Days";
            DropletsChart.VerticalAxis.Title = "Average Number of Droplets";
            TemperatureChart.HorizontalAxis.Title = "Date";
            TemperatureChart.VerticalAxis.Title = "Temperature (C)";
            RelativeHumidityChart.HorizontalAxis.Title = "Date";
            RelativeHumidityChart.VerticalAxis.Title = "RH(%)";
            PrecipitationChart.HorizontalAxis.Title = "Date";
            PrecipitationChart.VerticalAxis.Title = "Precipitation (inches)";
            SurfacePollutantsChart.HorizontalAxis.Title = "Date";
            SurfacePollutantsChart.VerticalAxis.Title = "Surface Pollutant density (mg/m)";
            TimeOfWetnessChart.HorizontalAxis.Title = "Date";
            TimeOfWetnessChart.VerticalAxis.Title = "TOW current (mA)";
            PitDistributionChart.HorizontalAxis.Title = "Pit Diameter (\u03BCm)";
            PitDistributionChart.VerticalAxis.Title = "Pit Count";
            SteelWeightLossChart.HorizontalAxis.Title = "Time(days)";
            SteelWeightLossChart.VerticalAxis.Title = "mm";
        }

        private void downloadDroplets_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Day, Average Number of Droplets");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in dropletPoints)
                {
                    string x = c.Value.ToString();
                    string y = c.Y.ToString();
                    string content = y + "," + x;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void downloadRelativeHumidity_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Date, RH(%)");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in realativeHumidityPoints)
                {
                    string x = c.Value.ToString();
                    string y = c.DateString.ToString();
                    string content = y + "," + x;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void downloadTemperature_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Date, Temperature (C)");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in temperaturePoints)
                {
                    string x = c.Value.ToString();
                    string y = c.DateString.ToString();
                    string content = y + "," + x;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void downloadPrecipitation_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Date, Precipitation (inches)");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in precipitationPoints)
                {
                    string x = c.Value.ToString();
                    string y = c.DateString.ToString();
                    string content = y + "," + x;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void downloadSurfacePollutants_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Date, Surface Pollutant Density (mg/m)");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in surfacePollutantPoints)
                {
                    string x = c.Value.ToString();
                    string y = c.DateString.ToString();
                    string content = y + "," + x;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void downloadTimeOfWetness_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Date, TOW current (mA)");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in timeOfWetnessPoints)
                {
                    string x = c.Value.ToString();
                    string y = c.DateString.ToString();
                    string content = y + "," + x;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void downloadPit_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Pit Diameter (\u03BCm), Pit Count");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in pitPoints)
                {
                    string x = c.Value.ToString();
                    string y = c.Series.ToString();
                    string content = x + "," + y;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void downloadSteelWeightLoss_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                string folderPath = saveFileDialog.FileName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("Time (days), mm");
                sb.Append("\r\n");
                foreach (CorrosionDataPoint c in steelLossPoints)
                {
                    string x = c.Value.ToString();
                    string y = c.Series.ToString();
                    string content = y + "," + x;
                    sb.Append(content);

                    sb.Append("\r\n");
                }
                string data = sb.ToString();
                System.IO.File.WriteAllText(folderPath, data);
            }
        }

        private void RibbonHomeClick(object sender, RoutedEventArgs e)
        {
            DropletsPane.Visibility = Visibility.Collapsed;
            RelativeHumidityPane.Visibility = Visibility.Collapsed;
            TemperaturePane.Visibility = Visibility.Collapsed;
            PrecipitationPane.Visibility = Visibility.Collapsed;
            SurfacePollutantsPane.Visibility = Visibility.Collapsed;
            TimeOfWetnessPane.Visibility = Visibility.Collapsed;
            PitDistributionPane.Visibility = Visibility.Collapsed;
            SteelWeightLossPane.Visibility = Visibility.Collapsed;
            FleetAnalysisPane.Visibility = Visibility.Collapsed;
        }

        private void RibbonCloseClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void RibbonSingleClick(object sender, RoutedEventArgs e)
        {
            errorMessages.Clear();
            DropletsPane.Visibility = Visibility.Collapsed;
            RelativeHumidityPane.Visibility = Visibility.Collapsed;
            TemperaturePane.Visibility = Visibility.Collapsed;
            PrecipitationPane.Visibility = Visibility.Collapsed;
            SurfacePollutantsPane.Visibility = Visibility.Collapsed;
            TimeOfWetnessPane.Visibility = Visibility.Collapsed;
            PitDistributionPane.Visibility = Visibility.Collapsed;
            SteelWeightLossPane.Visibility = Visibility.Collapsed;
            FleetAnalysisPane.Visibility = Visibility.Collapsed;
            DescriptionPane.IsActive = true;
            var SingleAnalysisWindow = new SingleAnalysis();
            SingleAnalysisWindow.ShowDialog();
            if(SingleAnalysisWindow.canceled == false)
            {
                ToggleLoadingVisability(true);

                IncrementLoadingBar("Running Analysis", 30);


                cd = singleAnalysisFromFileCombined(SingleAnalysisWindow.selectedFileName, SingleAnalysisWindow.locationValue);

                if (errorMessages.Count == 0)
                {

                    DropletsChart.Series.Clear();
                    CategoricalSeries series = new LineSeries();
                    series.ItemsSource = cd.getDropletsPoints();
                    dropletPoints = cd.getDropletsPoints();
                    series.CategoryBinding = new PropertyNameDataPointBinding("Y");
                    series.ValueBinding = new PropertyNameDataPointBinding("Value");
                    DropletsChart.Series.Add(series);

                    TemperatureChart.Series.Clear();
                    CategoricalSeries series2 = new LineSeries();
                    series2.ItemsSource = cd.getTemperaturePoints();
                    temperaturePoints = cd.getTemperaturePoints();
                    series2.CategoryBinding = new PropertyNameDataPointBinding("Date");
                    series2.ValueBinding = new PropertyNameDataPointBinding("Value");
                    TemperatureChart.Series.Add(series2);

                    RelativeHumidityChart.Series.Clear();
                    CategoricalSeries series3 = new LineSeries();
                    series3.ItemsSource = cd.getRelativeHumidityPoints();
                    realativeHumidityPoints = cd.getRelativeHumidityPoints();
                    series3.CategoryBinding = new PropertyNameDataPointBinding("Date");
                    series3.ValueBinding = new PropertyNameDataPointBinding("Value");
                    RelativeHumidityChart.Series.Add(series3);

                    PrecipitationChart.Series.Clear();
                    CategoricalSeries series4 = new LineSeries();
                    series4.ItemsSource = cd.getPrecipitationPoints();
                    precipitationPoints = cd.getPrecipitationPoints();
                    series4.CategoryBinding = new PropertyNameDataPointBinding("Date");
                    series4.ValueBinding = new PropertyNameDataPointBinding("Value");
                    PrecipitationChart.Series.Add(series4);

                    SurfacePollutantsChart.Series.Clear();
                    CategoricalSeries series5 = new LineSeries();
                    series5.ItemsSource = cd.getSurfacePollutantsPoints();
                    surfacePollutantPoints = cd.getSurfacePollutantsPoints();
                    series5.CategoryBinding = new PropertyNameDataPointBinding("Date");
                    series5.ValueBinding = new PropertyNameDataPointBinding("Value");
                    SurfacePollutantsChart.Series.Add(series5);

                    TimeOfWetnessChart.Series.Clear();
                    CategoricalSeries series6 = new LineSeries();
                    series6.ItemsSource = cd.getTimeofWetnessPoints();
                    timeOfWetnessPoints = cd.getTimeofWetnessPoints();
                    series6.CategoryBinding = new PropertyNameDataPointBinding("Date");
                    series6.ValueBinding = new PropertyNameDataPointBinding("Value");
                    TimeOfWetnessChart.Series.Add(series6);

                    PitDistributionChart.Series.Clear();
                    CategoricalSeries series7 = new BarSeries();
                    series7.ItemsSource = cd.getPitPoints();
                    pitPoints = cd.getPitPoints();
                    series7.CategoryBinding = new PropertyNameDataPointBinding("Value");
                    series7.ValueBinding = new PropertyNameDataPointBinding("Series");
                    PitDistributionChart.Series.Add(series7);

                    SteelWeightLossChart.Series.Clear();
                    ScatterPointSeries series8 = new ScatterPointSeries();
                    series8.ItemsSource = cd.getSteelLossPoints();
                    steelLossPoints = cd.getSteelLossPoints();
                    series8.XValueBinding = new PropertyNameDataPointBinding("Series");
                    series8.YValueBinding = new PropertyNameDataPointBinding("Value");
                    SteelWeightLossChart.Series.Add(series8);

                    ParameterTree.Items.Clear();
                    ParameterTree.Items.Add(new RadTreeViewItem() { Header = "Single Analysis", Items = { new RadTreeViewItem() { Header = "File", Items = { new RadTreeViewItem() { Header = SingleAnalysisWindow.selectedFileName } } }, new RadTreeViewItem() { Header = "Location", Items = { new RadTreeViewItem() { Header = SingleAnalysisWindow.locationString } } } } });


                    FleetAnalysisPane.Visibility = Visibility.Hidden;
                    DropletsPane.Visibility = Visibility.Visible;
                    RelativeHumidityPane.Visibility = Visibility.Visible;
                    TemperaturePane.Visibility = Visibility.Visible;
                    PrecipitationPane.Visibility = Visibility.Visible;
                    SurfacePollutantsPane.Visibility = Visibility.Visible;
                    TimeOfWetnessPane.Visibility = Visibility.Visible;
                    PitDistributionPane.Visibility = Visibility.Visible;
                    SteelWeightLossPane.Visibility = Visibility.Visible;
                    FleetAnalysisPane.Visibility = Visibility.Hidden;
                }else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in errorMessages)
                    {
                        sb.Append(s);
                        sb.Append("\n");
                    }
                    MessageBoxResult result = MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                ToggleLoadingVisability(false);
            }
        }

        private void RibbonLongTermClick(object sender, RoutedEventArgs e)
        {

            DropletsPane.Visibility = Visibility.Collapsed;
            RelativeHumidityPane.Visibility = Visibility.Collapsed;
            TemperaturePane.Visibility = Visibility.Collapsed;
            PrecipitationPane.Visibility = Visibility.Collapsed;
            SurfacePollutantsPane.Visibility = Visibility.Collapsed;
            TimeOfWetnessPane.Visibility = Visibility.Collapsed;
            PitDistributionPane.Visibility = Visibility.Collapsed;
            SteelWeightLossPane.Visibility = Visibility.Collapsed;
            FleetAnalysisPane.Visibility = Visibility.Collapsed;
            DescriptionPane.IsActive = true;

            errorMessages.Clear();
            try
            {
                List<string> allTails = GetAllTails();
                var LongTermWindow = new LongTermAnalysis(allTails);
                LongTermWindow.ShowDialog();

                if (!LongTermWindow.canceled)
                {
                    if (!LongTermWindow.fleetAnalysis)
                    {
                        ToggleLoadingVisability(true);
                        string selectedTail = LongTermWindow.selectedTail.ToString();
                        DateTime StartDate = (DateTime)LongTermWindow.DiagnosticStart;
                        DateTime EndDate = (DateTime)LongTermWindow.DiagnosticEnd;

                        IncrementLoadingBar("Running Analysis", 30);

                        cd = multiLocationAnalysis(true, selectedTail, StartDate, EndDate);

                        IncrementLoadingBar("Generating Charts", 80);

                        if (errorMessages.Count == 0)
                        {

                            DropletsChart.Series.Clear();
                            CategoricalSeries series = new LineSeries();
                            series.ItemsSource = cd.getDropletsPoints();
                            dropletPoints = cd.getDropletsPoints();
                            series.CategoryBinding = new PropertyNameDataPointBinding("Y");
                            series.ValueBinding = new PropertyNameDataPointBinding("Value");
                            DropletsChart.Series.Add(series);

                            TemperatureChart.Series.Clear();
                            CategoricalSeries series2 = new LineSeries();
                            series2.ItemsSource = cd.getTemperaturePoints();
                            temperaturePoints = cd.getTemperaturePoints();
                            series2.CategoryBinding = new PropertyNameDataPointBinding("Date");
                            series2.ValueBinding = new PropertyNameDataPointBinding("Value");
                            TemperatureChart.Series.Add(series2);

                            RelativeHumidityChart.Series.Clear();
                            CategoricalSeries series3 = new LineSeries();
                            series3.ItemsSource = cd.getRelativeHumidityPoints();
                            realativeHumidityPoints = cd.getRelativeHumidityPoints();
                            series3.CategoryBinding = new PropertyNameDataPointBinding("Date");
                            series3.ValueBinding = new PropertyNameDataPointBinding("Value");
                            RelativeHumidityChart.Series.Add(series3);

                            PrecipitationChart.Series.Clear();
                            CategoricalSeries series4 = new LineSeries();
                            series4.ItemsSource = cd.getPrecipitationPoints();
                            precipitationPoints = cd.getPrecipitationPoints();
                            series4.CategoryBinding = new PropertyNameDataPointBinding("Date");
                            series4.ValueBinding = new PropertyNameDataPointBinding("Value");
                            PrecipitationChart.Series.Add(series4);

                            SurfacePollutantsChart.Series.Clear();
                            CategoricalSeries series5 = new LineSeries();
                            series5.ItemsSource = cd.getSurfacePollutantsPoints();
                            surfacePollutantPoints = cd.getSurfacePollutantsPoints();
                            series5.CategoryBinding = new PropertyNameDataPointBinding("Date");
                            series5.ValueBinding = new PropertyNameDataPointBinding("Value");
                            SurfacePollutantsChart.Series.Add(series5);

                            TimeOfWetnessChart.Series.Clear();
                            CategoricalSeries series6 = new LineSeries();
                            series6.ItemsSource = cd.getTimeofWetnessPoints();
                            timeOfWetnessPoints = cd.getTimeofWetnessPoints();
                            series6.CategoryBinding = new PropertyNameDataPointBinding("Date");
                            series6.ValueBinding = new PropertyNameDataPointBinding("Value");
                            TimeOfWetnessChart.Series.Add(series6);

                            PitDistributionChart.Series.Clear();
                            CategoricalSeries series7 = new BarSeries();
                            series7.ItemsSource = cd.getPitPoints();
                            pitPoints = cd.getPitPoints();
                            series7.CategoryBinding = new PropertyNameDataPointBinding("Value");
                            series7.ValueBinding = new PropertyNameDataPointBinding("Series");
                            PitDistributionChart.Series.Add(series7);

                            SteelWeightLossChart.Series.Clear();
                            ScatterPointSeries series8 = new ScatterPointSeries();
                            series8.ItemsSource = cd.getSteelLossPoints();
                            steelLossPoints = cd.getSteelLossPoints();
                            series8.XValueBinding = new PropertyNameDataPointBinding("Series");
                            series8.YValueBinding = new PropertyNameDataPointBinding("Value");
                            SteelWeightLossChart.Series.Add(series8);

                            ParameterTree.Items.Clear();
                            RadTreeViewItem materialItem = new RadTreeViewItem() { Header = "Materials" };
                            foreach (string item in LongTermWindow.materials)
                            {
                                materialItem.Items.Add(new RadTreeViewItem() { Header = item });
                            }

                            RadTreeViewItem dateItem = new RadTreeViewItem() { Header = "Diagnostic Analysis" };
                            foreach (string item in baseNamesAndTimeStamps)
                            {
                                dateItem.Items.Add(new RadTreeViewItem() { Header = item });
                            }

                            ParameterTree.Items.Add(new RadTreeViewItem() { Header = "Long Term Analysis", Items = { new RadTreeViewItem() { Header = "MDS", Items = { new RadTreeViewItem() { Header = LongTermWindow.MDS } } }, new RadTreeViewItem() { Header = "Scenario Type", Items = { new RadTreeViewItem() { Header = LongTermWindow.scenarioType } } }, new RadTreeViewItem() { Header = "Selected Tail", Items = { new RadTreeViewItem() { Header = LongTermWindow.selectedTail } } }, materialItem, dateItem } });

                            DropletsPane.Visibility = Visibility.Visible;
                            RelativeHumidityPane.Visibility = Visibility.Visible;
                            TemperaturePane.Visibility = Visibility.Visible;
                            PrecipitationPane.Visibility = Visibility.Visible;
                            SurfacePollutantsPane.Visibility = Visibility.Visible;
                            TimeOfWetnessPane.Visibility = Visibility.Visible;
                            PitDistributionPane.Visibility = Visibility.Visible;
                            SteelWeightLossPane.Visibility = Visibility.Visible;
                            FleetAnalysisPane.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string s in errorMessages)
                            {
                                sb.Append(s);
                                sb.Append("\n");
                            }
                            MessageBoxResult result = MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else if (LongTermWindow.fleetAnalysis)
                    {
                        ToggleLoadingVisability(true);

                        List<string> selectedTails = new List<string>();
                        foreach (string item in LongTermWindow.selectedTails)
                        {
                            selectedTails.Add(item);
                        }

                        DateTime StartDate = (DateTime)LongTermWindow.DiagnosticStart;
                        DateTime EndDate = (DateTime)LongTermWindow.DiagnosticEnd;

                        MultiLocationAnalysis ml = new MultiLocationAnalysis();
                        List<MultiTailDataObject> ObjectSource = new List<MultiTailDataObject>();
                        if (errorMessages.Count == 0)
                        {

                            int indexId = 1;
                            RadTreeViewItem dateItem = new RadTreeViewItem() { Header = "Diagnostic Analysis" };
                            foreach (string Tail in selectedTails)
                            {
                                var list = (from t in db.t_History
                                            where t.Tail_Number == Tail && StartDate <= t.Departure_Time && EndDate >= t.Arrival_Time
                                            select t).OrderBy(x => x.Arrival_Time);
                                SingleAnalysisFunctions cm = new SingleAnalysisFunctions();
                                List<AnalysisTask> tasks = new List<AnalysisTask>();
                                foreach (t_History his in list)
                                {
                                    AnalysisTask convertedHistory = new AnalysisTask();
                                    if (his.Arrival_Time.Date < StartDate)
                                    {
                                        convertedHistory.StartDate = StartDate;
                                    }
                                    else
                                    {
                                        convertedHistory.StartDate = his.Arrival_Time.Date;
                                    }
                                    if (his.Departure_Time.Date > EndDate)
                                    {
                                        convertedHistory.EndDate = EndDate;
                                    }
                                    else
                                    {
                                        convertedHistory.EndDate = his.Departure_Time.Date;
                                    }
                                    convertedHistory.Location = his.Location;
                                    convertedHistory.WashFreq = 0;
                                    convertedHistory.DetergentWash = false;
                                    convertedHistory.MetDataType = MetDataTypes.Meteorological;
                                    convertedHistory.CrystalSize = 6;
                                    convertedHistory.LocationString = GetLocationById(his.Location);
                                    tasks.Add(convertedHistory);
                                }
                                int numTasks = tasks.Count();
                                cd.BaseNames = new string[numTasks];
                                cd.cutoffValues = new int[numTasks];
                                cd.cutoffValuesDroplets = new int[numTasks];


                                int wpIndex = 1;
                                int guamIndex = 1;
                                int tinkerIndex = 1;
                                int eglinIndex = 1;
                                int hickamIndex = 1;
                                int hillIndex = 1;
                                int lukeIndex = 1;
                                int kwIndex = 1;
                                int robinIndex = 1;

                                int index = 0;
                                baseNamesAndTimeStamps.Clear();
                                foreach (AnalysisTask task in tasks)
                                {
                                    if (task.LocationString == "Wright Patterson Air Force Base")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Wright Patt" + wpIndex))
                                        {
                                            wpIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Wright Patt" + wpIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Guam International Airport")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Guam" + guamIndex))
                                        {
                                            guamIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Guam" + guamIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Tinker Air Force Base")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Tinker" + tinkerIndex))
                                        {
                                            tinkerIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Tinker" + tinkerIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Eglin Air Force Base")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Eglin" + eglinIndex))
                                        {
                                            eglinIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Eglin" + eglinIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Hickam Air Force Base")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Hickam" + hickamIndex))
                                        {
                                            hickamIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Hickam" + hickamIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Hill Air Force Base")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Hill" + hillIndex))
                                        {
                                            hillIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Hill" + hillIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Luke Air Force Base")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Luke" + lukeIndex))
                                        {
                                            lukeIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Luke" + lukeIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Naval Air Station Key West")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Key West" + kwIndex))
                                        {
                                            kwIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Key West" + kwIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else if (task.LocationString == "Robins Air Force Base")
                                    {
                                        if (Array.Exists(cd.BaseNames, element => element == "Robins" + robinIndex))
                                        {
                                            robinIndex++;
                                        }
                                        baseNamesAndTimeStamps.Add("Robins" + robinIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    else
                                    {
                                        baseNamesAndTimeStamps.Add(task.LocationString + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                                    }
                                    index++;
                                }



                                int value = ml.BeginMultiTailAnalysis(StartDate, EndDate);
                                MultiTailDataObject m = new MultiTailDataObject();
                                m.Id = indexId;
                                m.MDS = "F-15";
                                m.TailNumber = Tail;
                                m.LocalizedCorrosion = value;
                                m.FatigueDamage = value;
                                ObjectSource.Add(m);
                                indexId++;

                                RadTreeViewItem singleDateItem = new RadTreeViewItem() { Header = Tail };
                                foreach (string item in baseNamesAndTimeStamps)
                                {
                                    singleDateItem.Items.Add(new RadTreeViewItem() { Header = item });
                                }
                                dateItem.Items.Add(singleDateItem);
                            }
                            FleetGridView.ItemsSource = ObjectSource;

                            ParameterTree.Items.Clear();

                            RadTreeViewItem materialItem = new RadTreeViewItem() { Header = "Materials" };
                            foreach (string item in LongTermWindow.materials)
                            {
                                materialItem.Items.Add(new RadTreeViewItem() { Header = item });
                            }
                            RadTreeViewItem tailItem = new RadTreeViewItem() { Header = "Selected Tails" };
                            foreach (string item in LongTermWindow.selectedTails)
                            {
                                tailItem.Items.Add(new RadTreeViewItem() { Header = item });
                            }
                            ParameterTree.Items.Add(new RadTreeViewItem() { Header = "Fleet Analysis", Items = { new RadTreeViewItem() { Header = "MDS", Items = { new RadTreeViewItem() { Header = LongTermWindow.MDS } } }, new RadTreeViewItem() { Header = "Scenario Type", Items = { new RadTreeViewItem() { Header = LongTermWindow.scenarioType } } }, tailItem, materialItem, dateItem } });

                            DropletsPane.Visibility = Visibility.Collapsed;
                            RelativeHumidityPane.Visibility = Visibility.Collapsed;
                            TemperaturePane.Visibility = Visibility.Collapsed;
                            PrecipitationPane.Visibility = Visibility.Collapsed;
                            SurfacePollutantsPane.Visibility = Visibility.Collapsed;
                            TimeOfWetnessPane.Visibility = Visibility.Collapsed;
                            PitDistributionPane.Visibility = Visibility.Collapsed;
                            SteelWeightLossPane.Visibility = Visibility.Collapsed;
                            FleetAnalysisPane.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string s in errorMessages)
                            {
                                sb.Append(s);
                                sb.Append("\n");
                            }
                            MessageBoxResult result = MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {

                    }
                }

            }catch(Exception sqlError){
                if (sqlError.InnerException != null)
                {
                   errorMessages.Add("Error encountered during analysis, Exact error message was: " + sqlError.InnerException.Message);
                }
                else
                {
                    errorMessages.Add("Error encountered during analysis, Exact error message was: " + sqlError.Message);
                }
                StringBuilder sb = new StringBuilder();
                foreach (string s in errorMessages)
                {
                    sb.Append(s);
                    sb.Append("\n");
                }
                MessageBoxResult result = MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            ToggleLoadingVisability(false);
        }

        private void IncrementLoadingBar(string step, int value)
        {
            Dispatcher.Invoke(DispatcherPriority.Loaded,
            (Action)(() =>
            {
                LoadingLabel.Content = step;
                PercentageLabel.Content = value + "%";
                LoadingBar.Value = value;
            }));

        }

        private void ToggleLoadingVisability(bool visable)
        {
            Dispatcher.Invoke(DispatcherPriority.Loaded,
            (Action)(() =>
                {
                    if (visable)
                    {
                        this.InstructionText.Visibility = Visibility.Hidden;
                        this.InstructionTextTwo.Visibility = Visibility.Hidden;
                        this.InstructionTextThree.Visibility = Visibility.Hidden;
                        this.InstructionTextFour.Visibility = Visibility.Hidden;
                        //this.InstructionTextFive.Visibility = Visibility.Hidden;
                        this.LoadingLabel.Visibility = Visibility.Visible;
                        this.PercentageLabel.Visibility = Visibility.Visible;
                        this.LoadingBar.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        LoadingLabel.Content = "Reading File";
                        PercentageLabel.Content = "10%";
                        LoadingBar.Value = 10;
                        this.LoadingLabel.Visibility = Visibility.Hidden;
                        this.PercentageLabel.Visibility = Visibility.Hidden;
                        this.LoadingBar.Visibility = Visibility.Hidden;
                        this.InstructionText.Visibility = Visibility.Visible;
                        this.InstructionTextTwo.Visibility = Visibility.Visible;
                        this.InstructionTextThree.Visibility = Visibility.Visible;
                        this.InstructionTextFour.Visibility = Visibility.Visible;
                        //this.InstructionTextFive.Visibility = Visibility.Visible;
                    }
                }));
        }

        public CorrosionData singleAnalysisFromFileCombined(string fileName, int location)
        {

            string result = string.Empty;

            string extention = System.IO.Path.GetExtension(System.IO.Path.GetFileName(fileName));
            List<CombinedDataViewModel> fileData = new List<CombinedDataViewModel>();
            string title = "";
            if (extention == ".csv" || extention == ".txt")
            {
                result = new StreamReader(fileName).ReadToEnd();
                string fixedResult = result.Replace('\r', ' ');
                string[] allLines = fixedResult.Split('\n');
                int linenumber = 1;
                int savedMonth = 0;
                foreach (string lineText in allLines)
                {
                    if (linenumber >= 3)
                    {
                        lineText.TrimEnd(',');
                        string trimmedLineText = lineText.TrimEnd(',', ' ');
                        string[] lineValues = trimmedLineText.Split(',');
                        if (lineValues.Length == 11 && (lineValues[0].TrimEnd(' ') != "0" || linenumber == 3))//7 items per row
                        {
                            try
                            {
                                CombinedDataViewModel s = new CombinedDataViewModel();
                                if (lineValues[0].TrimEnd(' ') != "0" && lineValues[0] != "NULL" && lineValues[0].TrimEnd(' ') != "" && !lineValues[0].Contains("#"))
                                {
                                    s.Record_Number = Convert.ToInt32(lineValues[0].TrimEnd(' '));
                                }
                                else
                                {
                                    s.Record_Number = 0;
                                }
                                if (lineValues[1].TrimEnd(' ') != "0" && lineValues[1] != "NULL" && lineValues[1].TrimEnd(' ') != "" && !lineValues[1].Contains("#"))
                                {
                                    s.TOWSensor = Convert.ToDouble(lineValues[1].TrimEnd(' '));
                                }
                                else
                                {
                                    s.TOWSensor = 0;
                                }
                                if (lineValues[2].TrimEnd(' ') != "0" && lineValues[2] != "NULL" && lineValues[2].TrimEnd(' ') != "" && !lineValues[2].Contains("#"))
                                {
                                    s.TOWCalc = Convert.ToDouble(lineValues[2].TrimEnd(' '));
                                }
                                else
                                {
                                    s.TOWCalc = 0;
                                }
                                if (lineValues[3].TrimEnd(' ') != "0" && lineValues[3] != "NULL" && lineValues[3].TrimEnd(' ') != "" && !lineValues[3].Contains("#"))
                                {
                                    s.SurfaceTemp = Convert.ToDouble(lineValues[3]);
                                }
                                else
                                {
                                    s.SurfaceTemp = 0;
                                }
                                if (lineValues[4].TrimEnd(' ') != "0" && lineValues[4] != "NULL" && lineValues[4].TrimEnd(' ') != "" && !lineValues[4].Contains("#"))
                                {
                                    s.TimeInterval = Convert.ToInt32(lineValues[4].TrimEnd(' '));
                                }
                                else
                                {
                                    s.TimeInterval = 0;
                                }
                                if (lineValues[5].TrimEnd(' ') != "0" && lineValues[5] != "NULL" && lineValues[5].TrimEnd(' ') != "" && !lineValues[5].Contains("#"))
                                {
                                    s.ATM_RH = Convert.ToDouble(lineValues[5].TrimEnd(' '));
                                }
                                else
                                {
                                    s.ATM_RH = 0;
                                }
                                if (lineValues[6].TrimEnd(' ') != "0" && lineValues[6] != "NULL" && lineValues[6].TrimEnd(' ') != "" && !lineValues[6].Contains("#"))
                                {
                                    s.ATM_Temp = Convert.ToDouble(lineValues[6].TrimEnd(' '));
                                }
                                else
                                {
                                    s.ATM_Temp = 0;
                                }
                                if (lineValues[7].TrimEnd(' ') != "0" && lineValues[7] != "NULL" && lineValues[7].TrimEnd(' ') != "" && !lineValues[7].Contains("#"))
                                {
                                    s.Rain = Convert.ToDouble(lineValues[7].TrimEnd(' '));
                                }
                                else
                                {
                                    s.Rain = 0;
                                }
                                if (lineValues[8].TrimEnd(' ') != "" && lineValues[8] != "NULL" && lineValues[8].TrimEnd(' ') != "" && !lineValues[8].Contains("#"))
                                {
                                    s.Timestamp = Convert.ToDateTime(lineValues[8].TrimEnd(' '));
                                }
                                else
                                {
                                    s.Timestamp = DateTime.Now;
                                }
                                if (lineValues[9].TrimEnd(' ') != "0" && lineValues[9] != "NULL" && lineValues[9].TrimEnd(' ') != "" && !lineValues[9].Contains("#"))
                                {
                                    double catchDouble = Convert.ToDouble(lineValues[9].TrimEnd(' '));
                                    s.Month = (int)catchDouble;
                                    savedMonth = s.Month;
                                }
                                else
                                {
                                    s.Month = savedMonth;
                                }
                                if (lineValues[10].TrimEnd(' ') != "0" && lineValues[10] != "NULL" && lineValues[10].TrimEnd(' ') != "" && !lineValues[10].Contains("#"))
                                {
                                    double catchDouble = Convert.ToDouble(lineValues[10].TrimEnd(' '));
                                    s.DataSourceType = (int)catchDouble;
                                }
                                else
                                {
                                    s.DataSourceType = 1;
                                }
                                fileData.Add(s);
                            }
                            catch (Exception e)
                            {
                                errorMessages.Add("Invalid data was encountered. Error encountered on line " + linenumber + ". Exact error message was: " + e.InnerException.Message);
                            }
                        }else if (linenumber > 3 && lineValues.Length == 1)
                        {

                        }
                        else
                        {
                            if ((lineValues.Length != 11 && lineValues[0].TrimEnd(' ') != "0") || (lineValues.Length != 11 && lineValues[0].TrimEnd(' ') != "")) {
                                errorMessages.Add("File contains an incorrect number of columns (" + lineValues.Length + "). Error encountered on line " + linenumber);
                            }
                        }
                    }
                    else if (linenumber == 1)
                    {
                        title = lineText.TrimEnd(',', ' ');
                    }
                    else if (linenumber == 2)
                    {

                    }
                    linenumber++;
                }
            }
            if (errorMessages.Count == 0 || (errorMessages.Count != 0 && fileData.Count != 0))
            {
                errorMessages.Clear();
                SingleAnalysisFunctions cm = new SingleAnalysisFunctions();
                Dictionary<string, PollutionModel> pollutionData = getAllPollutionValues();
                IncrementLoadingBar("Finalizing", 80);

                try
                {
                    CorrosionData cdt = cm.BeginAnalysisSingle(fileData, location, pollutionData);
                    cdt.BaseNames[0] = "Custom Location";
                    cdt.fileTitle = title;
                    return cdt;

                }
                catch(Exception e)
                {
                    if (e.InnerException != null)
                    {
                        if (fileData.Count != 0)
                        {
                            errorMessages.Add("Error encountered during analysis, Exact error message was: " + e.InnerException.Message);
                        }
                        else
                        {
                            errorMessages.Add("File data could not be processed for selected analysis");
                        }
                    }
                    else
                    {
                        if (fileData.Count != 0)
                        {
                            errorMessages.Add("Error encountered during analysis, Exact error message was: " + e.Message);
                        }
                        else
                        {
                            errorMessages.Add("File data could not be processed for selected analysis");
                        }
                    }
                }
            }
            return cd;
        }

        public CorrosionData multiLocationAnalysis(bool finalAnalysis, string selectedTail, DateTime StartDate, DateTime EndDate)
        {
            var list = (from t in db.t_History
                        where t.Tail_Number == selectedTail && StartDate <= t.Departure_Time && EndDate >= t.Arrival_Time
                        select t).OrderBy(x => x.Arrival_Time);
            SingleAnalysisFunctions cm = new SingleAnalysisFunctions();
            List<AnalysisTask> tasks = new List<AnalysisTask>();
            foreach (t_History his in list)
            {
                AnalysisTask convertedHistory = new AnalysisTask();
                if (his.Arrival_Time.Date < StartDate)
                {
                    convertedHistory.StartDate = StartDate;
                }
                else
                {
                    convertedHistory.StartDate = his.Arrival_Time.Date;
                }
                if (his.Departure_Time.Date > EndDate)
                {
                    convertedHistory.EndDate = EndDate;
                }
                else
                {
                    convertedHistory.EndDate = his.Departure_Time.Date;
                }
                convertedHistory.Location = his.Location;
                convertedHistory.WashFreq = 0;
                convertedHistory.DetergentWash = false;
                convertedHistory.MetDataType = MetDataTypes.Meteorological;
                convertedHistory.CrystalSize = 6;
                convertedHistory.LocationString = GetLocationById(his.Location);
                tasks.Add(convertedHistory);
            }
            Dictionary<string, PollutionModel> pollutionData = getAllPollutionValues();

            CorrosionData cd = new CorrosionData();
            List<CombinedDataViewModel> sensorData = new List<CombinedDataViewModel>();

            try
            {

                int wpIndex = 1;
                int guamIndex = 1;
                int tinkerIndex = 1;
                int eglinIndex = 1;
                int hickamIndex = 1;
                int hillIndex = 1;
                int lukeIndex = 1;
                int kwIndex = 1;
                int robinIndex = 1;

                int numTasks = tasks.Count();
                cd.BaseNames = new string[numTasks];
                cd.cutoffValues = new int[numTasks];
                cd.cutoffValuesDroplets = new int[numTasks];

                int index = 0;
                baseNamesAndTimeStamps.Clear();
                foreach (AnalysisTask task in tasks)
                {
                    var a = ReadFile(task);
                    var b = a.OrderBy(t => t.Timestamp);
                    sensorData.AddRange(b);
                    cd.cutoffValues[index] = sensorData.Count;
                    if (task.LocationString == "Wright Patterson Air Force Base")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Wright Patt" + wpIndex))
                        {
                            wpIndex++;
                        }
                        cd.BaseNames[index] = "Wright Patt" + wpIndex;
                        baseNamesAndTimeStamps.Add("Wright Patt" + wpIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Guam International Airport")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Guam" + guamIndex))
                        {
                            guamIndex++;
                        }
                        cd.BaseNames[index] = "Guam" + guamIndex;
                        baseNamesAndTimeStamps.Add("Guam" + guamIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Tinker Air Force Base")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Tinker" + tinkerIndex))
                        {
                            tinkerIndex++;
                        }
                        cd.BaseNames[index] = "Tinker" + tinkerIndex;
                        baseNamesAndTimeStamps.Add("Tinker" + tinkerIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Eglin Air Force Base")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Eglin" + eglinIndex))
                        {
                            eglinIndex++;
                        }
                        cd.BaseNames[index] = "Eglin" + eglinIndex;
                        baseNamesAndTimeStamps.Add("Eglin" + eglinIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Hickam Air Force Base")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Hickam" + hickamIndex))
                        {
                            hickamIndex++;
                        }
                        cd.BaseNames[index] = "Hickam" + hickamIndex;
                        baseNamesAndTimeStamps.Add("Hickam" + hickamIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Hill Air Force Base")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Hill" + hillIndex))
                        {
                            hillIndex++;
                        }
                        cd.BaseNames[index] = "Hill" + hillIndex;
                        baseNamesAndTimeStamps.Add("Hill" + hillIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Luke Air Force Base")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Luke" + lukeIndex))
                        {
                            lukeIndex++;
                        }
                        cd.BaseNames[index] = "Luke" + lukeIndex;
                        baseNamesAndTimeStamps.Add("Luke" + lukeIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Naval Air Station Key West")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Key West" + kwIndex))
                        {
                            kwIndex++;
                        }
                        cd.BaseNames[index] = "Key West" + kwIndex;
                        baseNamesAndTimeStamps.Add("Key West" + kwIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else if (task.LocationString == "Robins Air Force Base")
                    {
                        if (Array.Exists(cd.BaseNames, element => element == "Robins" + robinIndex))
                        {
                            robinIndex++;
                        }
                        cd.BaseNames[index] = "Robins" + robinIndex;
                        baseNamesAndTimeStamps.Add("Robins" + robinIndex + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    else
                    {
                        cd.BaseNames[index] = task.LocationString;
                        baseNamesAndTimeStamps.Add(task.LocationString + ": " + task.StartDate.ToShortDateString() + " - " + task.EndDate.ToShortDateString());
                    }
                    index++;
                }

                cm.BeginAnalysisMultiple(tasks, selectedTail, sensorData, pollutionData, ref cd);

            }catch(Exception e)
            {
                if (e.InnerException != null)
                {
                    if (sensorData.Count != 0)
                    {
                        errorMessages.Add("Error encountered during analysis, Exact error message was: " + e.InnerException.Message);
                    }else
                    {
                        errorMessages.Add("No sensor data avalible for selected analysis");
                    }
                }else
                {
                    if (sensorData.Count != 0)
                    {
                        errorMessages.Add("Error encountered during analysis, Exact error message was: " + e.Message);
                    }
                    else
                    {
                        errorMessages.Add("No sensor data avalible for selected analysis");
                    }
                }
            }
            return cd;
        }

        public string GetLocationById(int ID)
        {
            List<t_Location> l1 = db.t_Location.Where(r => (r.Id == ID)).ToList();
            string LocationName = (l1.FirstOrDefault()).Name;
            return LocationName;
        }

        public List<CombinedDataViewModel> ReadFile(AnalysisTask _task)
        {
            List<CombinedDataViewModel> x = GetMetData(_task, savedindex);
            savedindex = x.Count();
            return x;

        }

        public List<CombinedDataViewModel> GetMetData(AnalysisTask _t, int startindex)
        { 
            List<CombinedDataViewModel> DataList = new List<CombinedDataViewModel>();

            List<CombinedData> list = db.CombinedDatas.Where(r => (r.Timestamp > _t.StartDate && r.Timestamp < _t.EndDate && r.Location == _t.Location)).ToList();
            int id = startindex;
            foreach (CombinedData item in list)
            {
                DataList.Add(new CombinedDataViewModel
                {
                    ID = id,
                    Record_Number = Convert.ToInt32(item.Record_Number),
                    TOWSensor = Convert.ToDouble(item.TOWSensor),
                    TOWCalc = Convert.ToDouble(item.TOWCalc),
                    SurfaceTemp = Convert.ToDouble(item.SurfaceTemp),
                    TimeInterval = Convert.ToInt32(item.TimeInterval),
                    ATM_RH = Convert.ToDouble(item.ATM_RH),
                    ATM_Temp = Convert.ToDouble(item.ATM_Temp),
                    Rain = Convert.ToDouble(item.Rain),
                    Timestamp = item.Timestamp ?? DateTime.Now,
                    Month = Convert.ToInt32(item.Month),
                    Location = item.Location ?? 0,
                    DataSourceType = Convert.ToInt32(item.DataType)
                });
                id++;
            }
            return DataList;
        }

        public Dictionary<string, PollutionModel> getAllPollutionValues()
        {
            var a = (from t in db.t_Salt_Composition
                     join y in db.t_Base_Properties on new { t.Location, t.Month } equals new { y.Location, y.Month }
                     select new PollutionModel
                     {
                         Deposit = (double)y.Salt_dep,
                         AmmNit = (double)t.NH4NO3,
                         AmmSul = (double)t.NH42SO4,
                         RainConc = (double)y.Conc,
                         NaCl = (double)t.NaCl,
                         MgCl2 = (double)t.MgC12,
                         Na2S04 = (double)t.Na2S04,
                         CaC12 = (double)t.CaC12,
                         KCI = (double)t.KCI,
                         MgSO4 = (double)t.MgSO4,
                         K2SO4 = (double)t.K2SO4,
                         CaSO4 = (double)t.CaSO4,
                         total = (double)t.Total,
                         HSMass = (double?)t.HSMass,
                         HNMass = (double?)t.HNMass,
                         Sea = 0.5,
                         RainDep = 2.20775,
                         Location = t.Location,
                         Month = t.Month
                     });
            Dictionary<string, PollutionModel> result = new Dictionary<string, PollutionModel>();
            foreach (PollutionModel pm in a)
            {
                result.Add(pm.Location + "-" + pm.Month, pm);
            }

            return result;
        }

        public List<string> GetAllTails()
        {
            List<t_History> his = db.t_History.ToList();
            List<string> tails = new List<string>();
            foreach (t_History item in his)
            {
                if (!tails.Contains(item.Tail_Number))
                {
                    tails.Add(item.Tail_Number);
                }
            }
            return tails;
        }

    }
}
