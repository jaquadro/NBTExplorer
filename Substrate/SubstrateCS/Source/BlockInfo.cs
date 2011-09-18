using System;
using System.Collections.Generic;
using Substrate.Nbt;

namespace Substrate
{
    /// <summary>
    /// Provides named id values for known block types.
    /// </summary>
    public static class BlockType
    {
        public const int AIR = 0;
        public const int STONE = 1;
        public const int GRASS = 2;
        public const int DIRT = 3;
        public const int COBBLESTONE = 4;
        public const int WOOD_PLANK = 5;
        public const int SAPLING = 6;
        public const int BEDROCK = 7;
        public const int WATER = 8;
        public const int STATIONARY_WATER = 9;
        public const int LAVA = 10;
        public const int STATIONARY_LAVA = 11;
        public const int SAND = 12;
        public const int GRAVEL = 13;
        public const int GOLD_ORE = 14;
        public const int IRON_ORE = 15;
        public const int COAL_ORE = 16;
        public const int WOOD = 17;
        public const int LEAVES = 18;
        public const int SPONGE = 19;
        public const int GLASS = 20;
        public const int LAPIS_ORE = 21;
        public const int LAPIS_BLOCK = 22;
        public const int DISPENSER = 23;
        public const int SANDSTONE = 24;
        public const int NOTE_BLOCK = 25;
        public const int BED = 26;
        public const int POWERED_RAIL = 27;
        public const int DETECTOR_RAIL = 28;
        public const int STICKY_PISTON = 29;
        public const int COBWEB = 30;
        public const int TALL_GRASS = 31;
        public const int DEAD_SHRUB = 32;
        public const int PISTON = 33;
        public const int PISTON_HEAD = 34;
        public const int WOOL = 35;
        public const int PISTON_MOVING = 36;
        public const int YELLOW_FLOWER = 37;
        public const int RED_ROSE = 38;
        public const int BROWN_MUSHROOM = 39;
        public const int RED_MUSHROOM = 40;
        public const int GOLD_BLOCK = 41;
        public const int IRON_BLOCK = 42;
        public const int DOUBLE_SLAB = 43;
        public const int SLAB = 44;
        public const int BRICK_BLOCK = 45;
        public const int TNT = 46;
        public const int BOOKSHELF = 47;
        public const int MOSS_STONE = 48;
        public const int OBSIDIAN = 49;
        public const int TORCH = 50;
        public const int FIRE = 51;
        public const int MONSTER_SPAWNER = 52;
        public const int WOOD_STAIRS = 53;
        public const int CHEST = 54;
        public const int REDSTONE_WIRE = 55;
        public const int DIAMOND_ORE = 56;
        public const int DIAMOND_BLOCK = 57;
        public const int CRAFTING_TABLE = 58;
        public const int CROPS = 59;
        public const int FARMLAND = 60;
        public const int FURNACE = 61;
        public const int BURNING_FURNACE = 62;
        public const int SIGN_POST = 63;
        public const int WOOD_DOOR = 64;
        public const int LADDER = 65;
        public const int RAILS = 66;
        public const int COBBLESTONE_STAIRS = 67;
        public const int WALL_SIGN = 68;
        public const int LEVER = 69;
        public const int STONE_PLATE = 70;
        public const int IRON_DOOR = 71;
        public const int WOOD_PLATE = 72;
        public const int REDSTONE_ORE = 73;
        public const int GLOWING_REDSTONE_ORE = 74;
        public const int REDSTONE_TORCH_OFF = 75;
        public const int REDSTONE_TORCH_ON = 76;
        public const int STONE_BUTTON = 77;
        public const int SNOW = 78;
        public const int ICE = 79;
        public const int SNOW_BLOCK = 80;
        public const int CACTUS = 81;
        public const int CLAY_BLOCK = 82;
        public const int SUGAR_CANE = 83;
        public const int JUKEBOX = 84;
        public const int FENCE = 85;
        public const int PUMPKIN = 86;
        public const int NETHERRACK = 87;
        public const int SOUL_SAND = 88;
        public const int GLOWSTONE_BLOCK = 89;
        public const int PORTAL = 90;
        public const int JACK_O_LANTERN = 91;
        public const int CAKE_BLOCK = 92;
        public const int REDSTONE_REPEATER_ON = 93;
        public const int REDSTONE_REPEATER_OFF = 94;
        public const int LOCKED_CHEST = 95;
        public const int TRAPDOOR = 96;
        public const int SILVERFISH_STONE = 97;
        public const int STONE_BRICK = 98;
        public const int HUGE_RED_MUSHROOM = 99;
        public const int HUGE_BROWN_MUSHROOM = 100;
        public const int IRON_BARS = 101;
        public const int GLASS_PANE = 102;
        public const int MELON = 103;
        public const int PUMPKIN_STEM = 104;
        public const int MELON_STEM = 105;
        public const int VINES = 106;
        public const int FENCE_GATE = 107;
        public const int BRICK_STAIRS = 108;
        public const int STONE_BRICK_STAIRS = 109;
    }

    /// <summary>
    /// Represents the physical state of a block, such as solid or fluid.
    /// </summary>
    public enum BlockState
    {
        /// <summary>
        /// A solid state that stops movement.
        /// </summary>
        SOLID,

        /// <summary>
        /// A nonsolid state that can be passed through.
        /// </summary>
        NONSOLID,

        /// <summary>
        /// A fluid state that flows and impedes movement.
        /// </summary>
        FLUID
    }

    /// <summary>
    /// Provides information on a specific type of block.
    /// </summary>
    /// <remarks>By default, all known MC block types are already defined and registered, assuming Substrate
    /// is up to date with the current MC version.  All unknown blocks are given a default type and unregistered status.
    /// New block types may be created and used at runtime, and will automatically populate various static lookup tables
    /// in the <see cref="BlockInfo"/> class.</remarks>
    public class BlockInfo
    {
        /// <summary>
        /// The maximum number of sequential blocks starting at 0 that can be registered.
        /// </summary>
        public const int MAX_BLOCKS = 256;

        /// <summary>
        /// The maximum opacity value that can be assigned to a block (fully opaque).
        /// </summary>
        public const int MAX_OPACITY = 15;

        /// <summary>
        /// The minimum opacity value that can be assigned to a block (fully transparent).
        /// </summary>
        public const int MIN_OPACITY = 0;

        /// <summary>
        /// The maximum luminance value that can be assigned to a block.
        /// </summary>
        public const int MAX_LUMINANCE = 15;

        /// <summary>
        /// The minimum luminance value that can be assigned to a block.
        /// </summary>
        public const int MIN_LUMINANCE = 0;

        private static readonly BlockInfo[] _blockTable;
        private static readonly int[] _opacityTable;
        private static readonly int[] _luminanceTable;

        private class CacheTableArray<T> : ICacheTable<T>
        {
            private T[] _cache;

            public T this[int index]
            {
                get { return _cache[index]; }
            }

            public CacheTableArray (T[] cache)
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
        private bool _transmitLight = false;
        private bool _blocksFluid = true;
        private bool _registered = false;

        private BlockState _state = BlockState.SOLID;

        private DataLimits _dataLimits;

        private static readonly CacheTableArray<BlockInfo> _blockTableCache;
        private static readonly CacheTableArray<int> _opacityTableCache;
        private static readonly CacheTableArray<int> _luminanceTableCache;

        /// <summary>
        /// Gets the lookup table for id-to-info values.
        /// </summary>
        public static ICacheTable<BlockInfo> BlockTable
        {
            get { return _blockTableCache; }
        }

        /// <summary>
        /// Gets the lookup table for id-to-opacity values.
        /// </summary>
        public static ICacheTable<int> OpacityTable
        {
            get { return _opacityTableCache; }
        }

        /// <summary>
        /// Gets the lookup table for id-to-luminance values.
        /// </summary>
        public static ICacheTable<int> LuminanceTable
        {
            get { return _luminanceTableCache; }
        }

        /// <summary>
        /// Get's the block's Id.
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        /// <summary>
        /// Get's the name of the block type.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the block's opacity value.  An opacity of 0 is fully transparent to light.
        /// </summary>
        public int Opacity 
        {
            get { return _opacity; }
        }
        
        /// <summary>
        /// Gets the block's luminance value.
        /// </summary>
        /// <remarks>Blocks with luminance act as light sources and transmit light to other blocks.</remarks>
        public int Luminance 
        {
            get { return _luminance; }
        }

        /// <summary>
        /// Checks whether the block transmits light to neighboring blocks.
        /// </summary>
        /// <remarks>A block may stop the transmission of light, but still be illuminated.</remarks>
        public bool TransmitsLight
        {
            get { return _transmitLight; }
        }

        /// <summary>
        /// Checks whether the block partially or fully blocks the transmission of light.
        /// </summary>
        public bool ObscuresLight
        {
            get { return _opacity > MIN_OPACITY || !_transmitLight; }
        }

        /// <summary>
        /// Checks whether the block stops fluid from passing through it.
        /// </summary>
        /// <remarks>A block that does not block fluids will be destroyed by fluid.</remarks>
        public bool BlocksFluid
        {
            get { return _blocksFluid; }
        }

        /// <summary>
        /// Gets the block's physical state type.
        /// </summary>
        public BlockState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Checks whether this block type has been registered as a known type.
        /// </summary>
        public bool Registered
        {
            get { return _registered; }
        }

        internal BlockInfo (int id)
        {
            _id = id;
            _name = "Unknown Block";
            _blockTable[_id] = this;
        }

        /// <summary>
        /// Constructs a new <see cref="BlockInfo"/> record for a given block id and name.
        /// </summary>
        /// <param name="id">The id of the block.</param>
        /// <param name="name">The name of the block.</param>
        /// <remarks>All user-constructed <see cref="BlockInfo"/> objects are registered automatically.</remarks>
        public BlockInfo (int id, string name)
        {
            _id = id;
            _name = name;
            _blockTable[_id] = this;
            _registered = true;
        }

        /// <summary>
        /// Sets a new opacity value for this block type.
        /// </summary>
        /// <param name="opacity">A new opacity value.</param>
        /// <returns>The object instance used to invoke this method.</returns>
        public BlockInfo SetOpacity (int opacity)
        {
            _opacity = MIN_OPACITY + opacity;
            _opacityTable[_id] = _opacity;

            if (opacity == MAX_OPACITY) {
                _transmitLight = false;
            }
            else {
                _transmitLight = true;
            }

            return this;
        }

        /// <summary>
        /// Sets a new luminance value for this block type.
        /// </summary>
        /// <param name="luminance">A new luminance value.</param>
        /// <returns>The object instance used to invoke this method.</returns>
        public BlockInfo SetLuminance (int luminance)
        {
            _luminance = luminance;
            _luminanceTable[_id] = _luminance;
            return this;
        }

        /// <summary>
        /// Sets whether or not this block type will transmit light to neigboring blocks.
        /// </summary>
        /// <param name="transmit">True if this block type can transmit light to neighbors, false otherwise.</param>
        /// <returns>The object instance used to invoke this method.</returns>
        public BlockInfo SetLightTransmission (bool transmit)
        {
            _transmitLight = transmit;
            return this;
        }

        /// <summary>
        /// Sets limitations on what data values are considered valid for this block type.
        /// </summary>
        /// <param name="low">The lowest valid integer value.</param>
        /// <param name="high">The highest valid integer value.</param>
        /// <param name="bitmask">A mask representing which bits are interpreted as a bitmask in the data value.</param>
        /// <returns>The object instance used to invoke this method.</returns>
        public BlockInfo SetDataLimits (int low, int high, int bitmask)
        {
            _dataLimits = new DataLimits(low, high, bitmask);
            return this;
        }

        /// <summary>
        /// Sets the physical state of the block type.
        /// </summary>
        /// <param name="state">A physical state.</param>
        /// <returns>The object instance used to invoke this method.</returns>
        public BlockInfo SetState (BlockState state)
        {
            _state = state;

            if (_state == BlockState.SOLID) {
                _blocksFluid = true;
            }
            else {
                _blocksFluid = false;
            }

            return this;
        }

        /// <summary>
        /// Sets whether or not this block type blocks fluids.
        /// </summary>
        /// <param name="blocks">True if this block type blocks fluids, false otherwise.</param>
        /// <returns>The object instance used to invoke this method.</returns>
        public BlockInfo SetBlocksFluid (bool blocks)
        {
            _blocksFluid = blocks;
            return this;
        }

        /// <summary>
        /// Tests if the given data value is valid for this block type.
        /// </summary>
        /// <param name="data">A data value to test.</param>
        /// <returns>True if the data value is valid, false otherwise.</returns>
        /// <remarks>This method uses internal information set by <see cref="SetDataLimits"/>.</remarks>
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
        public static BlockInfoEx Dispenser;
        public static BlockInfo Sandstone;
        public static BlockInfoEx NoteBlock;
        public static BlockInfo Bed;
        public static BlockInfo PoweredRail;
        public static BlockInfo DetectorRail;
        public static BlockInfo StickyPiston;
        public static BlockInfo Cobweb;
        public static BlockInfo TallGrass;
        public static BlockInfo DeadShrub;
        public static BlockInfo Piston;
        public static BlockInfo PistonHead;
        public static BlockInfo Wool;
        public static BlockInfoEx PistonMoving;
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
        public static BlockInfoEx MonsterSpawner;
        public static BlockInfo WoodStairs;
        public static BlockInfoEx Chest;
        public static BlockInfo RedstoneWire;
        public static BlockInfo DiamondOre;
        public static BlockInfo DiamondBlock;
        public static BlockInfo CraftTable;
        public static BlockInfo Crops;
        public static BlockInfo Farmland;
        public static BlockInfoEx Furnace;
        public static BlockInfoEx BurningFurnace;
        public static BlockInfoEx SignPost;
        public static BlockInfo WoodDoor;
        public static BlockInfo Ladder;
        public static BlockInfo Rails;
        public static BlockInfo CobbleStairs;
        public static BlockInfoEx WallSign;
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
        public static BlockInfoEx LockedChest;
        public static BlockInfo Trapdoor;
        public static BlockInfo SilverfishStone;
        public static BlockInfo StoneBrick;
        public static BlockInfo HugeRedMushroom;
        public static BlockInfo HugeBrownMushroom;
        public static BlockInfo IronBars;
        public static BlockInfo GlassPane;
        public static BlockInfo Melon;
        public static BlockInfo PumpkinStem;
        public static BlockInfo MelonStem;
        public static BlockInfo Vines;
        public static BlockInfo FenceGate;
        public static BlockInfo BrickStairs;
        public static BlockInfo StoneBrickStairs;

        static BlockInfo ()
        {
            _blockTable = new BlockInfo[MAX_BLOCKS];
            _opacityTable = new int[MAX_BLOCKS];
            _luminanceTable = new int[MAX_BLOCKS];

            _blockTableCache = new CacheTableArray<BlockInfo>(_blockTable);
            _opacityTableCache = new CacheTableArray<int>(_opacityTable);
            _luminanceTableCache = new CacheTableArray<int>(_luminanceTable);

            Air = new BlockInfo(0, "Air").SetOpacity(0).SetState(BlockState.NONSOLID);
            Stone = new BlockInfo(1, "Stone");
            Grass = new BlockInfo(2, "Grass");
            Dirt = new BlockInfo(3, "Dirt");
            Cobblestone = new BlockInfo(4, "Cobblestone");
            WoodPlank = new BlockInfo(5, "Wooden Plank");
            Sapling = new BlockInfo(6, "Sapling").SetOpacity(0).SetState(BlockState.NONSOLID);
            Bedrock = new BlockInfo(7, "Bedrock");
            Water = new BlockInfo(8, "Water").SetOpacity(3).SetState(BlockState.FLUID);
            StationaryWater = new BlockInfo(9, "Stationary Water").SetOpacity(3).SetState(BlockState.FLUID);
            Lava = new BlockInfo(10, "Lava").SetOpacity(0).SetLuminance(MAX_LUMINANCE).SetState(BlockState.FLUID);
            StationaryLava = new BlockInfo(11, "Stationary Lava").SetOpacity(0).SetLuminance(MAX_LUMINANCE).SetState(BlockState.FLUID);
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
            Dispenser = new BlockInfoEx(23, "Dispenser");
            Sandstone = new BlockInfo(24, "Sandstone");
            NoteBlock = new BlockInfoEx(25, "Note Block");
            Bed = new BlockInfo(26, "Bed").SetOpacity(0);
            PoweredRail = new BlockInfo(27, "Powered Rail").SetOpacity(0).SetState(BlockState.NONSOLID);
            DetectorRail = new BlockInfo(28, "Detector Rail").SetOpacity(0).SetState(BlockState.NONSOLID);
            StickyPiston = new BlockInfo(29, "Sticky Piston").SetOpacity(0);
            Cobweb = new BlockInfo(30, "Cobweb").SetOpacity(0).SetState(BlockState.NONSOLID);
            TallGrass = new BlockInfo(31, "Tall Grass").SetOpacity(0).SetState(BlockState.NONSOLID);
            DeadShrub = new BlockInfo(32, "Dead Shrub").SetOpacity(0).SetState(BlockState.NONSOLID);
            Piston = new BlockInfo(33, "Piston").SetOpacity(0);
            PistonHead = new BlockInfo(34, "Piston Head").SetOpacity(0);
            Wool = new BlockInfo(35, "Wool");
            PistonMoving = (BlockInfoEx)new BlockInfoEx(36, "Piston Moving").SetOpacity(0);
            YellowFlower = new BlockInfo(37, "Yellow Flower").SetOpacity(0).SetState(BlockState.NONSOLID);
            RedRose = new BlockInfo(38, "Red Rose").SetOpacity(0).SetState(BlockState.NONSOLID);
            BrownMushroom = new BlockInfo(39, "Brown Mushroom").SetOpacity(0).SetLuminance(1).SetState(BlockState.NONSOLID);
            RedMushroom = new BlockInfo(40, "Red Mushroom").SetOpacity(0).SetState(BlockState.NONSOLID);
            GoldBlock = new BlockInfo(41, "Gold Block");
            IronBlock = new BlockInfo(42, "Iron Block");
            DoubleSlab = new BlockInfo(43, "Double Slab");
            Slab = new BlockInfo(44, "Slab").SetOpacity(0);
            BrickBlock = new BlockInfo(45, "Brick Block");
            TNT = new BlockInfo(46, "TNT");
            Bookshelf = new BlockInfo(47, "Bookshelf");
            MossStone = new BlockInfo(48, "Moss Stone");
            Obsidian = new BlockInfo(49, "Obsidian");
            Torch = new BlockInfo(50, "Torch").SetOpacity(0).SetLuminance(MAX_LUMINANCE - 1).SetState(BlockState.NONSOLID);
            Fire = new BlockInfo(51, "Fire").SetOpacity(0).SetLuminance(MAX_LUMINANCE).SetState(BlockState.NONSOLID);
            MonsterSpawner = (BlockInfoEx)new BlockInfoEx(52, "Monster Spawner").SetOpacity(0);
            WoodStairs = new BlockInfo(53, "Wooden Stairs").SetOpacity(0);
            Chest = (BlockInfoEx)new BlockInfoEx(54, "Chest").SetOpacity(0);
            RedstoneWire = new BlockInfo(55, "Redstone Wire").SetOpacity(0).SetState(BlockState.NONSOLID);
            DiamondOre = new BlockInfo(56, "Diamond Ore");
            DiamondBlock = new BlockInfo(57, "Diamond Block");
            CraftTable = new BlockInfo(58, "Crafting Table");
            Crops = new BlockInfo(59, "Crops").SetOpacity(0).SetState(BlockState.NONSOLID);
            Farmland = new BlockInfo(60, "Farmland").SetOpacity(0);
            Furnace = new BlockInfoEx(61, "Furnace");
            BurningFurnace = (BlockInfoEx)new BlockInfoEx(62, "Burning Furnace").SetLuminance(MAX_LUMINANCE - 1);
            SignPost = (BlockInfoEx)new BlockInfoEx(63, "Sign Post").SetOpacity(0).SetState(BlockState.NONSOLID);
            WoodDoor = new BlockInfo(64, "Wooden Door").SetOpacity(0);
            Ladder = new BlockInfo(65, "Ladder").SetOpacity(0);
            Rails = new BlockInfo(66, "Rails").SetOpacity(0).SetState(BlockState.NONSOLID);
            CobbleStairs = new BlockInfo(67, "Cobblestone Stairs").SetOpacity(0);
            WallSign = (BlockInfoEx)new BlockInfoEx(68, "Wall Sign").SetOpacity(0).SetState(BlockState.NONSOLID);
            Lever = new BlockInfo(69, "Lever").SetOpacity(0).SetState(BlockState.NONSOLID);
            StonePlate = new BlockInfo(70, "Stone Pressure Plate").SetOpacity(0).SetState(BlockState.NONSOLID);
            IronDoor = new BlockInfo(71, "Iron Door").SetOpacity(0);
            WoodPlate = new BlockInfo(72, "Wooden Pressure Plate").SetOpacity(0).SetState(BlockState.NONSOLID);
            RedstoneOre = new BlockInfo(73, "Redstone Ore");
            GlowRedstoneOre = new BlockInfo(74, "Glowing Redstone Ore").SetLuminance(9);
            RedstoneTorch = new BlockInfo(75, "Redstone Torch (Off)").SetOpacity(0).SetState(BlockState.NONSOLID);
            RedstoneTorchOn = new BlockInfo(76, "Redstone Torch (On)").SetOpacity(0).SetLuminance(7).SetState(BlockState.NONSOLID);
            StoneButton = new BlockInfo(77, "Stone Button").SetOpacity(0).SetState(BlockState.NONSOLID);
            Snow = new BlockInfo(78, "Snow").SetOpacity(0).SetState(BlockState.NONSOLID);
            Ice = new BlockInfo(79, "Ice").SetOpacity(3);
            SnowBlock = new BlockInfo(80, "Snow Block");
            Cactus = new BlockInfo(81, "Cactus").SetOpacity(0);
            ClayBlock = new BlockInfo(82, "Clay Block");
            SugarCane = new BlockInfo(83, "Sugar Cane").SetOpacity(0).SetState(BlockState.NONSOLID);
            Jukebox = new BlockInfo(84, "Jukebox");
            Fence = new BlockInfo(85, "Fence").SetOpacity(0);
            Pumpkin = new BlockInfo(86, "Pumpkin");
            Netherrack = new BlockInfo(87, "Netherrack");
            SoulSand = new BlockInfo(88, "Soul Sand");
            Glowstone = new BlockInfo(89, "Glowstone Block").SetLuminance(MAX_LUMINANCE);
            Portal = new BlockInfo(90, "Portal").SetOpacity(0).SetLuminance(11).SetState(BlockState.NONSOLID);
            JackOLantern = new BlockInfo(91, "Jack-O-Lantern").SetLuminance(MAX_LUMINANCE);
            CakeBlock = new BlockInfo(92, "Cake Block").SetOpacity(0);
            RedstoneRepeater = new BlockInfo(93, "Redstone Repeater (Off)").SetOpacity(0);
            RedstoneRepeaterOn = new BlockInfo(94, "Redstone Repeater (On)").SetOpacity(0).SetLuminance(7);
            LockedChest = (BlockInfoEx)new BlockInfoEx(95, "Locked Chest").SetLuminance(MAX_LUMINANCE);
            Trapdoor = new BlockInfo(96, "Trapdoor").SetOpacity(0);
            SilverfishStone = new BlockInfo(97, "Stone with Silverfish");
            StoneBrick = new BlockInfo(98, "Stone Brick");
            HugeRedMushroom = new BlockInfo(99, "Huge Red Mushroom");
            HugeBrownMushroom = new BlockInfo(100, "Huge Brown Mushroom");
            IronBars = new BlockInfo(101, "Iron Bars").SetOpacity(0);
            GlassPane = new BlockInfo(102, "Glass Pane").SetOpacity(0);
            Melon = new BlockInfo(103, "Melon");
            PumpkinStem = new BlockInfo(104, "Pumpkin Stem").SetOpacity(0).SetState(BlockState.NONSOLID);
            MelonStem = new BlockInfo(105, "Melon Stem").SetOpacity(0).SetState(BlockState.NONSOLID);
            Vines = new BlockInfo(106, "Vines").SetOpacity(0).SetState(BlockState.NONSOLID);
            FenceGate = new BlockInfo(107, "Fence Gate").SetOpacity(0);
            BrickStairs = new BlockInfo(108, "Brick Stairs").SetOpacity(0);
            StoneBrickStairs = new BlockInfo(109, "Stone Brick Stairs").SetOpacity(0);

            for (int i = 0; i < MAX_BLOCKS; i++) {
                if (_blockTable[i] == null) {
                    _blockTable[i] = new BlockInfo(i);
                }
            }

            // Override default light transmission rules

            Lava.SetLightTransmission(false);
            StationaryLava.SetLightTransmission(false);
            Slab.SetLightTransmission(false);
            WoodStairs.SetLightTransmission(false);
            Farmland.SetLightTransmission(false);
            CobbleStairs.SetLightTransmission(false);
            BrickStairs.SetLightTransmission(false);
            StoneBrickStairs.SetLightTransmission(false);

            // Override default fluid blocking rules

            SignPost.SetBlocksFluid(true);
            WallSign.SetBlocksFluid(true);
            Cactus.SetBlocksFluid(false);

            // Set Tile Entity Data

            Dispenser.SetTileEntity("Trap");
            NoteBlock.SetTileEntity("Music");
            PistonMoving.SetTileEntity("Piston");
            MonsterSpawner.SetTileEntity("MobSpawner");
            Chest.SetTileEntity("Chest");
            Furnace.SetTileEntity("Furnace");
            BurningFurnace.SetTileEntity("Furnace");
            SignPost.SetTileEntity("Sign");
            WallSign.SetTileEntity("Sign");

            // Set Data Limits

            Wood.SetDataLimits(0, 2, 0);
            Leaves.SetDataLimits(0, 2, 0);
            Jukebox.SetDataLimits(0, 2, 0);
            Sapling.SetDataLimits(0, 15, 0);
            Cactus.SetDataLimits(0, 15, 0);
            SugarCane.SetDataLimits(0, 15, 0);
            Water.SetDataLimits(0, 7, 0x8);
            Lava.SetDataLimits(0, 7, 0x8);
            TallGrass.SetDataLimits(0, 2, 0);
            Crops.SetDataLimits(0, 7, 0);
            PoweredRail.SetDataLimits(0, 5, 0x8);
            DetectorRail.SetDataLimits(0, 5, 0x8);
            StickyPiston.SetDataLimits(1, 5, 0x8);
            Piston.SetDataLimits(1, 5, 0x8);
            PistonHead.SetDataLimits(1, 5, 0x8);
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
            Trapdoor.SetDataLimits(0, 3, 0x4);
            StoneBrick.SetDataLimits(0, 2, 0);
            HugeRedMushroom.SetDataLimits(0, 10, 0);
            HugeBrownMushroom.SetDataLimits(0, 10, 0);
            Vines.SetDataLimits(0, 0, 0xF);
            FenceGate.SetDataLimits(0, 3, 0x4);
        }
    }

    /// <summary>
    /// An extended <see cref="BlockInfo"/> that includes <see cref="TileEntity"/> information.
    /// </summary>
    public class BlockInfoEx : BlockInfo
    {
        private string _tileEntityName;

        /// <summary>
        /// Gets the name of the <see cref="TileEntity"/> type associated with this block type.
        /// </summary>
        public string TileEntityName
        {
            get { return _tileEntityName; }
        }

        internal BlockInfoEx (int id) : base(id) { }

        /// <summary>
        /// Constructs a new <see cref="BlockInfoEx"/> with a given block id and name.
        /// </summary>
        /// <param name="id">The id of the block type.</param>
        /// <param name="name">The name of the block type.</param>
        public BlockInfoEx (int id, string name) : base(id, name) { }

        /// <summary>
        /// Sets the name of the <see cref="TileEntity"/> type associated with this block type.
        /// </summary>
        /// <param name="name">The name of a registered <see cref="TileEntity"/> type.</param>
        /// <returns>The object instance used to invoke this method.</returns>
        /// <seealso cref="TileEntityFactory"/>
        public BlockInfo SetTileEntity (string name) {
            _tileEntityName = name;
            return this;
        }
    }
}
