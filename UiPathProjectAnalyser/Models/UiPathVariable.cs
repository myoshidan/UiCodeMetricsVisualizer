using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathProjectAnalyser.Models
{
    public class UiPathVariable
    {
        public string Name { get; set; }
        public string TypeArguments { get; set; }
        public string AnnotationText { get; set; }
    }
}
