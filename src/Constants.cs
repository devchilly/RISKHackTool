using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RISKHackTool.src
{
    public static class BaseOffsets
    {
        public const int GAME_MANAGER_OFFSET = 0x2715CF8;
    }

    public static class Constants
    {
        public const int MAX_BUFFER_SIZE = 100;
        public const int MAX_SCROLL_SIZE = 5000;
    }

    public static class GameManagerOffsets
    {
        public const int CONFIG_OFFSET = 0xB8;
    }

    public static class GameManagerConfigOffsets
    {
        public const int FOG_ENABLED_OFFSET = 0x16;
    }

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
        public const int CARD_LIST_OFFSET = 0xF8;
    }

    public static class CardListOffsets
    {
        public const int PHYSICAL_CARD_LIST_OFFSET = 0x10;
        public const int NUMBER_OF_CARDS_OFFSET = 0x18;
    }

    public static class PhysicalCardListOffsets
    {
        public const int FIRST_CARD_OFFSET = 0x20;
    }

    public static class CardOffsets
    {
        public const int CARD_TYPE_OFFSET = 0x10;
        public const int CARD_TERRITORY_OFFSET = 0x18;
    }

    public static class StringOffsets
    {
        public const int SIZE_OFFSET = 0x10;
        public const int FIRST_CHAR_OFFSET = 0x14;
    }

    public static class MemoryConstants
    {
        public const int BOOLEAN_BYTES = 0x1;
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

    enum CardType
    {
        Any = 0,
        Infantry = 1,
        Calvary = 2,
        Artillery = 3
    }

    enum FogConfig
    {
        Disabled = 0
    }
}
