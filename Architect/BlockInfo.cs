using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBT
{

    enum BlockType
    {
        SOLID = 1,
        NONSOLID = 2,
        FLUID = 3,
    }

    class Block
    {
        virtual public String name () { return ""; }
        virtual public BlockType type () { return BlockType.NONSOLID; }
        virtual public uint color () { return 0xFF80FF; }

        virtual public int blockId () { return 0; }
        virtual public int transp () { return 15; }
        virtual public int luminance () { return 0; }

        virtual public bool valid () { return false; }

        virtual public BlockModel model () { return BlockInfo.ModelCube; }
    }

    class Block0 : Block
    {
        override public String name () { return "Air"; }
        override public uint color () { return 0xFFFFFFFF; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block1 : Block
    {
        override public String name () { return "Stone"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x808080; }
        override public int blockId () { return 1; }
        override public bool valid () { return true; }
    }

    class Block2 : Block
    {
        override public String name () { return "Grass"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x6AAA40; }
        override public int blockId () { return 2; }
        override public bool valid () { return true; }
    }

    class Block3 : Block
    {
        override public String name () { return "Dirt"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x966C4A; }
        override public int blockId () { return 3; }
        override public bool valid () { return true; }
    }

    class Block4 : Block
    {
        override public String name () { return "Cobblestone"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x696969; }
        override public int blockId () { return 4; }
        override public bool valid () { return true; }
    }

    class Block5 : Block
    {
        override public String name () { return "Wooden Plank"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 5; }
        override public bool valid () { return true; }
    }

    class Block6 : Block
    {
        override public String name () { return "Sapling"; }
        override public uint color () { return 0x49CC25; }
        override public int blockId () { return 6; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block7 : Block
    {
        override public String name () { return "Bedrock"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x333333; }
        override public int blockId () { return 7; }
        override public bool valid () { return true; }
    }

    class Block8 : Block
    {
        override public String name () { return "Water"; }
        override public BlockType type () { return BlockType.FLUID; }
        override public uint color () { return 0x1F55FF; }
        override public int blockId () { return 8; }
        override public int transp () { return 2; }
        override public bool valid () { return true; }
    }

    class Block9 : Block
    {
        override public String name () { return "Stationary Water"; }
        override public BlockType type () { return BlockType.FLUID; }
        override public uint color () { return 0x1F55FF; }
        override public int blockId () { return 9; }
        override public int transp () { return 2; }
        override public bool valid () { return true; }
    }

    class Block10 : Block
    {
        override public String name () { return "Lava"; }
        override public BlockType type () { return BlockType.FLUID; }
        override public uint color () { return 0xFC5700; }
        override public int blockId () { return 10; }
        override public int luminance () { return 15; }
        override public bool valid () { return true; }
    }

    class Block11 : Block
    {
        override public String name () { return "Stationary Lava"; }
        override public BlockType type () { return BlockType.FLUID; }
        override public uint color () { return 0xFC5700; }
        override public int blockId () { return 11; }
        override public int luminance () { return 15; }
        override public bool valid () { return true; }
    }

    class Block12 : Block
    {
        override public String name () { return "Sand"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xE5DEA8; }
        override public int blockId () { return 12; }
        override public bool valid () { return true; }
    }

    class Block13 : Block
    {
        override public String name () { return "Gravel"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xB0A2A5; }
        override public int blockId () { return 13; }
        override public bool valid () { return true; }
    }

    class Block14 : Block
    {
        override public String name () { return "Gold Ore"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xFFEC4D; }
        override public int blockId () { return 14; }
        override public bool valid () { return true; }
    }

    class Block15 : Block
    {
        override public String name () { return "Iron Ore"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xD8AF93; }
        override public int blockId () { return 15; }
        override public bool valid () { return true; }
    }

    class Block16 : Block
    {
        override public String name () { return "Coal Ore"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x454545; }
        override public int blockId () { return 16; }
        override public bool valid () { return true; }
    }

    class Block17 : Block
    {
        override public String name () { return "Wood"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x675231; }
        override public int blockId () { return 17; }
        override public bool valid () { return true; }
    }

    class Block18 : Block
    {
        override public String name () { return "Leaves"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x006400; }
        override public int blockId () { return 18; }
        override public int transp () { return 1; }
        override public bool valid () { return true; }
    }

    class Block19 : Block
    {
        override public String name () { return "Sponge"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xCECE46; }
        override public int blockId () { return 19; }
        override public bool valid () { return true; }
    }

    class Block20 : Block
    {
        override public String name () { return "Glass"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xC0F5FE; }
        override public int blockId () { return 20; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block21 : Block
    {
        override public String name () { return "Lapis Lazuli Ore"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x1B4AA1; }
        override public int blockId () { return 21; }
        override public bool valid () { return true; }
    }

    class Block22 : Block
    {
        override public String name () { return "Lapis Lazuli Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x1B4AA1; }
        override public int blockId () { return 22; }
        override public bool valid () { return true; }
    }

    class Block23 : Block
    {
        override public String name () { return "Dispenser"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x4C4C4C; }
        override public int blockId () { return 23; }
        override public bool valid () { return true; }
    }

    class Block24 : Block
    {
        override public String name () { return "Sandstone"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xDCD5A8; }
        override public int blockId () { return 24; }
        override public bool valid () { return true; }
    }

    class Block25 : Block
    {
        override public String name () { return "Note Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9B664B; }
        override public int blockId () { return 25; }
        override public bool valid () { return true; }
    }

    class Block35 : Block
    {
        override public String name () { return "Wool"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xFEFEFE; }
        override public int blockId () { return 7; }
        override public bool valid () { return true; }
    }

    class Block37 : Block
    {
        override public String name () { return "Yellow Flower"; }
        override public uint color () { return 0xCCD302; }
        override public int blockId () { return 37; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block38 : Block
    {
        override public String name () { return "Red Rose"; }
        override public uint color () { return 0xBA050B; }
        override public int blockId () { return 38; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block39 : Block
    {
        override public String name () { return "Brown Mushroom"; }
        override public uint color () { return 0x916D55; }
        override public int blockId () { return 39; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block40 : Block
    {
        override public String name () { return "Red Mushroom"; }
        override public uint color () { return 0xE21212; }
        override public int blockId () { return 40; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block41 : Block
    {
        override public String name () { return "Gold Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xFFF144; }
        override public int blockId () { return 41; }
        override public bool valid () { return true; }
    }

    class Block42 : Block
    {
        override public String name () { return "Iron Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xE9E9E9; }
        override public int blockId () { return 42; }
        override public bool valid () { return true; }
    }

    class Block43 : Block
    {
        override public String name () { return "Double Stone Slab"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xA8A8A8; }
        override public int blockId () { return 43; }
        override public bool valid () { return true; }
    }

    class Block44 : Block
    {
        override public String name () { return "Stone Slab"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xA8A8A8; }
        override public int blockId () { return 44; }
        override public bool valid () { return true; }
    }

    class Block45 : Block
    {
        override public String name () { return "Brick Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9D4D37; }
        override public int blockId () { return 45; }
        override public bool valid () { return true; }
    }

    class Block46 : Block
    {
        override public String name () { return "TNT"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xC13C17; }
        override public int blockId () { return 46; }
        override public bool valid () { return true; }
    }

    class Block47 : Block
    {
        override public String name () { return "Bookshelf"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xB4905A; }
        override public int blockId () { return 47; }
        override public bool valid () { return true; }
    }

    class Block48 : Block
    {
        override public String name () { return "Moss Stone"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x567856; }
        override public int blockId () { return 48; }
        override public bool valid () { return true; }
    }

    class Block49 : Block
    {
        override public String name () { return "Obsidian"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x1E182B; }
        override public int blockId () { return 49; }
        override public bool valid () { return true; }
    }

    class Block50 : Block
    {
        override public String name () { return "Torch"; }
        override public uint color () { return 0xFFD800; }
        override public int blockId () { return 50; }
        override public int transp () { return 0; }
        override public int luminance () { return 14; }
        override public bool valid () { return true; }
    }

    class Block51 : Block
    {
        override public String name () { return "Fire"; }
        override public uint color () { return 0xFCA100; }
        override public int blockId () { return 51; }
        override public int transp () { return 0; }
        override public int luminance () { return 15; }
        override public bool valid () { return true; }
    }

    class Block52 : Block
    {
        override public String name () { return "Monster Spawner"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x1B547C; }
        override public int blockId () { return 52; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block53 : Block
    {
        override public String name () { return "Wooden Stairs"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 49; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block54 : Block
    {
        override public String name () { return "Chest"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xAB792D; }
        override public int blockId () { return 54; }
        override public bool valid () { return true; }
    }

    class Block55 : Block
    {
        override public String name () { return "Redstone Wire"; }
        override public uint color () { return 0xD60000; }
        override public int blockId () { return 55; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block56 : Block
    {
        override public String name () { return "Diamond Ore"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x5DECF5; }
        override public int blockId () { return 56; }
        override public bool valid () { return true; }
    }

    class Block57 : Block
    {
        override public String name () { return "Diamond Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x69DFDA; }
        override public int blockId () { return 57; }
        override public bool valid () { return true; }
    }

    class Block58 : Block
    {
        override public String name () { return "Workbench"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xA0693C; }
        override public int blockId () { return 58; }
        override public bool valid () { return true; }
    }

    class Block59 : Block
    {
        override public String name () { return "Crops"; }
        override public uint color () { return 0x87950C; }
        override public int blockId () { return 49; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block60 : Block
    {
        override public String name () { return "Farmland"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x633F24; }
        override public int blockId () { return 60; }
        override public bool valid () { return true; }
    }

    class Block61 : Block
    {
        override public String name () { return "Furnace"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x696969; }
        override public int blockId () { return 61; }
        override public bool valid () { return true; }
    }

    class Block62 : Block
    {
        override public String name () { return "Burning Furnace"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x696969; }
        override public int blockId () { return 62; }
        override public int luminance () { return 14; }
        override public bool valid () { return true; }
    }

    class Block63 : Block
    {
        override public String name () { return "Sign Post"; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 63; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block64 : Block
    {
        override public String name () { return "Wooden Door"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 64; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block65 : Block
    {
        override public String name () { return "Ladder"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 65; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block66 : Block
    {
        override public String name () { return "Minecart Tracks"; }
        override public uint color () { return 0xA4A4A4; }
        override public int blockId () { return 66; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block67 : Block
    {
        override public String name () { return "Cobblestone Stairs"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x696969; }
        override public int blockId () { return 67; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block68 : Block
    {
        override public String name () { return "Wall Sign"; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 66; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block69 : Block
    {
        override public String name () { return "Lever"; }
        override public uint color () { return 0x6E5938; }
        override public int blockId () { return 69; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block70 : Block
    {
        override public String name () { return "Stone Pressure Plate"; }
        override public uint color () { return 0x747474; }
        override public int blockId () { return 70; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block71 : Block
    {
        override public String name () { return "Iron Door"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xB8B8B8; }
        override public int blockId () { return 71; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block72 : Block
    {
        override public String name () { return "Wooden Pressure Plate"; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 72; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block73 : Block
    {
        override public String name () { return "Redstone Ore"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xAA0404; }
        override public int blockId () { return 73; }
        override public bool valid () { return true; }
    }

    class Block74 : Block
    {
        override public String name () { return "Glowing Redstone Ore"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xAA0404; }
        override public int blockId () { return 74; }
        override public int luminance () { return 9; }
        override public bool valid () { return true; }
    }

    class Block75 : Block
    {
        override public String name () { return "Redstone Torch Off"; }
        override public uint color () { return 0x560000; }
        override public int blockId () { return 75; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block76 : Block
    {
        override public String name () { return "Redstone Torch On"; }
        override public uint color () { return 0xFD0000; }
        override public int blockId () { return 76; }
        override public int transp () { return 0; }
        override public int luminance () { return 7; }
        override public bool valid () { return true; }
    }

    class Block77 : Block
    {
        override public String name () { return "Stone Button"; }
        override public uint color () { return 0x7F7F7F; }
        override public int blockId () { return 77; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block78 : Block
    {
        override public String name () { return "Snow"; }
        override public uint color () { return 0xEEFFFF; }
        override public int blockId () { return 78; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block79 : Block
    {
        override public String name () { return "Ice"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x5577A9FF; }
        override public int blockId () { return 79; }
        override public int transp () { return 3; }
        override public bool valid () { return true; }
    }

    class Block80 : Block
    {
        override public String name () { return "Snow Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xEEFFFF; }
        override public int blockId () { return 80; }
        override public bool valid () { return true; }
    }

    class Block81 : Block
    {
        override public String name () { return "Cactus"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x11831F; }
        override public int blockId () { return 81; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block82 : Block
    {
        override public String name () { return "Clay"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9BA1AC; }
        override public int blockId () { return 82; }
        override public bool valid () { return true; }
    }

    class Block83 : Block
    {
        override public String name () { return "Sugar Cane"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xAADB74; }
        override public int blockId () { return 83; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block84 : Block
    {
        override public String name () { return "Jukebox"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9B664B; }
        override public int blockId () { return 84; }
        override public bool valid () { return true; }
    }

    class Block85 : Block
    {
        override public String name () { return "Fence"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x9F844D; }
        override public int blockId () { return 85; }
        override public int transp () { return 0; }
        override public bool valid () { return true; }
    }

    class Block86 : Block
    {
        override public String name () { return "Pumpkin"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xE3901D; }
        override public int blockId () { return 86; }
        override public bool valid () { return true; }
    }

    class Block87 : Block
    {
        override public String name () { return "Netherrack"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x864E4E; }
        override public int blockId () { return 87; }
        override public bool valid () { return true; }
    }

    class Block88 : Block
    {
        override public String name () { return "Sould Sand"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0x826A5A; }
        override public int blockId () { return 88; }
        override public bool valid () { return true; }
    }

    class Block89 : Block
    {
        override public String name () { return "Glowstone"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xC08F46; }
        override public int blockId () { return 89; }
        override public int luminance () { return 15; }
        override public bool valid () { return true; }
    }

    class Block90 : Block
    {
        override public String name () { return "Portal"; }
        override public uint color () { return 0xD67FFF; }
        override public int blockId () { return 90; }
        override public int transp () { return 0; }
        override public int luminance () { return 11; }
        override public bool valid () { return true; }
    }

    class Block91 : Block
    {
        override public String name () { return "Jack-O-Lantern"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xE3901D; }
        override public int blockId () { return 91; }
        override public int luminance () { return 15; }
        override public bool valid () { return true; }
    }

    class Block92 : Block
    {
        override public String name () { return "Cake Block"; }
        override public BlockType type () { return BlockType.SOLID; }
        override public uint color () { return 0xEAE9EB; }
        override public int blockId () { return 92; }
        override public bool valid () { return true; }
    }

    class BlockInfo
    {
        public static Block Air = new Block0();
        public static Block Stone = new Block1();
        public static Block Grass = new Block2();
        public static Block Dirt = new Block3();
        public static Block Cobblestone = new Block4();
        public static Block WoodPlank = new Block5();
        public static Block Sapling = new Block6();
        public static Block Bedrock = new Block7();
        public static Block Water = new Block8();
        public static Block StaWater = new Block9();
        public static Block Lava = new Block10();
        public static Block StaLava = new Block11();
        public static Block Sand = new Block12();
        public static Block Gravel = new Block13();
        public static Block GoldOre = new Block14();
        public static Block IronOre = new Block15();
        public static Block CoalOre = new Block16();
        public static Block Wood = new Block17();
        public static Block Leaves = new Block18();
        public static Block Sponge = new Block19();
        public static Block Glass = new Block20();
        public static Block LapisOre = new Block21();
        public static Block LapisBlock = new Block22();
        public static Block Dispenser = new Block23();
        public static Block Sandstone = new Block24();
        public static Block NoteBlock = new Block25();
        public static Block Wool = new Block35();
        public static Block YellowFlower = new Block37();
        public static Block RedRose = new Block38();
        public static Block BrownMushroom = new Block39();
        public static Block RedMushroom = new Block40();
        public static Block GoldBlock = new Block41();
        public static Block IronBlock = new Block42();
        public static Block DStoneSlab = new Block43();
        public static Block StoneSlab = new Block44();
        public static Block BrickBlock = new Block45();
        public static Block TNT = new Block46();
        public static Block Bookshelf = new Block47();
        public static Block MossStone = new Block48();
        public static Block Obsidian = new Block49();
        public static Block Torch = new Block50();
        public static Block Fire = new Block51();
        public static Block MonsterSpawner = new Block52();
        public static Block WoodStairs = new Block53();
        public static Block Chest = new Block54();
        public static Block RedstoneWire = new Block55();
        public static Block DiamondOre = new Block56();
        public static Block DiamondBlock = new Block57();
        public static Block Workbench = new Block58();
        public static Block Crops = new Block59();
        public static Block Farmland = new Block60();
        public static Block Furnace = new Block61();
        public static Block LitFurnace = new Block62();
        public static Block SignPost = new Block63();
        public static Block WoodDoor = new Block64();
        public static Block Ladder = new Block65();
        public static Block MinecartTracks = new Block66();
        public static Block StoneStairs = new Block67();
        public static Block WallSign = new Block68();
        public static Block Lever = new Block69();
        public static Block StonePlate = new Block70();
        public static Block IronDoor = new Block71();
        public static Block WoodPlate = new Block72();
        public static Block RedstoneOre = new Block73();
        public static Block GlowRedstoneOre = new Block74();
        public static Block RedstoneTorch = new Block75();
        public static Block RedstoneTorchOn = new Block76();
        public static Block StoneButton = new Block77();
        public static Block Snow = new Block78();
        public static Block Ice = new Block79();
        public static Block SnowBlock = new Block80();
        public static Block Cactus = new Block81();
        public static Block Clay = new Block82();
        public static Block SugarCane = new Block83();
        public static Block Jukebox = new Block84();
        public static Block Fence = new Block85();
        public static Block Pumpkin = new Block86();
        public static Block Netherrack = new Block87();
        public static Block SoulSand = new Block88();
        public static Block Glowstone = new Block89();
        public static Block Portal = new Block90();
        public static Block JackOLantern = new Block91();
        public static Block CakeBlock = new Block92();

        public static Block[] Index = new Block[100] {
            Air,
            Stone,
            Grass,
            Dirt,
            Cobblestone,
            WoodPlank,
            Sapling,
            Bedrock,
            Water,
            StaWater,
            Lava,
            StaLava,
            Sand,
            Gravel,
            GoldOre,
            IronOre,
            CoalOre,
            Wood,
            Leaves,
            Sponge,
            Glass,
            LapisOre,
            LapisBlock,
            Dispenser,
            Sandstone,
            NoteBlock,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            Wool,
            null,
            YellowFlower,
            RedRose,
            BrownMushroom,
            RedMushroom,
            GoldBlock,
            IronBlock,
            DStoneSlab,
            StoneSlab,
            BrickBlock,
            TNT,
            Bookshelf,
            MossStone,
            Obsidian,
            Torch,
            Fire,
            MonsterSpawner,
            WoodStairs,
            Chest,
            RedstoneWire,
            DiamondOre,
            DiamondBlock,
            Workbench,
            Crops,
            Farmland,
            Furnace,
            LitFurnace,
            SignPost,
            WoodDoor,
            Ladder,
            MinecartTracks,
            StoneStairs,
            WallSign,
            Lever,
            StonePlate,
            IronDoor,
            WoodPlate,
            RedstoneOre,
            GlowRedstoneOre,
            RedstoneTorch,
            RedstoneTorchOn,
            StoneButton,
            Snow,
            Ice,
            SnowBlock,
            Cactus,
            Clay,
            SugarCane,
            Jukebox,
            Fence,
            Pumpkin,
            Netherrack,
            SoulSand,
            Glowstone,
            Portal,
            JackOLantern,
            CakeBlock,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
        };

        public static BlockModel ModelCube = new BlockModelCube();
    }
}
