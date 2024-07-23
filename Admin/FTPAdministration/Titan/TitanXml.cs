using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AccurateAppend.Security.FTPAdministration
{
    /// <summary>
    /// Provides xml compatibility with the Titan COM api.
    /// </summary>
    public static class TitanXml
    {
        /// <summary>
        /// Parses the provided xml to provide access to the Titan FTP session list information.
        /// </summary>
        /// <remarks>
        /// XML Format:
        /// <![CDATA[
        /// <SessionList>
        ///  <Session>
        ///   <SessionId>XXX</SessionId>
        ///   <Username>users name</Username>
        ///   <ClientIP>xx.yy.zz.aa</ClientIP>
        ///   <Protocol>FTP or FTPS or SFTP</Protocol>
        ///  </Session>
        /// </SessionList>      
        /// ]]>
        /// Tuple:
        /// <list type="table">
        /// <listheader>
        /// <term>Property</term>
        /// <description>Element Value</description>
        /// </listheader>
        /// <item>
        /// <term>Item1</term>
        /// <description>SessionId</description>
        /// </item>
        /// <item>
        /// <term>Item2</term>
        /// <description>Username</description>
        /// </item>
        /// <item>
        /// <term>Item3</term>
        /// <description>ClientIP</description>
        /// </item>
        /// <item>
        /// <term>Item4</term>
        /// <description>Protocol</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="xml">The <see cref="XElement"/> containing the session list information.</param>
        /// <returns>A sequence of <see cref="Tuple{String, String, String}"/> types.</returns>
        public static IEnumerable<Tuple<String, String, String, String>> Sessions(this XElement xml)
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));
            if (xml.Name.LocalName != "SessionList") throw new ArgumentOutOfRangeException(nameof(xml), xml.Name, $"{nameof(xml)} must be a 'SessionList' element");

            return xml.Descendants("Session").Select(n =>
            {
                var sessionId = n.Descendants("SessionId").First().Value;
                var userName = n.Descendants("Username").First().Value;
                var clientIp = n.Descendants("ClientIP").First().Value;
                var protocol = n.Descendants("Protocol").First().Value;

                return Tuple.Create(sessionId, userName, clientIp, protocol);
            });
        }
    }
}