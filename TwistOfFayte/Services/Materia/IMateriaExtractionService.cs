using System.Threading.Tasks;
using Ocelot.Chain;

namespace TwistOfFayte.Services.Materia;

public interface IMateriaExtractionService
{
    bool ShouldExtract();

    IChain ExtractEquipped();
}
