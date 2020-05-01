using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FileScannerAPI.Common
{
    public class LoggingSettings
    {
        public LogLevel LogLevel { get; set; }

        public string LogPath { get; set; }
    }

    public class LogLevel
    {
        public LogLevel Default { get; set; }

        public LogLevel System { get; set; }

        public LogLevel Microsoft { get; set; }
    }
}
