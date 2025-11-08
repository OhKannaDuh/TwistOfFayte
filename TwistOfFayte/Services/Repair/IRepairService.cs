using System.Threading.Tasks;
using Ocelot.Chain;

namespace TwistOfFayte.Services.Repair;

public interface IRepairService
{
    bool ShouldRepair();

    IChain Repair();
}
