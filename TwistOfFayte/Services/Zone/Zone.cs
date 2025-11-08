using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using Ocelot.Extensions;
using Ocelot.Lifecycle;
using Ocelot.Services.ClientState;
using Ocelot.Services.Data;
using Ocelot.Services.Logger;
using Aetheryte = TwistOfFayte.Data.Zone.Aetheryte;

namespace TwistOfFayte.Services.Zone;

public class Zone(
    IDataRepository<AetheryteData> aetheryteRepository,
    IDataRepository<Map> mapRepository,
    IDataManager data,
    IClient client,
    IFramework framework
) : IZone, IOnTerritoryChanged, IOnStart
{
    public ushort Id { get; private set; } = 0;

    public List<Aetheryte> Aetherytes { get; private set; } = [];


    public void IOnTerritoryChanged(ushort territory)
    {
        if (territory == Id)
        {
            return;
        }

        UpdateTerritoryData(territory);
    }

    public void OnStart()
    {
        framework.RunOnTick(() => UpdateTerritoryData(client.CurrentTerritoryId));
    }

    private void UpdateTerritoryData(ushort territory)
    {
        Id = territory;

        Dictionary<uint, Vector3> positions = [];

        unsafe
        {
            var layout = LayoutWorld.Instance()->ActiveLayout;
            if (layout == null || !layout->InstancesByType.TryGetValue(InstanceType.Aetheryte, out var mapPtr, false))
            {
                return;
            }

            foreach (ILayoutInstance* instance in mapPtr.Value->Values)
            {
                var transform = instance->GetTransformImpl();
                var position = transform->Translation;

                positions[instance->Id.InstanceKey] = position;
            }


            Aetherytes.Clear();
            var aetherytes = aetheryteRepository.Where(a => a.Territory.RowId == territory && a.IsAetheryte);

            foreach (var aetheryte in aetherytes)
            {
                var level = aetheryte.Level[0].ValueNullable;
                if (level != null)
                {
                    Aetherytes.Add(new Aetheryte(
                        aetheryte,
                        new Vector3(level.Value.X, level.Value.Y, level.Value.Z)
                    ));

                    continue;
                }

                var sheet = data.GetSubrowExcelSheet<MapMarker>();
                var marker = sheet.Flatten().FirstOrNull(m => m.DataType == 3 && m.DataKey.RowId == aetheryte.RowId)
                             ?? sheet.Flatten().First(m => m.DataType == 4 && m.DataKey.RowId == aetheryte.AethernetName.RowId);

                var position = PixelCoordsToWorldCoords(marker.X, marker.Y, aetheryte.Territory.Value.Map.RowId);

                Aetherytes.Add(new Aetheryte(
                    aetheryte,
                    positions.OrderBy(p => p.Value.Distance2D(position)).First().Value
                ));
            }
        }
    }

    private Vector3 PixelCoordsToWorldCoords(int x, int z, uint mapId)
    {
        var scale = 1f;
        short offsetX = 0;
        short offsetZ = 0;

        if (mapRepository.ContainsKey(mapId))
        {
            var map = mapRepository.Get(mapId);
            scale = map.SizeFactor * 0.01f;
            offsetX = map.OffsetX;
            offsetZ = map.OffsetY; // Y because maps are 2d but FFXIV 3d coords has y as up (So y should be z)
        }

        var wx = PixelCoordToWorldCoord(x, scale, offsetX);
        var wz = PixelCoordToWorldCoord(z, scale, offsetZ);

        return new Vector3(wx, 0, wz);
    }

    private float PixelCoordToWorldCoord(float coord, float scale, short offset)
    {
        const float factor = 2048.0f / (50 * 41);
        return (coord * factor - 1024f) / scale - offset * 0.001f;
    }
}
