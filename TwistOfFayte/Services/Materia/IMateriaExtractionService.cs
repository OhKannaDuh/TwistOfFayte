using Ocelot.Chain;

namespace TwistOfFayte.Services.Materia;

public interface IMateriaExtractionService
{
    bool ShouldExtract();

    IChain ExtractEquipped();
}
