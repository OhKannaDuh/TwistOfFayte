using TwistOfFayte.Data.Fates;

namespace TwistOfFayte.Services.State;

public interface IStateManager
{
    bool IsActive();

    void Toggle();
    
    void TurnOff();

    void TurnOn();
    
    FateId? GetCurrentFate();

    bool IsInFate();

    FateId? GetSelectedFate();

    void SetSelectedFate(FateId id);

    bool HasSelectedFate();
}
