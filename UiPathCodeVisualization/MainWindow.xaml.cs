using MahApps.Metro.Controls;
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
using UiPathProjectAnalyser.Models;
using UiPathCodeVisualization;
using UiPathProjectAnalyser.Utility;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using LiveCharts;
using LiveCharts.Wpf;

namespace UiPathCodeVisualization
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public CommonOpenFileDialog dlg;

        public LiveChartData ChartData { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dlg = new CommonOpenFileDialog();
            dlg.Title = "project.jsonを選択してください";
            var filter = new CommonFileDialogFilter();
            filter.DisplayName = "許可されたファイル";
            filter.Extensions.Add("json");
            dlg.Filters.Add(filter);
            ChartData = new LiveChartData();
            cartesianChart.DataContext = ChartData;
            pieChart.DataContext = ChartData;
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItems.Count == 0) return;
            var activityList = (listView.SelectedItems[0] as UiPathWorkFlow).ActivityLists;
            this.activityListView.DataContext = activityList;
            ChartData.SetActivityData(activityList);
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.folderTextBox.Text = dlg.FileName;
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.folderTextBox.Text)){
                var result = new UiPathProjectAnalyser.UiPathProjectAnalyser(this.folderTextBox.Text);
                this.projectPanel.DataContext = result;
                this.CTreeView.ItemsSource = result.CallHierarchies;
                this.listView.DataContext = result.WorkFlows;
                this.activityListView.DataContext = result.WorkFlows.FirstOrDefault()?.ActivityLists;
                ChartData.SetTotalActivityData(result.WorkFlows,result.TotalAvtivityCount);
            }
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }
}
