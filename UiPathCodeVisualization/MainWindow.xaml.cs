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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UiPathProjectAnalyser.Excel;
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

        public ObservableCollection<UiPathProjectAnalyser.UiPathProjectAnalyser> UiPathProjects { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dlg = new CommonOpenFileDialog();
            dlg.Title = "分析対象の親フォルダを指定してください";
            dlg.IsFolderPicker = true;
            dlg.RestoreDirectory = true;

            UiPathProjects = new ObservableCollection<UiPathProjectAnalyser.UiPathProjectAnalyser>();
            BindingOperations.EnableCollectionSynchronization(UiPathProjects, new object());

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
                await this.ShowMessageAsync("Warn", "分析対象の親フォルダを指定してください");
                return;
            }

            if (!Directory.Exists(this.folderTextBox.Text))
            {
                await this.ShowMessageAsync("Warn", "分析対象の親フォルダが見つかりません");
                return;
            }
            try
            {
                var projectJsonList = Directory.EnumerateFiles(this.folderTextBox.Text, "project.json", SearchOption.AllDirectories);
                if(projectJsonList.Count() == 0)
                {
                    await this.ShowMessageAsync("Warn", "指定フォルダ内にproject.jsonファイルが見つかりません");
                    return;
                }

                UiPathProjects.Clear();

                progress.IsActive = true;

                await Task.Run(() =>
                {
                    foreach (var projectJsonFile in projectJsonList)
                    {
                        UiPathProjects.Add(new UiPathProjectAnalyser.UiPathProjectAnalyser(projectJsonFile));
                    }
                });

                progress.IsActive = false;

                var result = UiPathProjects.FirstOrDefault();
                this.projectPanel.DataContext = result;
                this.CTreeView.ItemsSource = result.CallHierarchies;
                this.listView.DataContext = result.WorkFlows;
                this.libraryListView.DataContext = result.LibraryLists;
                this.activityListView.DataContext = result.WorkFlows.FirstOrDefault()?.ActivityLists;
                this.variableListView.DataContext = result.WorkFlows.FirstOrDefault()?.VariableLists;
                ChartData.SetTotalActivityData(result.WorkFlows, result.TotalAvtivityCount);

                this.projectListView.DataContext = UiPathProjects;
            }
            catch (Exception ex)
            {
                progress.IsActive = false;
                await this.ShowMessageAsync("Error",$"{ex.Message},{Environment.NewLine},{ex.StackTrace}");
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


        private void HamburgerMenu_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            switch ((args.InvokedItem as HamburgerMenuItem).Label)
            {
                case "Search":
                    SearchGrid.Visibility = Visibility.Visible;
                    AnalysisGrid.Visibility = Visibility.Hidden;
                    break;
                case "Analysis":
                    AnalysisGrid.Visibility = Visibility.Visible;
                    SearchGrid.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }

        }

        private void projectListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (projectListView.SelectedItems.Count == 0) return;
            var result = projectListView.SelectedItems[0] as UiPathProjectAnalyser.UiPathProjectAnalyser;
            this.projectPanel.DataContext = result;
            this.CTreeView.ItemsSource = result.CallHierarchies;
            this.listView.DataContext = result.WorkFlows;
            this.libraryListView.DataContext = result.LibraryLists;
            this.activityListView.DataContext = result.WorkFlows.FirstOrDefault()?.ActivityLists;
            this.variableListView.DataContext = result.WorkFlows.FirstOrDefault()?.VariableLists;
            ChartData.SetTotalActivityData(result.WorkFlows, result.TotalAvtivityCount);
            AnalysisGrid.Visibility = Visibility.Visible;
            SearchGrid.Visibility = Visibility.Hidden;

            hamburgerMenu.SelectedIndex = 1;
        }

        private async void exportButton_Click(object sender, RoutedEventArgs e)
        {
            var save_dlg = new CommonOpenFileDialog();
            save_dlg.Title = "保存先Excelファイル名を指定してください";
            save_dlg.DefaultFileName = $"UiPathプロジェクト一覧_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            save_dlg.Filters.Add(new CommonFileDialogFilter("Excelファイル", "*.xlsx"));
            save_dlg.RestoreDirectory = true;

            if (save_dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var excelinstance = new ExportReportExcel(UiPathProjects.ToList(), save_dlg.FileName);
                excelinstance.CreateProjectListSheets();
                await this.ShowMessageAsync("保存成功", "Excelファイル出力が完了しました。");
            }
        }
    }
}
