namespace Assets.Scripts.Lib.Model.Enum
{
    public enum RrfAttribute
    {
        House,
        Tree,
        Wall,
        Tank,
        Turret,
        Kannone,
        Muzzle,
        Wheel,
        Follower,
        MG1,    // Bow
        MG2,    // Coax
        MG3,    // Anti-air
        MG4,    // Unused
        Hatch,  // Object and their child objects are displayed if the unit's hatch is open.  If a hatch object is present, the players external view camera does not have an offset.
        Smoke1,
        Smoke2,
        Smoke3,
        Smoke4,
        Smoke5,
        Smoke6,
        SmokeM,
        DustH,
        DustV,
        VisionArea,
        Commander,
        Binocular,
        ScopeScale,
        DispNo,
        DispWater,
        DispOil,
        DispOilPressure,
        DispGear,
        DispSpeed,
        DispRpm,
        DispTurret,
        Disp9,
        Disp10,
        Disp11,
        Disp12,
        Disp13,
        Disp14,
        Disp15,
        BlackObj,
        TrackType1,
        TrackType2,
        TrackType3,
        TrackType4,
        TrackType5,
        TrackType6,
        TrackType7,
        TrackType8 = 50,
        M10Zoom = 62,
        M101X = 63,
        Exhaust = 70,
        Disp62 = 88,
        Disp63 = 89,
        Disp64 = 90,
        MantleXA = 91,
        Schuerzen = 92,
        HSchuerzen = 93,
        AAMG = 94,  // Unused
        Hedgehod = 95,
        Radio = 96,
        WetStowage = 97,
        APlatesTurret = 98,
        APlatesHull = 99,
        Umbrella = 100,
        ComSprite = 101,
        TrackL = 102,
        TrackR = 103,
        Misc1 = 104,
        Misc2 = 105,
        Barrel = 106, // object and their child objects will recoil when the main gun is fired.
        IdlerWheel = 110,
        CrewDriver = 114,
        CrewRadioOp = 115,
        CrewGunner = 116,
        CrewLoader = 117,
        CrewCommander = 118,
        VisualJunk = 120,   // ignored for damage and collision, but is used for AI spotting.
        Camo = 121, // ignored for damage and collision, but is used for AI spotting.
        Hatch2 = 122,   // will display when object #13 (hatch) is hidden
        NullVisable = 254,  // ignored for damage and collision, but is used for AI spotting.
        Null = 255 // ignored for damage and collision, but is used for AI spotting.
    }
}
