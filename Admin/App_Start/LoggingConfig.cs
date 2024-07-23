using System.Diagnostics;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Core.Definitions;
using EventLogger;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Boostrapper for configuring the application level logging and support.
    /// </summary>
    public class LoggingConfig : AbstractBootstrapper
    {
        #region Overrides

        /// <summary>Signals the object that initialization is starting.</summary>
        public override void BeginInit()
        {
            if (this.IsInitialized) return;

            // Override to use the new Azure queue
            Logger.Loggers.Clear();
            Logger.Loggers.Add(new EventLogger.AzureQueue.QueueLogger());

#if DEBUG
            if (Debugger.IsAttached)
            {
                // Clear out loggers if we're debugging so we don't cause crap alerts
                Logger.Loggers.Clear();
            }
#endif
            // Configure the global app for logging
            Logger.GlobalOverride(Application.AccurateAppend_Admin);
        }

        /// <inheritdoc />
        public override void EndInit()
        {
        }

        #endregion

        #region Helper

        /// <summary>
        /// Provides a convienent abstraction over this component to run everything in one call.
        /// </summary>
        public static void Execute()
        {
            var boostrapper = new LoggingConfig();
            boostrapper.Run();
        }

        #endregion
    }
}