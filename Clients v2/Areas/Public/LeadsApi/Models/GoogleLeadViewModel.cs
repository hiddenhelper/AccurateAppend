#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable InconsistentNaming - this is a JSON payload representation
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Models
{
    /// <summary>
    /// Lead structure used by FormStack form post
    /// </summary>
    [Serializable()]
    public class GoogleLeadViewModel : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleLeadViewModel"/> class.
        /// </summary>
        /// <remarks>
        /// Ensures the <see cref="user_column_data"/> is not null.
        /// </remarks>
        public GoogleLeadViewModel()
        {
            this.user_column_data = new List<UserData>();
        }

        public String Email
        {
            get
            {
                return this.user_column_data
                    .FirstOrDefault(a => a.column_id == UserDataKeys.Email)
                    ?.string_value ?? String.Empty;
            }
        }

        public String Phone
        {
            get
            {
                return this.user_column_data
                           .FirstOrDefault(a => a.column_id == UserDataKeys.Phone)
                           ?.string_value ?? String.Empty;
            }
        }

        public String lead_id { get; set; }

        public String api_version { get; set; }

        public String form_id { get; set; }

        public Int32 campaign_id { get; set; }

        public String google_key { get; set; }
        
        public Boolean is_test { get; set; }

        public String gcl_id { get; set; }

        public ICollection<UserData> user_column_data { get; }

        public String HandshakeKey { get; set; }

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!String.Equals(this.HandshakeKey, "CF1311A0-6332-4CF1-A2E2-3B02AA64771D")) yield return new ValidationResult("Your request was unable to be processed. Bad request.", new[] {nameof(this.HandshakeKey)});

            if (String.IsNullOrWhiteSpace(this.Email) && String.IsNullOrWhiteSpace(this.Phone)) yield return new ValidationResult($"{nameof(this.Email)} OR {nameof(this.Phone)} must be supplied.", new[] { nameof(this.HandshakeKey) });
        }

        #endregion
    }

    /// <summary>
    /// Class used by Google to store form values as name+value pairs.
    /// Significant keys are contained in the <see cref="UserDataKeys"/> constants.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("{" + nameof(column_name) + ("}={" + nameof(string_value) + "}"))]
    public class UserData
    {
        /// <summary>
        /// The name of the key column.
        /// </summary>
        public String column_name { get; set; }

        /// <summary>
        /// The value associated with the key.
        /// </summary>
        public String string_value { get; set; }

        /// <summary>
        /// The name of the key.
        /// </summary>
        public String column_id { get; set; }
    }

    /// <summary>
    /// Contains constants for the <see cref="UserData"/> <see cref="UserData.column_id"/> key values we use.
    /// </summary>
    public static class UserDataKeys
    {
        /// <summary>
        /// Contains the key "Full Name".
        /// </summary>
        public const String FullName = "FULL_NAME";

        /// <summary>
        /// Contains the key "User Email".
        /// </summary>
        public const String Email = "EMAIL";

        /// <summary>
        /// Contains the key "User Phone".
        /// </summary>
        public const String Phone = "PHONE_NUMBER";
    }
}