using Ocelot.Services.Commands;

namespace TwistOfFayte.Commands;

public class HelpCommandDelegate(HelpCommand command) : IMainCommandDelegate
{
    public IOcelotCommand Command { get; } = command;
}
