using System;

namespace AccurateAppend.Websites.Admin.TempDataExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public class AlertMessage
    {
        /// <summary>
        /// Alert type
        /// </summary>
        public AlertType Type { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public string Body { get; set; }
    }

    /// <summary>
    /// Bootstrap alert types
    /// </summary>
    /// <remarks>
    /// Members are used in Css and need to be lower case
    /// </remarks>
    public enum AlertType
    {
        /// <summary>
        /// Success alert
        /// </summary>
        success,
        /// <summary>
        /// Warning alert
        /// </summary>
        warning,
        /// <summary>
        /// Danger alert
        /// </summary>
        danger
    }
}