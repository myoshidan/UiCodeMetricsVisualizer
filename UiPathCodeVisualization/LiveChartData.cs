using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPathProjectAnalyser.Models;

namespace UiPathCodeVisualization
{
    public class LiveChartData
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public SeriesCollection PieSeriesCollection { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }

        public LiveChartData()
        {
            SeriesCollection = new SeriesCollection();
            PieSeriesCollection = new SeriesCollection();
            Labels = new List<string>();
            Formatter = value => value.ToString("N");
            PointLabel = chartPoint =>
                string.Format("{0} ({1}%)", chartPoint.Y, (int)(chartPoint.Participation*100));
        }


        public void SetTotalActivityData(ObservableCollection<UiPathWorkFlow> workFlows,int totalActivityCount)
        {
            var activities = workFlows.SelectMany(x => x.ActivityLists).GroupBy(p => p.ActivityName).OrderByDescending(p => p.Count());
            var count = 0;
            var actCount = 0;
            var collection = new List<PieSeries>();

            foreach (var item in activities)
            {
                if(count < 5)
                {
                    collection.Add(new PieSeries()
                    {
                        Title = item.Key,
                        Values = new ChartValues<int> { item.Count() },
                        LabelPoint = PointLabel,
                        LabelPosition = PieLabelPosition.InsideSlice,
                        DataLabels = true
                    }) ;
                    count += 1;
                    actCount += item.Count();
                }
                else
                {
                    collection.Add(new PieSeries()
                    {
                        Title = "Other",
                        Values = new ChartValues<int> { totalActivityCount - actCount },
                        LabelPoint = PointLabel,
                        LabelPosition = PieLabelPosition.InsideSlice,
                        DataLabels = true
                    });
                    break;
                }
            }

            this.PieSeriesCollection.Clear();
            this.PieSeriesCollection.AddRange(collection);
        }

        public void SetActivityData(ObservableCollection<UiPathActivity> activityLists)
        {
            var actCount = activityLists.GroupBy(p => p.ActivityName).OrderByDescending(p => p.Count()).Select(p => p.Count());
            var actKey = activityLists.GroupBy(p => p.ActivityName).OrderByDescending(p => p.Count()).Select(p => p.Key);
            if (actCount.Count() > 10) actCount = actCount.Take(10);
            if (actKey.Count() > 10) actKey = actKey.Take(10);

            this.SeriesCollection.Clear();
            this.SeriesCollection.Add(new RowSeries
            {
                Title = "Activity Top10",
                Values = new ChartValues<int>(actCount.Reverse())
            });
            this.Labels.Clear();            
            this.Labels.AddRange(actKey.Reverse());
        }
    }
}
