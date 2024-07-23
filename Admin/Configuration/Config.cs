using System;
using System.Configuration;

namespace AccurateAppend.Websites.Admin.Configuration
{
    public static class Config
    {
        public static String AccurateAppendDb
        {
            get
            {
                var connection = ConfigurationManager.ConnectionStrings["ASPMembershipDB"];
                if (connection == null) throw new ConfigurationErrorsException("ASPMembershipDB connection string setting is null. Did you forget to include this in your web.config?");

                return connection.ConnectionString;
            }
        }

        public static String EventLogDb => EventLogger.Properties.Settings.Default.AccurateAppendEventLogConnectionString;
    }
}