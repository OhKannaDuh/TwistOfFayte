using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
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
    IFramework framework,
    ILogger<Zone> logger
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

        logger.Debug("Zone has changed to {t}", territory);

        UpdateTerritoryDataAsync(territory);
    }

    public void OnStart()
    {
        framework.RunOnTick(() => UpdateTerritoryDataAsync(client.CurrentTerritoryId));
    }

    private async Task UpdateTerritoryDataAsync(ushort territory, CancellationToken token = default)
    {
        Id = territory;
        Aetherytes.Clear();
        if (Id == 0)
        {
            return;
        }

        var positions = new Dictionary<uint, Vector3>();

        const int maxAttempts = 5;
        var delay = TimeSpan.FromMilliseconds(250);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            token.ThrowIfCancellationRequested();

            var success = await Task
                .Run(() =>
                {
                    unsafe
                    {
                        try
                        {
                            var layout = LayoutWorld.Instance()->ActiveLayout;
                            if (layout == null || !layout->InstancesByType.TryGetValue(InstanceType.Aetheryte, out var mapPtr, false))
                            {
                                return false;
                            }


                            foreach (ILayoutInstance* instance in mapPtr.Value->Values)
                            {
                                var transform = instance->GetTransformImpl();
                                var position = transform->Translation;

                                positions[instance->Id.InstanceKey] = position;
                            }


                            Aetherytes.Clear();
                            var aetherytes = aetheryteRepository.Where(a => a.Territory.RowId == territory && a.IsAetheryte).ToList();
                            logger.Debug("Found {c} aetherytes in this zone", aetherytes.Count);

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

                            return true;
                        }
                        catch (Exception ex)
                        {
                            logger.Warn(ex, "Attempt {Attempt} failed in UpdateTerritoryDataAsync unsafe section.", attempt);
                            return false;
                        }
                    }
                }, token)
                .ConfigureAwait(false);

            if (success)
            {
                break;
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                logger.Error("Unable to get active layout after {Attempts} attempts.", maxAttempts);
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
