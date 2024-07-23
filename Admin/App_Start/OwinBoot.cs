using System;
using System.Configuration;
using AccurateAppend.Websites.Admin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OwinConfig))]

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Configures the OWIN interfaces for startup
    /// </summary>
    /// <remarks>
    /// OWIN is required for SignalR so enabled for this application.
    /// </remarks>
    public class OwinConfig
    {
        /// <summary>
        /// Convention based method used for bootstrapping.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> component supplied by the OWIN infrastructure.</param>
        public void Configuration(IAppBuilder app)
        {
            const String Key = "SignalR";

            var connection = ConfigurationManager.ConnectionStrings[Key];
            if (String.IsNullOrWhiteSpace(connection?.ConnectionString)) throw new ConfigurationErrorsException($"No configuration connection string for '{Key}' found. Did you forget to add this connection string?");

            GlobalHost.DependencyResolver.UseSqlServer(connection.ConnectionString);
            app.MapSignalR();
        }
    }
}