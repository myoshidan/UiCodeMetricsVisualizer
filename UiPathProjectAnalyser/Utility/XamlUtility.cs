using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UiPathProjectAnalyser.Utility
{
    public static class XamlUtility
    {
        public static XDocument ReadXamlFile(string filePath)
        {
            return XDocument.Load(filePath);
        }

    }
}
