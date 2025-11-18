# 2.2.1

- Fix bug that would break automation when the selected fate changes.

# 2.2.0

- Added ability to blacklist fates by middle-clicking them.
- Added a help window under `/twist-of-fayte help|h`. Currently, this lists general tips and notes, but in the future should list all command information and config accessors.
- Added some missing translations

# 2.1.1

-   Updated icon

# 2.1.0

-   Disabled circle rendering around aetherytes
-   Renamed `reload-languages` to `reload-translations`
-   Added translation support to the automation state renderer
-   Corrected main window title
-   Added objective tracking for mobbing, collect and defend fates
    -   This should prevent the plugin from pulling much more mobs than it needs to. This is still subject to fate update delay.
-   Ensure aetheryte list is updated between zones. Preventing teleport bugs.
-   Added timeout to teleport state in case it is cancelled.

# 2.0.3

-   Fix bug in teleport handler causing it to init on the wrong state
-   Ensure a target is always selected in the fight gathered mobs handler

# 2.0.2

-   Prevent windows from auto-opening in release builds

# 2.0.1

-   Removed gate in ShouldDoFate

# 2.0.0

Complete rewrite to work with new version of Ocelot

-   Fixed bug that preventented max mob config from working as expected
-   Improved handling of roles outside of tanks.
    -   Tank/Healer/Dancer are grouped as their AOEs are all centered on self
    -   Casters are grouped as their AOEs are centered on a target
    -   Melees will be grouped in the future, for now they use the same as Tank/Healer/Dancer, but I want reliable support
        for Melees line/cone AOEs
-   Auto respawn, with optional wait for raise
-   MultiZone handling. Configure zones to hop between when no fates are available. These will cycle in a loop.
-   Auto repair and materia extraction
-   Teleport weighting. Configure teleport 'costs' to better score fates.
-   Relaxed movement to better facilitate VBM, this can still get whacky if there are ranged mobs
-   There are no configurations for selecting a rotation or mechanic plugin, they should be automatically determined based
    on what you have installed and enabled.
    -   Supported Rotation plugins: Wrath, VBM, RSR
    -   Supported Mechanic plugins: VBM, BMR
-   Auto start fate by talking to NPC when required
-   Auto sync level
-   Smart fate arrival.
    -   Arriving at a fate will put you in a different place depending on your job and the fates state
    -   If the requires talking to an NPC, it will start you near that NPC
    -   If you are melee, it will start you near the nearest enemy
    -   If you are ranged, it will start you in the outer 10% of the fate zone
