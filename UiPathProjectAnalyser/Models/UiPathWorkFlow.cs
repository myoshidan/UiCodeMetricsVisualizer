using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UiPathProjectAnalyser.Helper;
using UiPathProjectAnalyser.Utility;

namespace UiPathProjectAnalyser.Models
{
    public class UiPathWorkFlow
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int CyclomaticComplexity { get; set; } = 1;
        public int ActivityCount { get; set; } = 0;
        public int ActivityKind { get; set; } = 0;
        public int TryCatchCount { get; set; } = 0;
        public int RetryScopeCount { get; set; } = 0;
        public int DelayCount { get; set; } = 0;
        public int VariableCount { get; set; } = 0;
        public int LogCount { get; set; } = 0;
        public int AnnotationCount { get; set; } = 0;
        public int NestedCount { get; set; } = 0;
        public int WorkflowScore { get; set; } = 0;


        public ObservableCollection<UiPathVariable> VariableLists { get; set; } = new ObservableCollection<UiPathVariable>();
        public ObservableCollection<UiPathActivity> ActivityLists { get; set; } = new ObservableCollection<UiPathActivity>();

        public ObservableCollection<string> InvokeFiles { get; set; }

        private XDocument Document { get; set; }

        public UiPathWorkFlow(string filePath)
        {
            var complexActivity = new List<string>()
            {
                "If","FlowDecision","While","ForEach","Switch"
            };

            this.FilePath = filePath;
            this.FileName = Path.GetFileName(filePath);
            this.Document = XamlUtility.ReadXamlFile(filePath);
            var ActivitieItems = this.Document.Descendants().Where(elem => elem.Attribute("DisplayName") != null).ToList();
            this.ActivityCount = ActivitieItems.Count();
            foreach (var item in ActivitieItems)
            {
                var activity = new UiPathActivity();
                activity.ActivityName = item.Name.LocalName;
                activity.DisplayName = item.Attribute("DisplayName").Value;
                activity.Element = item;
                ActivityLists.Add(activity);

                if (complexActivity.Where(rule => item.Name.LocalName.Contains(rule)).Count() > 0)
                {
                    CyclomaticComplexity++;
                };

                if (item.Attributes().Where(p => p.Name.LocalName == "Annotation.AnnotationText").Count() > 0)
                {
                    activity.AnnotationText = item.Attributes().Where(p => p.Name.LocalName == "Annotation.AnnotationText").FirstOrDefault().Value;
                    AnnotationCount++;
                }

                if (item.Name.LocalName == "TryCatch")
                {
                    TryCatchCount++;
                }
                else if(item.Name.LocalName == "RetryScope")
                {
                    RetryScopeCount++;
                }
                else if (item.Name.LocalName == "Delay")
                {
                    DelayCount++;
                }
                else if (item.Name.LocalName == "LogMessage")
                {
                    LogCount++;
                }

            }
            this.ActivityKind = this.ActivityLists.GroupBy(p => p.ActivityName).Count();

            var variables = this.Document.Descendants().Where(elem => elem.Name.LocalName == "Variable").ToList();
            foreach (var item in variables)
            {
                var variable = new UiPathVariable();
                variable.Name = item.Attribute("Name").Value;
                var type = item.Attributes().Where(x => x.Name.LocalName == "TypeArguments")?.FirstOrDefault()?.Value;
                if (type.Contains(":")) type = type.Substring(type.IndexOf(":") + 1).Replace("x:","");
                variable.TypeArguments = type;
                variable.AnnotationText = item.Attributes().Where(x => x.Name.LocalName == "Annotation.AnnotationText")?.FirstOrDefault()?.Value;
                this.VariableLists.Add(variable);
            }
            this.VariableCount = VariableLists.Count();

            var count = 0;
            var max = 0;
            var idList = new List<string>();
            GetXElementSeq(this.Document.Root.Elements().Where(elem => elem.Attribute("DisplayName") != null)?.FirstOrDefault(), ref count,ref max,ref idList);
            this.NestedCount = max;

            this.WorkflowScore = WorkflowScoreHelper.VariableScore(VariableCount) +
                            WorkflowScoreHelper.ActivityScore(ActivityCount) +
                            WorkflowScoreHelper.ComplexityScore(CyclomaticComplexity) +
                            WorkflowScoreHelper.DepthScore(NestedCount) +
                            WorkflowScoreHelper.DelayScore(DelayCount);

            this.InvokeFiles = new ObservableCollection<string>(this.Document.Descendants().Where(elem => elem.Name.LocalName == "InvokeWorkflowFile").Select(elem => elem.Attribute("WorkflowFileName").Value).Distinct());

        }

        private void GetXElementSeq(XElement elm,ref int depth,ref int maxDepth,ref List<String>idLists)            
        {
            if (elm == null) return;
            var id = elm.Attributes().Where(x => x.Name.LocalName == "WorkflowViewState.IdRef")?.FirstOrDefault()?.Value;
            if (idLists.Contains(id)) return;
            idLists.Add(id);

            if (elm.Name.LocalName == "Sequence" || elm.Name.LocalName == "Flowchart" || elm.Name.LocalName == "StateMachine")
            {
                depth++;
            }

            foreach (var item in elm.Descendants()
                                    .Where(elem => elem.Attribute("DisplayName") != null)
                                    .Where(x => x.Name.LocalName == "Sequence" || x.Name.LocalName == "Flowchart" || x.Name.LocalName == "StateMachine"))
            {
                GetXElementSeq(item, ref depth,ref maxDepth,ref idLists);
            }

            if (elm == null || elm.Descendants()
                        .Where(elem => elem.Attribute("DisplayName") != null)
                        .Where(x => x.Name.LocalName == "Sequence" || x.Name.LocalName == "Flowchart" || x.Name.LocalName == "StateMachine")
                        .Count() == 0)
            {
                if (maxDepth < depth) maxDepth = depth;
                depth --;
            }
        }

    }
}
