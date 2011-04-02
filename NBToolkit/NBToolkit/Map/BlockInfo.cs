using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit
{
    using NBT;

    public interface IBlockTileEntity
    {
        string TileEntityName { get; }

        NBTCompoundNode TileEntitySchema { get; }
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

        private int _id = 0;
        private string _name = "";
        private int _opacity = MAX_OPACITY;
        private int _luminance = MIN_LUMINANCE;

        public static ItemCache<BlockInfo> BlockTable;

        public static ItemCache<int> OpacityTable;

        public static ItemCache<int> LuminanceTable;

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

        protected static NBTCompoundNode tileEntitySchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", NBT_Type.TAG_STRING),
            new NBTScalerNode("x", NBT_Type.TAG_INT),
            new NBTScalerNode("y", NBT_Type.TAG_INT),
            new NBTScalerNode("z", NBT_Type.TAG_INT),
        };

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
        public static BlockInfoTrap Dispenser;
        public static BlockInfo Sandstone;
        public static BlockInfoMusic NoteBlock;
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
        public static BlockInfoMonster MonsterSpawner;
        public static BlockInfo WoodStairs;
        public static BlockInfoChest Chest;
        public static BlockInfo RedstoneWire;
        public static BlockInfo DiamondOre;
        public static BlockInfo DiamondBlock;
        public static BlockInfo CraftTable;
        public static BlockInfo Crops;
        public static BlockInfo Farmland;
        public static BlockInfoFurnace Furnace;
        public static BlockInfoFurnace BurningFurnace;
        public static BlockInfoSign SignPost;
        public static BlockInfo WoodDoor;
        public static BlockInfo Ladder;
        public static BlockInfo Rails;
        public static BlockInfo CobbleStairs;
        public static BlockInfoSign WallSign;
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

            BlockTable = new ItemCache<BlockInfo>(_blockTable);
            OpacityTable = new ItemCache<int>(_opacityTable);
            LuminanceTable = new ItemCache<int>(_luminanceTable);

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
            Dispenser = new BlockInfoTrap(23, "Dispenser");
            Sandstone = new BlockInfo(24, "Sandstone");
            NoteBlock = new BlockInfoMusic(25, "Note Block");
            Bed = new BlockInfo(26, "Bed").SetOpacity(0);
            Wool = new BlockInfo(35, "Wool");
            YellowFlower = new BlockInfo(37, "Yellow Flower").SetOpacity(0);
            RedRose = new BlockInfo(38, "Red Rose").SetOpacity(0);
            BrownMushroom = new BlockInfo(39, "Brown Mushroom").SetOpacity(0);
            RedMushroom = new BlockInfo(40, "Red Mushroom").SetOpacity(0);
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
            MonsterSpawner = (BlockInfoMonster)new BlockInfoMonster(52, "Monster Spawner").SetOpacity(0);
            WoodStairs = new BlockInfo(53, "Wooden Stairs").SetOpacity(0);
            Chest = new BlockInfoChest(54, "Chest");
            RedstoneWire = new BlockInfo(55, "Redstone Wire").SetOpacity(0);
            DiamondOre = new BlockInfo(56, "Diamond Ore");
            DiamondBlock = new BlockInfo(57, "Diamond Block");
            CraftTable = new BlockInfo(58, "Crafting Table");
            Crops = new BlockInfo(59, "Crops").SetOpacity(0);
            Farmland = new BlockInfo(60, "Farmland").SetOpacity(0);
            Furnace = new BlockInfoFurnace(61, "Furnace");
            BurningFurnace = (BlockInfoFurnace)new BlockInfoFurnace(62, "Burning Furnace").SetLuminance(MAX_LUMINANCE - 1);
            SignPost = (BlockInfoSign)new BlockInfoSign(63, "Sign Post").SetOpacity(0);
            WoodDoor = new BlockInfo(64, "Wooden Door").SetOpacity(0);
            Ladder = new BlockInfo(65, "Ladder").SetOpacity(0);
            Rails = new BlockInfo(66, "Rails").SetOpacity(0);
            CobbleStairs = new BlockInfo(67, "Cobblestone Stairs").SetOpacity(0);
            WallSign = (BlockInfoSign)new BlockInfoSign(68, "Wall Sign").SetOpacity(0);
            Lever = new BlockInfo(69, "Lever").SetOpacity(0);
            StonePlate = new BlockInfo(70, "Stone Pressure Plate").SetOpacity(0);
            IronDoor = new BlockInfo(71, "Iron Door").SetOpacity(0);
            WoodPlank = new BlockInfo(72, "Wooden Pressure Plate").SetOpacity(0);
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
        }
    }

    public class BlockInfoTrap : BlockInfo, IBlockTileEntity
    {
        public string TileEntityName
        {
            get { return "Trap"; }
        }

        public NBTCompoundNode TileEntitySchema
        {
            get { return tileEntitySchema; }
        }

        public BlockInfoTrap (int id, string name)
            : base(id, name)
        {
        }

        protected static new NBTCompoundNode tileEntitySchema = BlockInfo.tileEntitySchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND),
        });
    }

    public class BlockInfoMusic : BlockInfo, IBlockTileEntity
    {
        public string TileEntityName
        {
            get { return "Music"; }
        }

        public NBTCompoundNode TileEntitySchema
        {
            get { return tileEntitySchema; }
        }

        public BlockInfoMusic (int id, string name)
            : base(id, name)
        {
        }

        protected static new NBTCompoundNode tileEntitySchema = BlockInfo.tileEntitySchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("note", NBT_Type.TAG_COMPOUND),
        });
    }

    public class BlockInfoMonster : BlockInfo, IBlockTileEntity
    {
        public string TileEntityName
        {
            get { return "Monster Spawner"; }
        }

        public NBTCompoundNode TileEntitySchema
        {
            get { return tileEntitySchema; }
        }

        public BlockInfoMonster (int id, string name)
            : base(id, name)
        {
        }

        protected static new NBTCompoundNode tileEntitySchema = BlockInfo.tileEntitySchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("EntityId", NBT_Type.TAG_STRING),
            new NBTScalerNode("Delay", NBT_Type.TAG_SHORT),
        });
    }

    public class BlockInfoChest : BlockInfo, IBlockTileEntity
    {
        public string TileEntityName
        {
            get { return "Trap"; }
        }

        public NBTCompoundNode TileEntitySchema
        {
            get { return tileEntitySchema; }
        }

        public BlockInfoChest (int id, string name)
            : base(id, name)
        {
        }

        protected static new NBTCompoundNode tileEntitySchema = BlockInfo.tileEntitySchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND),
        });
    }

    public class BlockInfoFurnace : BlockInfo, IBlockTileEntity
    {
        public string TileEntityName
        {
            get { return "Furnace"; }
        }

        public NBTCompoundNode TileEntitySchema
        {
            get { return tileEntitySchema; }
        }

        public BlockInfoFurnace (int id, string name)
            : base(id, name)
        {
        }

        protected static new NBTCompoundNode tileEntitySchema = BlockInfo.tileEntitySchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("BurnTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("CookTime", NBT_Type.TAG_SHORT),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND),
        });
    }

    public class BlockInfoSign : BlockInfo, IBlockTileEntity
    {
        public string TileEntityName
        {
            get { return "Sign"; }
        }

        public NBTCompoundNode TileEntitySchema
        {
            get { return tileEntitySchema; }
        }

        public BlockInfoSign (int id, string name)
            : base(id, name)
        {
        }

        protected static new NBTCompoundNode tileEntitySchema = BlockInfo.tileEntitySchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Text1", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text2", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text3", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text4", NBT_Type.TAG_STRING),
        });
    }
}
