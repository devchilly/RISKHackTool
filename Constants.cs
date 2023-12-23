using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISKHackTool
{

    public static class TerritoriesListOffsets
    {
        public const int SIZE_OFFSET = 0x18;
        public const int FIRST_TERRITORY_OFFSET = 0x20;
    }

    public static class TerritoryOffsets
    {
        public const int PLAYER_OFFSET = 0x80; // pointer to a player, so multiple territories will have same values
        public const int TERRITORY_INFO_OFFSET = 0x68;
        public const int REGION_OFFSET = 0x20; // need to check if this is similar to the player pointer
        public const int ENCRYPTED_UNITS_OFFSET = 0x88; // these are held as 8 byte encrypted values
        public const int TERRITORY_TYPE_OFFSET = 0xB4;
    }

    public static class TerritoryInfoOffsets
    {
        public const int NAME_OFFSET = 0x18;
    }

    public static class RegionOffsets
    {
        public const int REGION_INFO_OFFSET = 0x40;
    }

    public static class RegionInfoOffsets
    {
        public const int NAME_OFFSET = 0x18;
    }

    public static class PlayerOffsets
    {
        public const int COLOR_OFFSET = 0x40;
        public const int PLACEABLE_TROOPS_OFFSET = 0xD0;
    }

    public static class StringOffsets
    {
        public const int SIZE_OFFSET = 0x10;
        public const int FIRST_CHAR_OFFSET = 0x14;
    }

    public static class MemoryConstants
    {
        public const int INT_BYTES = 0x4;
        public const int POINTER_BYTES = 0x8;
    }

    public static class CSVConstants
    {
        public const int TERRITORY_NAME = 0;
        public const int COLOR = 1;
        public const int TERRITORY_TYPE = 2;
        public const int TROOP_COUNT = 3;
        public const int NUM_COLS = 4;
    }

    enum TerritoryType
    {
        Regular = 0,
        Capital = 1,
        Blizzard = 256
    }
}
