namespace LogViewer3.ViewModels
{
    public class LogLine
    {
        public int LineNumber { get; set; }
        public string Data { get; set; }
        public string BackgroundColor
        {
            get
            {
                if (Data.Contains("DEBUG")) return "#F15500";
                return null;
            }
        }

        public string ForegroundColor
        {
            get
            {
                if (Data.Contains("DEBUG")) return "#FF0078D7";
                if (Data.Contains("ERROR")) return "#C50500";
                return "#111111";
            }
        }
    }
}
