using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UiPathProjectAnalyser.Models
{
    public class UiPathActivity
    {
        public string ActivityName { get; set; }
        public string DisplayName { get; set; }
        public string AnnotationText { get; set; }
        public XElement Element { get; set; }
    }
}
