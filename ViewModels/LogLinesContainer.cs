using System.Collections.Generic;

namespace LogViewer3.ViewModels
{
    public struct LogLinesContainer
    {
        public LogLine[] Lines { get; set; }
        public int Percentage { get; set; }
    }
}
