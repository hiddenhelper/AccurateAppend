using System;
using System.Diagnostics;
using System.IO;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models
{
    /// <summary>
    /// Corresponds to an admin site bill file entry.
    /// </summary>
    [DebuggerDisplay("{" + nameof(CustomerFilename) + "}, " + nameof(Selected) + ":{" + nameof(Selected) + "}")]
    [Serializable()]
    public class File
    {
        #region Properties

        /// <summary>
        /// Indicates whether the current file is selected for upload and attachment or not.
        /// </summary>
        /// <value>True if the file should be attached; otherwise false.</value>
        public Boolean Selected { get; set; }

        /// <summary>
        /// Gets or sets the on system name of the file.
        /// </summary>
        public String Filename { get; set; }

        /// <summary>
        /// Gets or sets the human readable form of the file name.
        /// </summary>
        public String CustomerFilename { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Performs logic to determine what the correct filename that should be used when transmitting the file as an attachment.
        /// Note this is NOT the same as the <see cref="CustomerFilename"/> as that can be modified via processing.
        /// </summary>
        /// <returns>The file name that should be used with the file attachment.</returns>
        public virtual String DetermineFileName()
        {
            var fileNameExtension = Path.GetExtension(this.Filename);
            if (!String.IsNullOrWhiteSpace(fileNameExtension))
            {
                return Path.ChangeExtension(this.CustomerFilename, fileNameExtension);
            }

            return this.CustomerFilename;
        }

        #endregion
    }
}