using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Logging;
using GuaranteedRate.Sextant.WebClients;
using Nest;

namespace GuaranteedRate.Sextant.Logging.File
{
    public class FileLogAppender : AsyncEventReporter, ILogAppender
    {
        //private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        protected override string Name { get; } = typeof(FileLogAppender).Name;
        private static ISet<string> _tags;


        #region config mappings

        public static string FILE_ENABLED = "FileLogAppender.Enabled";
        public static string LOG_FOLDER = "FileLogAppender.Folder";
        public static string LOG_NAME = "FileLogAppender.LogName";
        public static string FILE_QUEUE_SIZE = "FileLogAppender.QueueSize";
        public static string FILE_RETRY_LIMIT = "FileLogAppender.RetryLimit";
        public static string FILE_ALL = "FileLogAppender.All.Enabled";
        public static string FILE_ERROR = "FileLogAppender.Error.Enabled";
        public static string FILE_WARN = "FileLogAppender.Warn.Enabled";
        public static string FILE_INFO = "FileLogAppender.Info.Enabled";
        public static string FILE_DEBUG = "FileLogAppender.Debug.Enabled";
        public static string FILE_FATAL = "FileLogAppender.Fatal.Enabled";
        public static string FILE_TAGS = "FileLogAppender.Tags";

        #endregion



        public void Log(IDictionary<string, string> fields)
        {
            PostEvent(fields);
        }

        public void AddTag(string tag)
        {
            _tags.Add(tag);
        }

        public FileLogAppender(IEncompassConfig config)
            : base(config.GetValue(FILE_QUEUE_SIZE, 1000),
                config.GetValue(FILE_RETRY_LIMIT, 3))
        {
            _tags = new HashSet<string>();
            Setup(config);
        }

        public void Setup(IEncompassConfig config)
        {

            var allEnabled = config.GetValue(FILE_ALL, true);
            var errorEnabled = config.GetValue(FILE_ERROR, true);
            var warnEnabled = config.GetValue(FILE_WARN, true);
            var infoEnabled = config.GetValue(FILE_INFO, true);
            var debugEnabled = config.GetValue(FILE_DEBUG, true);
            var fatalEnabled = config.GetValue(FILE_FATAL, true);
            LogFolder = config.GetValue(LOG_FOLDER, System.IO.Path.GetTempPath());
            LogName  = config.GetValue(LOG_NAME, "Sextant_log");
            AllEnabled = allEnabled;
            ErrorEnabled = allEnabled || errorEnabled;
            WarnEnabled = allEnabled || warnEnabled;
            InfoEnabled = allEnabled || infoEnabled;
            DebugEnabled = allEnabled || debugEnabled;
            FatalEnabled = allEnabled || fatalEnabled;
        }

        private double maxFileSizeBytes = 100000;
        public bool AllEnabled { get; set; }
        public bool DebugEnabled { get; private set; }
        public bool InfoEnabled { get; private set; }
        public bool WarnEnabled { get; private set; }
        public bool ErrorEnabled { get; private set; }
        public bool FatalEnabled { get; private set; }
        public string LogFolder { get; private set; }
        public double MaxLogAgeInHours = 12;
        public DateTime LastLogScrub = DateTime.UtcNow;
        public string LogName { get; private set; }

        private  object locker = new object();

        private void DeleteOldLogs()
        {
            foreach (var f in System.IO.Directory.GetFiles(LogFolder))
            {
                var fi = new FileInfo(f);
                if (fi.Name.StartsWith(LogName))
                {
                    if ((DateTime.UtcNow - fi.CreationTime.ToUniversalTime()).TotalHours > MaxLogAgeInHours)
                    {
                        fi.Delete();
                    }
                }
            }
        }

        private string GetFilePath()
        {
            if ((DateTime.UtcNow - LastLogScrub).TotalMinutes > 5)
            {
                DeleteOldLogs();
                LastLogScrub=DateTime.UtcNow;
            }
            return Path.Combine(LogFolder,
                $"{LogName}-{DateTime.UtcNow.Year}-{DateTime.UtcNow.Month}-{DateTime.UtcNow.Day}-{DateTime.UtcNow.Hour}.log");
        }

        protected override bool PostEvent(object data)
        {
            try
            {
                var path = GetFilePath();
                var logEvent = SimpleLogEvent.Create(data, _tags);
                if (logEvent != null)
                {
                    lock (locker)
                    {
                        try
                        {
                            using (
                                var fs = new System.IO.FileStream(path, FileMode.Append, FileAccess.Write,
                                    FileShare.None))
                            {
                                StreamWriter writer = new StreamWriter(fs);
                                writer.WriteLine(
                                    $"{logEvent.level}: {logEvent.hostname} - {logEvent.timestamp} - {logEvent.process} - {logEvent.message} - tags: {String.Join(",", _tags)}");
                                writer.Flush();
                            }
                        }
                        catch (System.IO.IOException iox)
                        {
                            return false;
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                }
            }
            catch
                (Exception ex)
            {
                Logger.Warn(Name, $"The file logger request returned an error {ex.Message}");
                return false;
            }

            return true;
        }

    }
}
