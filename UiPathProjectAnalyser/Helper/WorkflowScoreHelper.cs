using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathProjectAnalyser.Helper
{
    public static class WorkflowScoreHelper
    {
        public static int VariableScore(int variableCount)
        {
            if (variableCount > 20) return 0;
            if (variableCount > 16) return 4;
            if (variableCount > 12) return 8;
            if (variableCount > 8) return 12;
            if (variableCount > 4) return 16;
            return 20;
        }

        public static int ActivityScore(int activityCount)
        {
            if (activityCount > 80) return 0;
            if (activityCount > 60) return 4;
            if (activityCount > 40) return 8;
            if (activityCount > 20) return 12;
            if (activityCount > 10) return 16;
            return 20;
        }

        public static int ComplexityScore(int cyclomaticComplexity)
        {
            if (cyclomaticComplexity > 20) return 0;
            if (cyclomaticComplexity > 16) return 4;
            if (cyclomaticComplexity > 12) return 8;
            if (cyclomaticComplexity > 8) return 12;
            if (cyclomaticComplexity > 4) return 16;
            return 20;
        }

        public static int DepthScore(int depth)
        {
            if (depth > 10) return 0;
            if (depth > 8) return 4;
            if (depth > 6) return 8;
            if (depth > 4) return 12;
            if (depth > 2) return 16;
            return 20;
        }

        public static int DelayScore(int delayCount)
        {
            if (delayCount > 10) return 0;
            if (delayCount > 6) return 4;
            if (delayCount > 4) return 8;
            if (delayCount > 2) return 12;
            if (delayCount > 0) return 16;
            return 20;
        }
    }
}
