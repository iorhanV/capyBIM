// System
using Assembly = System.Reflection.Assembly;
// Autodesk
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System.Resources;
using System.Globalization;
using System.Collections;

namespace capyBIM
{
    /// <summary>
    /// Variables that persist beyond the running of commands.
    /// Many of them are set once at app startup.
    /// </summary>
    public static class Globals
    {
        #region Global properties
        // Applications
        public static UIControlledApplication UiCtlApp { get; set; }
        public static ControlledApplication CtlApp { get; set; }
        public static UIApplication UiApp { get; set; }

        // Assembly
        public static Assembly Assembly { get; set; }
        public static string AssemblyPath { get; set; }

        // Revit versions
        public static string RevitVersion { get; set; }
        public static int RevitVersionInt { get; set; }

        // User names
        public static string UsernameRevit { get; set; }
        public static string UsernameWindows { get; set; }

        // Guids and versioning
        public static string AddinVersionNumber { get; set; }
        public static string AddinVersionName { get; set; }
        public static string AddinName { get; set; }
        public static string AddinGuid { get; set; }

        // Dictionaries for resources
        public static Dictionary<string, string> Tooltips { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Register method

        /// <summary>
        /// Register global properties on startup.
        /// </summary>
        /// <param name="uiCtlApp">The UIControlledApplication</param>
        public static void RegisterProperties(UIControlledApplication uiCtlApp)
        {
            UiCtlApp = uiCtlApp;
            CtlApp = uiCtlApp.ControlledApplication;
            // UiApp set on idling

            Assembly = Assembly.GetExecutingAssembly();
            AssemblyPath = Assembly.Location;

            RevitVersion = CtlApp.VersionNumber;
            RevitVersionInt = Int32.Parse(RevitVersion);

            // Revit username set on idling
            UsernameWindows = Environment.UserName;

            // Store versions and Ids
            AddinVersionNumber = "0.0";
            AddinVersionName = "wip";
            AddinGuid = "A60D5549-2E8F-444D-B2F9-1C0A8096297D";
            AddinName = "capyBIM";
        }

        #endregion

        #region Register tooltips

        /// <summary>
        /// Sets the tooltip values.
        /// </summary>
        /// <param name="resourcePath"">The full path to the tooltip resource.</param>
        /// <returns>Void (nothing).</returns>
        public static void RegisterTooltips(string resourcePath)
        {
            // Construct the assembly, resource and sub-assembly paths
            var resourceManager = new ResourceManager(resourcePath, Globals.Assembly);
            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            // Get all tooltip entries, store globally
            foreach (DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key.ToString();
                Tooltips[key] = entry.Value.ToString();
            }
        }

        #endregion
    }
}