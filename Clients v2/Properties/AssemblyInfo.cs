using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Clients Application")]
[assembly: AssemblyDescription("Accurate Append Public Clients Application")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Accurate Append Corp")]
[assembly: AssemblyProduct("Clients")]
[assembly: AssemblyCopyright("Copyright © Accurate Append Corp 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("EN-us")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("88181548-14d5-4052-906c-2fedbebbcff0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("2.0.0.0")]


// Provides R# MVC intellisense hints
[assembly: AspMvcAreaViewLocationFormat(@"~\Areas\{2}\{1}\Views\{0}.cshtml")]
[assembly: AspMvcViewLocationFormatAttribute(@"~\Areas\{2}\{1}\Views\{0}.cshtml")]