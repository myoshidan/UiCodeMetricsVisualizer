using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UiPathProjectAnalyser.Models;

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
            //pieChart.DataContext = ChartData;
            multiLineChart.DataContext = ChartData;

            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItems.Count == 0) return;
            var activityList = (listView.SelectedItems[0] as UiPathWorkFlow).ActivityLists;
            this.activityListView.DataContext = activityList;
            var variableList = (listView.SelectedItems[0] as UiPathWorkFlow).VariableLists;
            this.variableListView.DataContext = variableList;
            ChartData.SetActivityData(activityList);
        }

        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.folderTextBox.Text = dlg.FileName;
            }
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.folderTextBox.Text)) {
                await this.ShowMessageAsync("Warn", "project.jsonを指定してください");
                return;
            }

            if (!File.Exists(this.folderTextBox.Text))
            {
                await this.ShowMessageAsync("Warn", "project.jsonが見つかりません");
                return;
            }

            UiPathProjectAnalyser.UiPathProjectAnalyser result;
            try
            {
                result = new UiPathProjectAnalyser.UiPathProjectAnalyser(this.folderTextBox.Text);
                this.projectPanel.DataContext = result;
                this.CTreeView.ItemsSource = result.CallHierarchies;
                this.listView.DataContext = result.WorkFlows;

                this.activityListView.DataContext = result.WorkFlows.FirstOrDefault()?.ActivityLists;
                this.variableListView.DataContext = result.WorkFlows.FirstOrDefault()?.VariableLists;                
                ChartData.SetTotalActivityData(result.WorkFlows, result.TotalAvtivityCount);
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", ex.Message);
                return;
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
