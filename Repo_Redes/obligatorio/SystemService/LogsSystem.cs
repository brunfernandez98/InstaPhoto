using System.Collections.Generic;
using SharedLog;

namespace SystemService
{
    public class LogsSystem
    {
        public List<Log> logList;
        private static LogsSystem _instance;

        public LogsSystem()
        {
            logList=new List<Log>();
        }
        
        
        public static LogsSystem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LogsSystem();
            }
            return _instance;
        }
    }
}