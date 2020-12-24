
using System.Collections.Generic;
using SharedLog;
using SharedLogConstant;

namespace ServerLogs.Interfaces
{
    public interface ILogsService
    {
        
        IEnumerable<string> GetLogs();
        void SaveLogs(Log oneLog);
    }
}