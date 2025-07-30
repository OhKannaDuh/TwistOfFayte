using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using TwistOfFayte.Modules;

namespace TwistOfFayte.Windows;

[OcelotConfigWindow]
public class ConfigWindow(Plugin _plugin, Config _config) : OcelotConfigWindow(_plugin, _config)
{
    private IModule? selectedConfigModule;

    public override void PostInitialize()
    {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(400, 0),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    protected override void Render(RenderContext context)
    {
        var modules = plugin.Modules.GetModulesByConfigOrder().ToList();
        selectedConfigModule ??= modules.FirstOrDefault();

        using (ImRaii.Child("##LeftPanel", new Vector2(300, 0), true))
        {
            foreach (var module in modules)
            {
                if (module is not Module concreteModule || concreteModule.Config == null)
                {
                    continue;
                }

                var name = concreteModule.Config.GetType().Name;

                var title = concreteModule.Config.GetTitle();
                if (title != null)
                {
                    name = title;
                }

                var selected = module == selectedConfigModule;
                if (ImGui.Selectable(name, selected))
                {
                    selectedConfigModule = module;
                }
            }
        }

        ImGui.SameLine();


        using (ImRaii.Child("##RightPanel", new Vector2(0, 0), true))
        {
            selectedConfigModule!.RenderConfigUi(context);
        }
    }
}
