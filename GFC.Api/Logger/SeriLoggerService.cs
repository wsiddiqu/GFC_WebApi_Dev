using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GFC.Api.Logger
{
    public class SeriLoggerService
    {
        #region Fields
        /// <summary>
        /// Private singleton instance.
        /// </summary>
        private static readonly Lazy<SeriLoggerService> LazyInstance =
            new Lazy<SeriLoggerService>(() => new SeriLoggerService());

        /// <summary>
        /// Name of the log file created.
        /// </summary>
        private string _logFileName = "GFCApiServer.log";

        /// <summary>
        /// The name of the directory inside the installation directory.
        /// </summary>
        private string _gfcApiLogDirectory = "GFCApiDebug";
        #endregion

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="SeriLoggerService"/> class from being created.
        /// </summary>
        private SeriLoggerService()
        {
            //Do nothing
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton class instance.
        /// </summary>
        public static SeriLoggerService Instance => SeriLoggerService.LazyInstance.Value;

        /// <summary>
        /// The logger instance
        /// </summary>
        public Serilog.Core.Logger log = null;

        #endregion
        /// <summary>
        /// Gets the debug log directory from the registry. Returns a default directory in case 
        /// the entry is not found.
        /// </summary>
        /// <returns>The debug log directory path</returns>
        private string GetFASTLogDir()
        {
            string baseLogPath = @"C:\Program Files\TalonFAST\FASTDebugLogs";
            try
            {
                //Get the registry entry for the debug log directory
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    var TalonRegKey = hklm.OpenSubKey(@"Software\Talon");
                    if (TalonRegKey != null)
                    {
                        baseLogPath = (string)TalonRegKey.GetValue("DebugLogDir");
                        if (string.IsNullOrWhiteSpace(baseLogPath))
                        {
                            baseLogPath = @"C:\Program Files\TalonFAST\FASTDebugLogs";
                        }
                    }
                }
            }
            catch (Exception e)
            {
               
            }

            string lmsLogPath = baseLogPath + "\\" + _gfcApiLogDirectory;

            if (!Directory.Exists(lmsLogPath))
                Directory.CreateDirectory(lmsLogPath);

            return baseLogPath;
        }

        /// <summary>
        /// Initializes the logger with log creation path and logger configuration settings.
        /// </summary>
        public void Init()
        {
            string logFilePath = GetFASTLogDir() + "\\" + _logFileName;

            //Initialize the logger
            log = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.File(logFilePath)
                .CreateLogger();
        }
    }
}
