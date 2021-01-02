using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPathProjectAnalyser.Helper;
using UiPathProjectAnalyser.Models;

namespace UiPathProjectAnalyser
{
    public class UiPathProjectAnalyser
    {
        public string ProjectFolderPath { get; set; }
        public string ProjectFilePath { get; set; }
        public UiPathProject Project { get; set; }
        public int WorkflowFileCount { get; set; }
        public int TotalAvtivityCount { get; set; }
        public int TotalAvtivityKind { get; set; }
        public int TotalVariableCount { get; set; }
        public int TotalCyclomaticComplexity { get; set; }
        public int MaxNestedCount { get; set; }
        public int WorkflowScoreAverage { get; set; }

        public ObservableCollection<CallHierarchy> CallHierarchies { get; set; } = new ObservableCollection<CallHierarchy>();

        public ObservableCollection<UiPathWorkFlow> WorkFlows { get; set; } = new ObservableCollection<UiPathWorkFlow>();
        private List<string> XamlFiles { get; set; }


        public UiPathProjectAnalyser(string jsonFilePath)
        {
            this.ProjectFilePath = jsonFilePath;
            this.ProjectFolderPath = Path.GetDirectoryName(jsonFilePath);
            this.Project = GetProjectInfo(jsonFilePath);
            if (this.Project.name == null) return;
            this.XamlFiles = GetXamlFiles(ProjectFolderPath);
            this.WorkflowFileCount = this.XamlFiles.Count();
            foreach (var file in XamlFiles)
            {
                WorkFlows.Add(new UiPathWorkFlow(file));
            }
            if (WorkFlows.Count == 0) return;
            this.TotalAvtivityCount = WorkFlows.Select(x => x.ActivityCount).Sum();
            this.TotalVariableCount = WorkFlows.Select(x => x.VariableCount).Sum();
            this.MaxNestedCount = WorkFlows.Select(x => x.NestedCount).Max();
            this.TotalCyclomaticComplexity = WorkFlows.Select(x => x.CyclomaticComplexity).Sum();
            this.TotalAvtivityKind= WorkFlows.SelectMany(x => x.ActivityLists).GroupBy(p => p.ActivityName).Count();
            this.WorkflowScoreAverage = (int)WorkFlows.Select(x => x.WorkflowScore).Average();
            this.CallHierarchies = FetchCallHierarchy(this.Project.main);

        }

        private UiPathProject GetProjectInfo(string jsonFilePath)
        {
            using (var sr = new StreamReader(jsonFilePath, System.Text.Encoding.UTF8))
            {
                var jsonData = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<UiPathProject>(jsonData);
            }
        }

        private List<string> GetXamlFiles(string projectFolder)
        {
            return Directory.GetFiles(projectFolder, "*", SearchOption.AllDirectories).ToList().Where(p => p.EndsWith(".xaml")).ToList();
        }

        private ObservableCollection<CallHierarchy> FetchCallHierarchy(string root)
        {
            var rootHierarchy = new CallHierarchy(root);
            var hierarchies = new ObservableCollection<CallHierarchy>();
            hierarchies.Add(rootHierarchy);

            var rootWorkflow = WorkFlows.Where(x => x.FileName.Equals(root)).FirstOrDefault();
            if (rootWorkflow == null) return null;

            foreach (var item in rootWorkflow.InvokeFiles.Distinct())
            {
                GetHierarchy(item, ref rootHierarchy);
            }


            return hierarchies ;
        }

        private void GetHierarchy(string fileName, ref CallHierarchy hierarchy)
        {
            var workflow = WorkFlows.Where(x => x.FilePath.Contains(fileName)).FirstOrDefault();
            var childHierarchy = new CallHierarchy(fileName);
            hierarchy.CallHierarchies.Add(childHierarchy);

            foreach (var item in workflow.InvokeFiles)
            {
                GetHierarchy(item, ref childHierarchy);
            }
        }
    }
}
