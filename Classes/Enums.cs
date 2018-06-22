namespace ACSE
{
    // TODO: Move all enums to this file
    public enum Animal_Crossing_Grass_Type : byte
    {
        Triangle = 0,
        Square = 1,
        Circle = 2
    }

    public enum Wild_World_Grass_Type : byte // NOTE: Wild World saves the grass's seasonal modifier. Going to have to document all of them.
    {
        Triangle = 0,
        Circle = 1,
        Square = 2
    }

    public enum City_Folk_Grass_Type : byte
    {
        Triangle = 0,
        Circle = 1,
        Square = 2
    }

    public enum New_Leaf_Grass_Type : byte
    {
        Triangle = 0,
        Circle = 1,
        Square = 2
    }

    // AFe+ Password Enums

    public enum CodeType : int
    {
        Famicom = 0, // NES
        NPC = 1, // Original NPC Code
        Card_E = 2, // NOTE: This is a stubbed method (just returns 4)
        Magazine = 3, // Contest?
        User = 4, // Player-to-Player
        Card_E_Mini = 5, // Only one data strip?
        New_NPC = 6, // Using the new password system?
        Monument = 7 // Town Decorations (from Object Delivery Service, see: https://www.nintendo.co.jp/ngc/gaej/obje/)
    }

    public enum MonumentType : int
    {
        ParkClock = 0,
        GasLamp = 1,
        Windpump = 2,
        FlowerClock = 3,
        Heliport = 4,
        WindTurbine = 5,
        PipeStack = 6,
        Stonehenge = 7,
        Egg = 8,
        Footprints = 9,
        Geoglyph = 10,
        Mushroom = 11,
        Signpost = 12,
        Well = 13,
        Fountain = 14
    }
}