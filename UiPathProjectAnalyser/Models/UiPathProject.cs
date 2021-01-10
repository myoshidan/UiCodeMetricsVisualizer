using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UiPathProjectAnalyser.Models
{

    public class UiPathProject
    {
        public string name { get; set; }
        public string description { get; set; }
        public string main { get; set; }
        public Dependencies dependencies { get; set; }
        public object[] webServices { get; set; }
        public string schemaVersion { get; set; }
        public string studioVersion { get; set; }
        public string projectVersion { get; set; }
        public Runtimeoptions runtimeOptions { get; set; }
        public Designoptions designOptions { get; set; }
        public string expressionLanguage { get; set; }
    }

    public class Dependencies
    {
        public string UiPathExcelActivities { get; set; }
        public string UiPathMailActivities { get; set; }
        public string UiPathSystemActivities { get; set; }
        public string UiPathUIAutomationActivities { get; set; }
    }

    public class Runtimeoptions
    {
        public bool autoDispose { get; set; }
        public bool isPausable { get; set; }
        public bool requiresUserInteraction { get; set; }
        public bool supportsPersistence { get; set; }
        public string[] excludedLoggedData { get; set; }
        public string executionType { get; set; }
    }

    public class Designoptions
    {
        public string projectProfile { get; set; }
        public string outputType { get; set; }
        public Libraryoptions libraryOptions { get; set; }
        public object[] fileInfoCollection { get; set; }
    }

    public class Libraryoptions
    {
        public bool includeOriginalXaml { get; set; }
        public object[] privateWorkflows { get; set; }
    }

    public class UiPathProject2016
    {
        public string description { get; set; }
        public string version { get; set; }
        public string main { get; set; }
        public string id { get; set; }
        public Dependencies dependencies { get; set; }
        public Configurationoptions configurationOptions { get; set; }
        public string[] excludedData { get; set; }
    }

    public class Configurationoptions
    {
    }
}
