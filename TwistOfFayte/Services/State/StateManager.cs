using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Ocelot.Lifecycle;
using TwistOfFayte.Data.Fates;
using TwistOfFayte.Services.Fates;

namespace TwistOfFayte.Services.State;

public class StateManager(IFateRepository fates) : IStateManager, IOnUpdate
{
    private FateId? selectedFate;

    private bool isActive = true;

    public bool IsActive()
    {
        return isActive;
    }

    public void Toggle()
    {
        isActive = !isActive;
    }

    public void TurnOff()
    {
        isActive = false;
    }

    public void TurnOn()
    {
        isActive = true;
    }

    public unsafe FateId? GetCurrentFate()
    {
        var context = FateManager.Instance()->CurrentFate;
        if (context == null)
        {
            return null;
        }

        return new FateId(context->FateId);
    }

    public unsafe bool IsInFate()
    {
        return FateManager.Instance()->CurrentFate != null;
    }

    public FateId? GetSelectedFate()
    {
        return selectedFate;
    }

    public void SetSelectedFate(FateId id)
    {
        selectedFate = id;
    }

    public bool HasSelectedFate()
    {
        return selectedFate != null;
    }

    public void Update()
    {
        // Clear inactive fates
        if (selectedFate != null && !fates.HasFate(selectedFate.Value))
        {
            selectedFate = null;
        }
    }
}
