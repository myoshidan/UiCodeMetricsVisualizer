using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathProjectAnalyser.Models
{
    public class CallHierarchy
    {
        public string Name { get; set; }
        public List<CallHierarchy> CallHierarchies { get; set; }

        public CallHierarchy(string name)
        {
            this.Name = name;
            CallHierarchies = new List<CallHierarchy>();
        }
    }
}
