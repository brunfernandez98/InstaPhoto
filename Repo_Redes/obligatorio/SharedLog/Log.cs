using System;
using SharedLogConstant;

namespace SharedLog
{
    public class Log
    {
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public LogEnum Type { get; set; }
        public LogEnumSeverity TypeSeverity { get; set; }
        public Log()
        {
            Date = DateTime.Now;
        }
        public Log(string oneAuthor, LogEnum oneType,LogEnumSeverity oneTypeSeverity)
        {
            Author = oneAuthor;
            Date = DateTime.Now;
            Type = oneType;
            TypeSeverity = oneTypeSeverity;
        }

        public string ToString()
        {
            return Author + " realizo:" + Type + " - " + Date + " Con Severidad:" + TypeSeverity;
        }
    }
}