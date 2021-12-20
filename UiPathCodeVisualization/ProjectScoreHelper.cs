using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPathCodeVisualization.Properties;

namespace UiPathCodeVisualization
{

    public static class ProjectScoreHelper
    {
        public static int VariableScore(int variableCount)
        {
            if (variableCount > Properties.Settings.Default.VariableLv1) return 0;
            if (variableCount > Properties.Settings.Default.VariableLv2) return 5;
            if (variableCount > Properties.Settings.Default.VariableLv3) return 10;
            if (variableCount > Properties.Settings.Default.VariableLv4) return 15;
            if (variableCount > Properties.Settings.Default.VariableLv5) return 20;
            return 25;
        }

        public static int ActivityScore(int activityCount)
        {
            if (activityCount > Properties.Settings.Default.AvtivityLv1) return 0;
            if (activityCount > Properties.Settings.Default.AvtivityLv2) return 5;
            if (activityCount > Properties.Settings.Default.AvtivityLv3) return 10;
            if (activityCount > Properties.Settings.Default.AvtivityLv4) return 15;
            if (activityCount > Properties.Settings.Default.AvtivityLv5) return 20;
            return 25;
        }

        public static int ComplexityScore(int cyclomaticComplexity)
        {
            if (cyclomaticComplexity > Properties.Settings.Default.ComplexityLv1) return 0;
            if (cyclomaticComplexity > Properties.Settings.Default.ComplexityLv2) return 5;
            if (cyclomaticComplexity > Properties.Settings.Default.ComplexityLv3) return 10;
            if (cyclomaticComplexity > Properties.Settings.Default.ComplexityLv4) return 15;
            if (cyclomaticComplexity > Properties.Settings.Default.ComplexityLv5) return 20;
            return 25;
        }

        public static int DepthScore(int depth)
        {
            if (depth > Properties.Settings.Default.DepthLv1) return 0;
            if (depth > Properties.Settings.Default.DepthLv2) return 5;
            if (depth > Properties.Settings.Default.DepthLv3) return 10;
            if (depth > Properties.Settings.Default.DepthLv4) return 15;
            if (depth > Properties.Settings.Default.DepthLv5) return 20;
            return 25;
        }

        public static void ResetProperties()
        {
            Settings.Default.VariableLv1 = Settings.Default.Default_VariableLv1;
            Settings.Default.VariableLv2 = Settings.Default.Default_VariableLv2;
            Settings.Default.VariableLv3 = Settings.Default.Default_VariableLv3;
            Settings.Default.VariableLv4 = Settings.Default.Default_VariableLv4;
            Settings.Default.VariableLv5 = Settings.Default.Default_VariableLv5;

            Settings.Default.AvtivityLv1 = Settings.Default.Default_ActivityLv1;
            Settings.Default.AvtivityLv2 = Settings.Default.Default_ActivityLv2;
            Settings.Default.AvtivityLv3 = Settings.Default.Default_ActivityLv3;
            Settings.Default.AvtivityLv4 = Settings.Default.Default_ActivityLv4;
            Settings.Default.AvtivityLv5 = Settings.Default.Default_ActivityLv5;

            Settings.Default.ComplexityLv1 = Settings.Default.Default_ComplexityLv1;
            Settings.Default.ComplexityLv2 = Settings.Default.Default_ComplexityLv2;
            Settings.Default.ComplexityLv3 = Settings.Default.Default_ComplexityLv3;
            Settings.Default.ComplexityLv4 = Settings.Default.Default_ComplexityLv4;
            Settings.Default.ComplexityLv5 = Settings.Default.Default_ComplexityLv5;

            Settings.Default.DepthLv1 = Settings.Default.Default_DepthLv1;
            Settings.Default.DepthLv2 = Settings.Default.Default_DepthLv2;
            Settings.Default.DepthLv3 = Settings.Default.Default_DepthLv3;
            Settings.Default.DepthLv4 = Settings.Default.Default_DepthLv4;
            Settings.Default.DepthLv5 = Settings.Default.Default_DepthLv5;
        }
    }
}
