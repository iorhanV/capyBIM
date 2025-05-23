using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using capyBIM.CmdsResources;
using capyBIM.CmdsTools;
using Nice3point.Revit.Toolkit.External;
using capyBIM.Commands;
using capyBIM.Extensions;
using zRib = capyBIM.Utilities.RibbonUtils;

// using capyBIM.CmdsTools;
// using capyBIM.Extensions;


namespace capyBIM
{
    /// <summary>
    ///     Application entry point
    /// </summary>
// [UsedImplicitly]
    public class Application : IExternalApplication
    {
        #region Properties

        // Make a private uiCtlApp
        private static UIControlledApplication _uiCtlApp;

        #endregion

        public Result OnStartup(UIControlledApplication uiCtlApp)
        {
            #region Globals registration
            
            // Store _uiCtlApp, register on idling
            _uiCtlApp = uiCtlApp;

            try
            {
                _uiCtlApp.Idling += RegisterUiApp;
            }
            catch
            {
                Globals.UiApp = null;
                Globals.UsernameRevit = null;
            }

            // Registering globals
            Globals.RegisterProperties(uiCtlApp);
            Globals.RegisterTooltips($"{Globals.AddinName}.Resources.Files.Tooltips");

            #endregion

            CreateRibbon(uiCtlApp);
            
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            throw new NotImplementedException();
        }

        private void CreateRibbon(UIControlledApplication uiCtlApp)
        {
            uiCtlApp.Ext_AddRibbonTab(Globals.AddinName);
            
            // Create Resources panel
            var panelResources = uiCtlApp.Ext_AddRibbonPanel(Globals.AddinName, "Resources");
            
            // Create Resources stack
            var stackResources1 = zRib.NewPushButtonData<CmdGitHub>("Github");
            var stackResources2 = zRib.NewPushButtonData<CmdIn>("LinkedIn");
            var stackResources3 = zRib.NewPushButtonData<CmdRVTDocs>("RevitApi");
            // var stackResources = panelResources.AddStackedItems(
            panelResources.AddStackedItems( stackResources1, stackResources2, stackResources3);
            
            // Create Tools panel
            var panelTools = uiCtlApp.Ext_AddRibbonPanel(Globals.AddinName, "Tools");
            
            Debug.WriteLine("Panel Tools");

            var pushRotate = panelTools.Ext_AddPushButton<CmdRotate>("Rotate");
            
            // var panel = Application.CreatePanel("Commands", "capyBIM");
            //
            // panel.AddPushButton<StartupCommand>("Execute")
            //     .SetImage("/capyBIM;component/Resources/Icons/RibbonIcon16.png")
            //     .SetLargeImage("/capyBIM;component/Resources/Icons/RibbonIcon32.png");
        }
        
        #region Use idling to register UiApp

        /// <summary>
        /// Registers the UiApp and Revit username globally.
        /// </summary>
        /// <param name="sender">Sender of the Idling event.</param>
        /// <param name="e">Idling event arguments.</param>
        private static void RegisterUiApp(object sender, IdlingEventArgs e)
        {
            _uiCtlApp.Idling -= RegisterUiApp;

            if (sender is UIApplication uiApp)
            {
                Globals.UiApp = uiApp;
                Globals.UsernameRevit = uiApp.Application.Username;
            }
        }

        #endregion
    }
}