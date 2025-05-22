using Nice3point.Revit.Toolkit.External;
using capyBIM.Commands;

namespace capyBIM;

/// <summary>
///     Application entry point
/// </summary>
[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "capyBIM");

        panel.AddPushButton<StartupCommand>("Execute")
            .SetImage("/capyBIM;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/capyBIM;component/Resources/Icons/RibbonIcon32.png");
    }
}