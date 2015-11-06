using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
namespace NoiseMonitoring.Utils
{
    class LogWriter
    {
        private static LogWriter instance;
        private static Queue<Log> logQueue;
        private static string logDir = ConfigurationManager.AppSettings["dir"];
        private static string logFile = ConfigurationManager.AppSettings["logFile"];
        private static int maxLogAge = 1;
        private static int queueSize = 1;
        private static DateTime LastFlushed = DateTime.Now;

        private LogWriter() { }

        public static LogWriter Instance
        {
            get
            {
                // If the instance is null then create one and init the Queue
                if (instance == null)
                {
                    instance = new LogWriter();
                    logQueue = new Queue<Log>();
                }
                return instance;
            }
        }

         public void WriteToLog(string message)
        {
            // Lock the queue while writing to prevent contention for the log file
            lock (logQueue)
            {
                // Create the entry and push to the Queue
                Log logEntry = new Log(message);
                logQueue.Enqueue(logEntry);

                // If we have reached the Queue Size then flush the Queue
                if (logQueue.Count >= queueSize )
                {
                    FlushLog();
                }
            }            
        }

        private bool DoPeriodicFlush()
        {
            TimeSpan logAge = DateTime.Now - LastFlushed;
            if (logAge.TotalSeconds >= maxLogAge)
            {
                LastFlushed = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FlushLog()
        {
            while (logQueue.Count > 0)
            {
                Log entry = logQueue.Dequeue();
                //string logPath = logDir + entry.LogDate + "_" + logFile;
                string logPath = entry.LogDate + "_" + logFile;
        // This could be optimised to prevent opening and closing the file for each write
                using (System.IO.FileStream fs = System.IO.File.Open(logPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter log = new StreamWriter(fs))
                    {
                        log.WriteLine(string.Format("{0}\t{1}",entry.LogTime,entry.Message));
                    }
                }
            }            
        }
    }

    public class Log
    {
        public string Message { get; set; }
        public string LogTime { get; set; }
        public string LogDate { get; set; }

        public Log(string message)
        {
            Message = message;
            LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            LogTime = DateTime.Now.ToString("HH:mm:ss.fff");
        }
    }
}
