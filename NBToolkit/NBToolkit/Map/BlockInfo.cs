using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;

    public enum BlockType
    {
        AIR = 0,
        STONE = 1,
        GRASS = 2,
        DIRT = 3,
        COBBLESTONE = 4,
        WOOD_PLANK = 5,
        SAPLING = 6,
        BEDROCK = 7,
        WATER = 8,
        STATIONARY_WATER = 9,
        LAVA = 10,
        STATIONARY_LAVA = 11,
        SAND = 12,
        GRAVEL = 13,
        GOLD_ORE = 14,
        IRON_ORE = 15,
        COAL_ORE = 16,
        WOOD = 17,
        LEAVES = 18,
        SPONGE = 19,
        GLASS = 20,
        LAPIS_ORE = 21,
        LAPIS_BLOCK = 22,
        DISPENSER = 23,
        SANDSTONE = 24,
        NOTE_BLOCK = 25,
        BED = 26,
        WOOL = 35,
        YELLOW_FLOWER = 37,
        RED_ROSE = 38,
        BROWN_MUSHROOM = 39,
        RED_MUSHROOM = 40,
        GOLD_BLOCK = 41,
        IRON_BLOCK = 42,
        DOUBLE_SLAB = 43,
        SLAB = 44,
        BRICK_BLOCK = 45,
        TNT = 46,
        BOOKSHELF = 47,
        MOSS_STONE = 48,
        OBSIDIAN = 49,
        TORCH = 50,
        FIRE = 51,
        MONSTER_SPAWNER = 52,
        WOOD_STAIRS = 53,
        CHEST = 54,
        REDSTONE_WIRE = 55,
        DIAMOND_ORE = 56,
        DIAMOND_BLOCK = 57,
        CRAFTING_TABLE = 58,
        CROPS = 59,
        FARMLAND = 60,
        FURNACE = 61,
        BURNING_FURNACE = 62,
        SIGN_POST = 63,
        WOOD_DOOR = 64,
        LADDER = 65,
        RAILS = 66,
        COBBLESTONE_STAIRS = 67,
        WALL_SIGN = 68,
        LEVER = 69,
        STONE_PLATE = 70,
        IRON_DOOR = 71,
        WOOD_PLATE = 72,
        REDSTONE_ORE = 73,
        GLOWING_REDSTONE_ORE = 74,
        REDSTONE_TORCH_OFF = 75,
        REDSTONE_TORCH_ON = 76,
        STONE_BUTTON = 77,
        SNOW = 78,
        ICE = 79,
        SNOW_BLOCK = 80,
        CACTUS = 81,
        CLAY_BLOCK = 82,
        SUGAR_CANE = 83,
        JUKEBOX = 84,
        FENCE = 85,
        PUMPKIN = 86,
        NETHERRACK = 87,
        SOUL_SAND = 88,
        GLOWSTONE_BLOCK = 89,
        PORTAL = 90,
        JACK_O_LANTERN = 91,
        CAKE_BLOCK = 92,
        REDSTONE_REPEATER_ON = 93,
        REDSTONE_REPEATER_OFF = 94,
        LOCKED_CHEST = 95,
    }

    public class BlockInfo
    {
        public const int MAX_BLOCKS = 256;

        public const int MAX_OPACITY = 15;
        public const int MIN_OPACITY = 0;
        public const int MAX_LUMINANCE = 15;
        public const int MIN_LUMINANCE = 0;

        private static BlockInfo[] _blockTable;
        private static int[] _opacityTable;
        private static int[] _luminanceTable;

        protected internal static NBTCompoundNode[] _schemaTable;

        public class ItemCache<T>
        {
            private T[] _cache;

            public T this[int index]
            {
                get { return _cache[index]; }
            }

            public ItemCache (T[] cache)
            {
                _cache = cache;
            }
        }

        private class DataLimits
        {
            private int _low;
            private int _high;
            private int _bitmask;

            public int Low
            {
                get { return _low; }
            }

            public int High
            {
                get { return _high; }
            }

            public int Bitmask
            {
                get { return _bitmask; }
            }

            public DataLimits (int low, int high, int bitmask)
            {
                _low = low;
                _high = high;
                _bitmask = bitmask;
            }

            public bool Test (int data)
            {
                int rdata = data & ~_bitmask;
                return rdata >= _low && rdata <= _high;
            }
        }

        private int _id = 0;
        private string _name = "";
        private int _opacity = MAX_OPACITY;
        private int _luminance = MIN_LUMINANCE;

        private DataLimits _dataLimits;

        public static ItemCache<BlockInfo> BlockTable;

        public static ItemCache<int> OpacityTable;

        public static ItemCache<int> LuminanceTable;

        public static ItemCache<NBTCompoundNode> SchemaTable;

        public int ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int Opacity 
        {
            get { return _opacity; }
        }
        
        public int Luminance 
        {
            get { return _luminance; }
        }

        public BlockInfo (int id)
        {
            _id = id;
            _blockTable[_id] = this;
        }

        public BlockInfo (int id, string name)
        {
            _id = id;
            _name = name;
            _blockTable[_id] = this;
        }

        public BlockInfo SetOpacity (int opacity)
        {
            _opacity = MIN_OPACITY + opacity;
            _opacityTable[_id] = _opacity;
            return this;
        }

        public BlockInfo SetLuminance (int luminance)
        {
            _luminance = luminance;
            _luminanceTable[_id] = _luminance;
            return this;
        }

        public BlockInfo SetDataLimits (int low, int high, int bitmask)
        {
            _dataLimits = new DataLimits(low, high, bitmask);
            return this;
        }

        public bool TestData (int data)
        {
            if (_dataLimits == null) {
                return true;
            }
            return _dataLimits.Test(data);
        }

        public static BlockInfo Air;
        public static BlockInfo Stone;
        public static BlockInfo Grass;
        public static BlockInfo Dirt;
        public static BlockInfo Cobblestone;
        public static BlockInfo WoodPlank;
        public static BlockInfo Sapling;
        public static BlockInfo Bedrock;
        public static BlockInfo Water;
        public static BlockInfo StationaryWater;
        public static BlockInfo Lava;
        public static BlockInfo StationaryLava;
        public static BlockInfo Sand;
        public static BlockInfo Gravel;
        public static BlockInfo GoldOre;
        public static BlockInfo IronOre;
        public static BlockInfo CoalOre;
        public static BlockInfo Wood;
        public static BlockInfo Leaves;
        public static BlockInfo Sponge;
        public static BlockInfo Glass;
        public static BlockInfo LapisOre;
        public static BlockInfo LapisBlock;
        public static TileEntityBlockInfo Dispenser;
        public static BlockInfo Sandstone;
        public static TileEntityBlockInfo NoteBlock;
        public static BlockInfo Bed;
        public static BlockInfo Wool;
        public static BlockInfo YellowFlower;
        public static BlockInfo RedRose;
        public static BlockInfo BrownMushroom;
        public static BlockInfo RedMushroom;
        public static BlockInfo GoldBlock;
        public static BlockInfo IronBlock;
        public static BlockInfo DoubleSlab;
        public static BlockInfo Slab;
        public static BlockInfo BrickBlock;
        public static BlockInfo TNT;
        public static BlockInfo Bookshelf;
        public static BlockInfo MossStone;
        public static BlockInfo Obsidian;
        public static BlockInfo Torch;
        public static BlockInfo Fire;
        public static TileEntityBlockInfo MonsterSpawner;
        public static BlockInfo WoodStairs;
        public static TileEntityBlockInfo Chest;
        public static BlockInfo RedstoneWire;
        public static BlockInfo DiamondOre;
        public static BlockInfo DiamondBlock;
        public static BlockInfo CraftTable;
        public static BlockInfo Crops;
        public static BlockInfo Farmland;
        public static TileEntityBlockInfo Furnace;
        public static TileEntityBlockInfo BurningFurnace;
        public static TileEntityBlockInfo SignPost;
        public static BlockInfo WoodDoor;
        public static BlockInfo Ladder;
        public static BlockInfo Rails;
        public static BlockInfo CobbleStairs;
        public static TileEntityBlockInfo WallSign;
        public static BlockInfo Lever;
        public static BlockInfo StonePlate;
        public static BlockInfo IronDoor;
        public static BlockInfo WoodPlate;
        public static BlockInfo RedstoneOre;
        public static BlockInfo GlowRedstoneOre;
        public static BlockInfo RedstoneTorch;
        public static BlockInfo RedstoneTorchOn;
        public static BlockInfo StoneButton;
        public static BlockInfo Snow;
        public static BlockInfo Ice;
        public static BlockInfo SnowBlock;
        public static BlockInfo Cactus;
        public static BlockInfo ClayBlock;
        public static BlockInfo SugarCane;
        public static BlockInfo Jukebox;
        public static BlockInfo Fence;
        public static BlockInfo Pumpkin;
        public static BlockInfo Netherrack;
        public static BlockInfo SoulSand;
        public static BlockInfo Glowstone;
        public static BlockInfo Portal;
        public static BlockInfo JackOLantern;
        public static BlockInfo CakeBlock;
        public static BlockInfo RedstoneRepeater;
        public static BlockInfo RedstoneRepeaterOn;

        static BlockInfo ()
        {
            _blockTable = new BlockInfo[MAX_BLOCKS];
            _opacityTable = new int[MAX_BLOCKS];
            _luminanceTable = new int[MAX_BLOCKS];
            _schemaTable = new NBTCompoundNode[MAX_BLOCKS];

            BlockTable = new ItemCache<BlockInfo>(_blockTable);
            OpacityTable = new ItemCache<int>(_opacityTable);
            LuminanceTable = new ItemCache<int>(_luminanceTable);
            SchemaTable = new ItemCache<NBTCompoundNode>(_schemaTable);

            Air = new BlockInfo(0, "Air").SetOpacity(0);
            Stone = new BlockInfo(1, "Stone");
            Grass = new BlockInfo(2, "Grass");
            Dirt = new BlockInfo(3, "Dirt");
            Cobblestone = new BlockInfo(4, "Cobblestone");
            WoodPlank = new BlockInfo(5, "Wooden Plank");
            Sapling = new BlockInfo(6, "Sapling").SetOpacity(0);
            Bedrock = new BlockInfo(7, "Bedrock");
            Water = new BlockInfo(8, "Water").SetOpacity(3);
            StationaryWater = new BlockInfo(9, "Stationary Water").SetOpacity(3);
            Lava = new BlockInfo(10, "Lava").SetLuminance(MAX_LUMINANCE);
            StationaryLava = new BlockInfo(11, "Stationary Lava").SetLuminance(MAX_LUMINANCE);
            Sand = new BlockInfo(12, "Sand");
            Gravel = new BlockInfo(13, "Gravel");
            GoldOre = new BlockInfo(14, "Gold Ore");
            IronOre = new BlockInfo(15, "Iron Ore");
            CoalOre = new BlockInfo(16, "Coal Ore");
            Wood = new BlockInfo(17, "Wood");
            Leaves = new BlockInfo(18, "Leaves").SetOpacity(1);
            Sponge = new BlockInfo(19, "Sponge");
            Glass = new BlockInfo(20, "Glass").SetOpacity(0);
            LapisOre = new BlockInfo(21, "Lapis Lazuli Ore");
            LapisBlock = new BlockInfo(22, "Lapis Lazuli Block");
            Dispenser = new TileEntityBlockInfo(23, "Dispenser");
            Sandstone = new BlockInfo(24, "Sandstone");
            NoteBlock = new TileEntityBlockInfo(25, "Note Block");
            Bed = new BlockInfo(26, "Bed").SetOpacity(0);
            Wool = new BlockInfo(35, "Wool");
            YellowFlower = new BlockInfo(37, "Yellow Flower").SetOpacity(0);
            RedRose = new BlockInfo(38, "Red Rose").SetOpacity(0);
            BrownMushroom = new BlockInfo(39, "Brown Mushroom").SetOpacity(0).SetLuminance(1);
            RedMushroom = new BlockInfo(40, "Red Mushroom").SetOpacity(0).SetLuminance(1);
            GoldBlock = new BlockInfo(41, "Gold Block");
            IronBlock = new BlockInfo(42, "Iron Block");
            DoubleSlab = new BlockInfo(43, "Double Slab");
            Slab = new BlockInfo(44, "Slab");
            BrickBlock = new BlockInfo(45, "Brick Block");
            TNT = new BlockInfo(46, "TNT");
            Bookshelf = new BlockInfo(47, "Bookshelf");
            MossStone = new BlockInfo(48, "Moss Stone");
            Obsidian = new BlockInfo(49, "Obsidian");
            Torch = new BlockInfo(50, "Torch").SetOpacity(0).SetLuminance(MAX_LUMINANCE - 1);
            Fire = new BlockInfo(51, "Fire").SetOpacity(0).SetLuminance(MAX_LUMINANCE);
            MonsterSpawner = (TileEntityBlockInfo)new TileEntityBlockInfo(52, "Monster Spawner").SetOpacity(0);
            WoodStairs = new BlockInfo(53, "Wooden Stairs").SetOpacity(0);
            Chest = new TileEntityBlockInfo(54, "Chest");
            RedstoneWire = new BlockInfo(55, "Redstone Wire").SetOpacity(0);
            DiamondOre = new BlockInfo(56, "Diamond Ore");
            DiamondBlock = new BlockInfo(57, "Diamond Block");
            CraftTable = new BlockInfo(58, "Crafting Table");
            Crops = new BlockInfo(59, "Crops").SetOpacity(0);
            Farmland = new BlockInfo(60, "Farmland").SetOpacity(0);
            Furnace = new TileEntityBlockInfo(61, "Furnace");
            BurningFurnace = (TileEntityBlockInfo)new TileEntityBlockInfo(62, "Burning Furnace").SetLuminance(MAX_LUMINANCE - 1);
            SignPost = (TileEntityBlockInfo)new TileEntityBlockInfo(63, "Sign Post").SetOpacity(0);
            WoodDoor = new BlockInfo(64, "Wooden Door").SetOpacity(0);
            Ladder = new BlockInfo(65, "Ladder").SetOpacity(0);
            Rails = new BlockInfo(66, "Rails").SetOpacity(0);
            CobbleStairs = new BlockInfo(67, "Cobblestone Stairs").SetOpacity(0);
            WallSign = (TileEntityBlockInfo)new TileEntityBlockInfo(68, "Wall Sign").SetOpacity(0);
            Lever = new BlockInfo(69, "Lever").SetOpacity(0);
            StonePlate = new BlockInfo(70, "Stone Pressure Plate").SetOpacity(0);
            IronDoor = new BlockInfo(71, "Iron Door").SetOpacity(0);
            WoodPlate = new BlockInfo(72, "Wooden Pressure Plate").SetOpacity(0);
            RedstoneOre = new BlockInfo(73, "Redstone Ore");
            GlowRedstoneOre = new BlockInfo(74, "Glowing Redstone Ore").SetLuminance(9);
            RedstoneTorch = new BlockInfo(75, "Redstone Torch (Off)").SetOpacity(0);
            RedstoneTorchOn = new BlockInfo(76, "Redstone Torch (On)").SetOpacity(0).SetLuminance(7);
            StoneButton = new BlockInfo(77, "Stone Button").SetOpacity(0);
            Snow = new BlockInfo(78, "Snow").SetOpacity(0);
            Ice = new BlockInfo(79, "Ice").SetOpacity(3);
            SnowBlock = new BlockInfo(80, "Snow Block");
            Cactus = new BlockInfo(81, "Cactus").SetOpacity(0);
            ClayBlock = new BlockInfo(82, "Clay Block");
            SugarCane = new BlockInfo(83, "Sugar Cane").SetOpacity(0);
            Jukebox = new BlockInfo(84, "Jukebox");
            Fence = new BlockInfo(85, "Fence").SetOpacity(0);
            Pumpkin = new BlockInfo(86, "Pumpkin");
            Netherrack = new BlockInfo(87, "Netherrack");
            SoulSand = new BlockInfo(88, "Soul Sand");
            Glowstone = new BlockInfo(89, "Glowstone Block").SetLuminance(MAX_LUMINANCE);
            Portal = new BlockInfo(90, "Portal").SetOpacity(0).SetLuminance(11);
            JackOLantern = new BlockInfo(91, "Jack-O-Lantern").SetLuminance(MAX_LUMINANCE);
            CakeBlock = new BlockInfo(92, "Cake Block").SetOpacity(0);
            RedstoneRepeater = new BlockInfo(93, "Redstone Repeater (Off)").SetOpacity(0);
            RedstoneRepeaterOn = new BlockInfo(94, "Redstone Repeater (On)").SetOpacity(0).SetLuminance(7);

            for (int i = 0; i < MAX_BLOCKS; i++) {
                if (_blockTable[i] == null) {
                    _blockTable[i] = new BlockInfo(i, "Uknown Block");
                }
            }

            // Set Tile Entity Data

            Dispenser.SetTileEntity("Trap", TileEntity.TrapSchema);
            NoteBlock.SetTileEntity("Music", TileEntity.MusicSchema);
            MonsterSpawner.SetTileEntity("MonsterSpawner", TileEntity.MobSpawnerSchema);
            Chest.SetTileEntity("Chest", TileEntity.ChestSchema);
            Furnace.SetTileEntity("Furnace", TileEntity.FurnaceSchema);
            BurningFurnace.SetTileEntity("Furnace", TileEntity.FurnaceSchema);
            SignPost.SetTileEntity("Sign", TileEntity.SignSchema);
            WallSign.SetTileEntity("Sign", TileEntity.SignSchema);

            // Set Data Limits

            Wood.SetDataLimits(0, 2, 0);
            Leaves.SetDataLimits(0, 2, 0);
            Jukebox.SetDataLimits(0, 2, 0);
            Sapling.SetDataLimits(0, 15, 0);
            Cactus.SetDataLimits(0, 15, 0);
            SugarCane.SetDataLimits(0, 15, 0);
            Water.SetDataLimits(0, 7, 0x8);
            Lava.SetDataLimits(0, 7, 0x8);
            Crops.SetDataLimits(0, 7, 0);
            Wool.SetDataLimits(0, 15, 0);
            Torch.SetDataLimits(1, 5, 0);
            RedstoneTorch.SetDataLimits(0, 5, 0);
            RedstoneTorchOn.SetDataLimits(0, 5, 0);
            Rails.SetDataLimits(0, 9, 0);
            Ladder.SetDataLimits(2, 5, 0);
            WoodStairs.SetDataLimits(0, 3, 0);
            CobbleStairs.SetDataLimits(0, 3, 0);
            Lever.SetDataLimits(0, 6, 0x8);
            WoodDoor.SetDataLimits(0, 3, 0xC);
            IronDoor.SetDataLimits(0, 3, 0xC);
            StoneButton.SetDataLimits(1, 4, 0x8);
            SignPost.SetDataLimits(0, 15, 0);
            WallSign.SetDataLimits(2, 5, 0);
            Furnace.SetDataLimits(2, 5, 0);
            BurningFurnace.SetDataLimits(2, 5, 0);
            Dispenser.SetDataLimits(2, 5, 0);
            Pumpkin.SetDataLimits(0, 3, 0);
            JackOLantern.SetDataLimits(0, 3, 0);
            StonePlate.SetDataLimits(0, 0, 0x1);
            WoodPlate.SetDataLimits(0, 0, 0x1);
            Slab.SetDataLimits(0, 3, 0);
            DoubleSlab.SetDataLimits(0, 3, 0);
            Cactus.SetDataLimits(0, 5, 0);
            Bed.SetDataLimits(0, 3, 0x8);
            RedstoneRepeater.SetDataLimits(0, 0, 0xF);
            RedstoneRepeaterOn.SetDataLimits(0, 0, 0xF);
        }
    }

    public class TileEntityBlockInfo : BlockInfo
    {
        private string _tileEntityName;
        private NBTSchemaNode _tileEntitySchema;

        public string TileEntityName
        {
            get { return _tileEntityName; }
        }

        public NBTSchemaNode TileEntitySchema
        {
            get { return _tileEntitySchema; }
        }

        public TileEntityBlockInfo (int id) : base(id) { }

        public TileEntityBlockInfo (int id, string name) : base(id, name) { }

        public BlockInfo SetTileEntity (string name, NBTCompoundNode schema) {
            _tileEntityName = name;
            _tileEntitySchema = schema;
            _schemaTable[ID] = schema;
            return this;
        }
    }
}
