using Dalamud.Bindings.ImGui;
using Ocelot.Services.Translation;
using Ocelot.Services.Translation.Extensions;
using Ocelot.Services.UI;
using Ocelot.States;
using TwistOfFayte.Modules.Automator;
using TwistOfFayte.Services.State;

namespace TwistOfFayte.Renderers;

public class AutomationStateRenderer(
    IStateMachine<AutomatorState> stateMachine,
    IStateManager state,
    ITranslator<AutomationStateRenderer> translator,
    IUIService ui
)
{
    public void Render()
    {
        ui.LabelledValue(translator.T(".messages.is_active"), state.IsActive() ? translator.Yes() : translator.No());
        if (state.IsActive())
        {
            stateMachine.Render();
        }

        if (ImGui.Button(translator.T(".controls.toggle")))
        {
            state.Toggle();
        }
    }
}
