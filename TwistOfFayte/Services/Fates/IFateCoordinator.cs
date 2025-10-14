using System.Threading;
using System.Threading.Tasks;

namespace TwistOfFayte.Services.Fates;

public interface IFateCoordinator
{
    Task RunAsync(CancellationToken token);

    void RequestStop();
}
