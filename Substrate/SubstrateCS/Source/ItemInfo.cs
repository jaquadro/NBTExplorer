using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using Nbt;

    public static class ItemType
    {
        public const int IRON_SHOVEL = 256;
        public const int IRON_PICKAXE = 257;
        public const int IRON_AXE = 258;
        public const int FLINT_AND_STEEL = 259;
        public const int APPLE = 260;
        public const int BOW = 261;
        public const int ARROW = 262;
        public const int COAL = 263;
        public const int DIAMOND = 264;
        public const int IRON_INGOT = 265;
        public const int GOLD_INGOT = 266;
        public const int IRON_SWORD = 267;
        public const int WOODEN_SWORD = 268;
        public const int WOODEN_SHOVEL = 269;
        public const int WOODEN_PICKAXE = 270;
        public const int WOODEN_AXE = 271;
        public const int STONE_SWORD = 272;
        public const int STONE_SHOVEL = 273;
        public const int STONE_PICKAXE = 274;
        public const int STONE_AXE = 275;
        public const int DIAMOND_SWORD = 276;
        public const int DIAMOND_SHOVEL = 277;
        public const int DIAMOND_PICKAXE = 278;
        public const int DIAMOND_AXE = 279;
        public const int STICK = 280;
        public const int BOWL = 281;
        public const int MUSHROOM_SOUP = 282;
        public const int GOLD_SWORD = 283;
        public const int GOLD_SHOVEL = 284;
        public const int GOLD_PICKAXE = 285;
        public const int GOLD_AXE = 286;
        public const int STRING = 287;
        public const int FEATHER = 288;
        public const int GUNPOWDER = 289;
        public const int WOODEN_HOE = 290;
        public const int STONE_HOE = 291;
        public const int IRON_HOE = 292;
        public const int DIAMOND_HOE = 293;
        public const int GOLD_HOE = 294;
        public const int SEEDS = 295;
        public const int WHEAT = 296;
        public const int BREAD = 297;
        public const int LEATHER_CAP = 298;
        public const int LEATHER_TUNIC = 299;
        public const int LEATHER_PANTS = 300;
        public const int LEATHER_BOOTS = 301;
        public const int CHAIN_HELMET = 302;
        public const int CHAIN_CHESTPLATE = 303;
        public const int CHAIN_LEGGINGS = 304;
        public const int CHAIN_BOOTS = 305;
        public const int IRON_HELMET = 306;
        public const int IRON_CHESTPLATE = 307;
        public const int IRON_LEGGINGS = 308;
        public const int IRON_BOOTS = 309;
        public const int DIAMOND_HELMET = 310;
        public const int DIAMOND_CHESTPLATE = 311;
        public const int DIAMOND_LEGGINGS = 312;
        public const int DIAMOND_BOOTS = 313;
        public const int GOLD_HELMET = 314;
        public const int GOLD_CHESTPLATE = 315;
        public const int GOLD_LEGGINGS = 316;
        public const int GOLD_BOOTS = 317;
        public const int FLINT = 318;
        public const int RAW_PORKCHOP = 319;
        public const int COOKED_PORKCHOP = 320;
        public const int PAINTING = 321;
        public const int GOLDEN_APPLE = 322;
        public const int SIGN = 323;
        public const int WOODEN_DOOR = 324;
        public const int BUCKET = 325;
        public const int WATER_BUCKET = 326;
        public const int LAVA_BUCKET = 327;
        public const int MINECART = 328;
        public const int SADDLE = 329;
        public const int IRON_DOOR = 330;
        public const int REDSTONE_DUST = 331;
        public const int SNOWBALL = 332;
        public const int BOAT = 333;
        public const int LEATHER = 334;
        public const int MILK = 335;
        public const int CLAY_BRICK = 336;
        public const int CLAY = 337;
        public const int SUGAR_CANE = 338;
        public const int PAPER = 339;
        public const int BOOK = 340;
        public const int SLIMEBALL = 341;
        public const int STORAGE_MINECART = 342;
        public const int POWERED_MINECARD = 343;
        public const int EGG = 344;
        public const int COMPASS = 345;
        public const int FISHING_ROD = 346;
        public const int CLOCK = 347;
        public const int GLOWSTONE_DUST = 348;
        public const int RAW_FISH = 349;
        public const int COOKED_FISH = 350;
        public const int DYE = 351;
        public const int BONE = 352;
        public const int SUGAR = 353;
        public const int CAKE = 354;
        public const int BED = 355;
        public const int REDSTONE_REPEATER = 356;
        public const int COOKIE = 357;
        public const int MAP = 358;
        public const int GOLD_MUSIC_DISC = 2256;
        public const int GREEN_MUSIC_DISC = 2257;
    }

    public class ItemInfo
    {
        public class ItemCache<T>
        {
            private Dictionary<int, T> _cache;
            private static Random _rand = new Random();

            public T this[int index]
            {
                get 
                {
                    T val;
                    if (_cache.TryGetValue(index, out val)) {
                        return val;
                    }
                    return default(T);
                }
            }

            public ItemCache (Dictionary<int, T> cache)
            {
                _cache = cache;
            }

            public T Random ()
            {
                List<T> list = new List<T>(_cache.Values);
                return list[_rand.Next(list.Count)];
            }
        }

        private static Dictionary<int, ItemInfo> _itemTable;

        private int _id = 0;
        private string _name = "";
        private int _stack = 1;

        public static ItemCache<ItemInfo> ItemTable;

        public int ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int StackSize
        {
            get { return _stack; }
        }

        public ItemInfo (int id)
        {
            _id = id;
            _itemTable[_id] = this;
        }

        public ItemInfo (int id, string name)
        {
            _id = id;
            _name = name;
            _itemTable[_id] = this;
        }

        public ItemInfo SetStackSize (int stack)
        {
            _stack = stack;
            return this;
        }


        public static ItemInfo IronShovel;
        public static ItemInfo IronPickaxe;
        public static ItemInfo IronAxe;
        public static ItemInfo FlintAndSteel;
        public static ItemInfo Apple;
        public static ItemInfo Bow;
        public static ItemInfo Arrow;
        public static ItemInfo Coal;
        public static ItemInfo Diamond;
        public static ItemInfo IronIngot;
        public static ItemInfo GoldIngot;
        public static ItemInfo IronSword;
        public static ItemInfo WoodenSword;
        public static ItemInfo WoodenShovel;
        public static ItemInfo WoodenPickaxe;
        public static ItemInfo WoodenAxe;
        public static ItemInfo StoneSword;
        public static ItemInfo StoneShovel;
        public static ItemInfo StonePickaxe;
        public static ItemInfo StoneAxe;
        public static ItemInfo DiamondSword;
        public static ItemInfo DiamondShovel;
        public static ItemInfo DiamondPickaxe;
        public static ItemInfo DiamondAxe;
        public static ItemInfo Stick;
        public static ItemInfo Bowl;
        public static ItemInfo MushroomSoup;
        public static ItemInfo GoldSword;
        public static ItemInfo GoldShovel;
        public static ItemInfo GoldPickaxe;
        public static ItemInfo GoldAxe;
        public static ItemInfo String;
        public static ItemInfo Feather;
        public static ItemInfo Gunpowder;
        public static ItemInfo WoodenHoe;
        public static ItemInfo StoneHoe;
        public static ItemInfo IronHoe;
        public static ItemInfo DiamondHoe;
        public static ItemInfo GoldHoe;
        public static ItemInfo Seeds;
        public static ItemInfo Wheat;
        public static ItemInfo Bread;
        public static ItemInfo LeatherCap;
        public static ItemInfo LeatherTunic;
        public static ItemInfo LeatherPants;
        public static ItemInfo LeatherBoots;
        public static ItemInfo ChainHelmet;
        public static ItemInfo ChainChestplate;
        public static ItemInfo ChainLeggings;
        public static ItemInfo ChainBoots;
        public static ItemInfo IronHelmet;
        public static ItemInfo IronChestplate;
        public static ItemInfo IronLeggings;
        public static ItemInfo IronBoots;
        public static ItemInfo DiamondHelmet;
        public static ItemInfo DiamondChestplate;
        public static ItemInfo DiamondLeggings;
        public static ItemInfo DiamondBoots;
        public static ItemInfo GoldHelmet;
        public static ItemInfo GoldChestplate;
        public static ItemInfo GoldLeggings;
        public static ItemInfo GoldBoots;
        public static ItemInfo Flint;
        public static ItemInfo RawPorkchop;
        public static ItemInfo CookedPorkchop;
        public static ItemInfo Painting;
        public static ItemInfo GoldenApple;
        public static ItemInfo Sign;
        public static ItemInfo WoodenDoor;
        public static ItemInfo Bucket;
        public static ItemInfo WaterBucket;
        public static ItemInfo LavaBucket;
        public static ItemInfo Minecart;
        public static ItemInfo Saddle;
        public static ItemInfo IronDoor;
        public static ItemInfo RedstoneDust;
        public static ItemInfo Snowball;
        public static ItemInfo Boat;
        public static ItemInfo Leather;
        public static ItemInfo Milk;
        public static ItemInfo ClayBrick;
        public static ItemInfo Clay;
        public static ItemInfo SugarCane;
        public static ItemInfo Paper;
        public static ItemInfo Book;
        public static ItemInfo Slimeball;
        public static ItemInfo StorageMinecart;
        public static ItemInfo PoweredMinecart;
        public static ItemInfo Egg;
        public static ItemInfo Compass;
        public static ItemInfo FishingRod;
        public static ItemInfo Clock;
        public static ItemInfo GlowstoneDust;
        public static ItemInfo RawFish;
        public static ItemInfo CookedFish;
        public static ItemInfo Dye;
        public static ItemInfo Bone;
        public static ItemInfo Sugar;
        public static ItemInfo Cake;
        public static ItemInfo Bed;
        public static ItemInfo RedstoneRepeater;
        public static ItemInfo Cookie;
        public static ItemInfo Map;
        public static ItemInfo GoldMusicDisc;
        public static ItemInfo GreenMusicDisc;

        static ItemInfo ()
        {
            _itemTable = new Dictionary<int, ItemInfo>();

            ItemTable = new ItemCache<ItemInfo>(_itemTable);

            IronShovel = new ItemInfo(256, "Iron Shovel");
            IronPickaxe = new ItemInfo(257, "Iron Pickaxe");
            IronAxe = new ItemInfo(258, "Iron Axe");
            FlintAndSteel = new ItemInfo(259, "Flint and Steel");
            Apple = new ItemInfo(260, "Apple").SetStackSize(64);
            Bow = new ItemInfo(261, "Bow");
            Arrow = new ItemInfo(262, "Arrow").SetStackSize(64);
            Coal = new ItemInfo(263, "Coal").SetStackSize(64);
            Diamond = new ItemInfo(264, "Diamond").SetStackSize(64);
            IronIngot = new ItemInfo(265, "Iron Ingot").SetStackSize(64);
            GoldIngot = new ItemInfo(266, "Gold Ingot").SetStackSize(64);
            IronSword = new ItemInfo(267, "Iron Sword");
            WoodenSword = new ItemInfo(268, "Wooden Sword");
            WoodenShovel = new ItemInfo(269, "Wooden Shovel");
            WoodenPickaxe = new ItemInfo(270, "Wooden Pickaxe");
            WoodenAxe = new ItemInfo(271, "Wooden Axe");
            StoneSword = new ItemInfo(272, "Stone Sword");
            StoneShovel = new ItemInfo(273, "Stone Shovel");
            StonePickaxe = new ItemInfo(274, "Stone Pickaxe");
            StoneAxe = new ItemInfo(275, "Stone Axe");
            DiamondSword = new ItemInfo(276, "Diamond Sword");
            DiamondShovel = new ItemInfo(277, "Diamond Shovel");
            DiamondPickaxe = new ItemInfo(278, "Diamond Pickaxe");
            DiamondAxe = new ItemInfo(279, "Diamond Axe");
            Stick = new ItemInfo(280, "Stick").SetStackSize(64);
            Bowl = new ItemInfo(281, "Bowl").SetStackSize(64);
            MushroomSoup = new ItemInfo(282, "Mushroom Soup");
            GoldSword = new ItemInfo(283, "Gold Sword");
            GoldShovel = new ItemInfo(284, "Gold Shovel");
            GoldPickaxe = new ItemInfo(285, "Gold Pickaxe");
            GoldAxe = new ItemInfo(286, "Gold Axe");
            String = new ItemInfo(287, "String").SetStackSize(64);
            Feather = new ItemInfo(288, "Feather").SetStackSize(64);
            Gunpowder = new ItemInfo(289, "Gunpowder").SetStackSize(64);
            WoodenHoe = new ItemInfo(290, "Wooden Hoe");
            StoneHoe = new ItemInfo(291, "Stone Hoe");
            IronHoe = new ItemInfo(292, "Iron Hoe");
            DiamondHoe = new ItemInfo(293, "Diamond Hoe");
            GoldHoe = new ItemInfo(294, "Gold Hoe");
            Seeds = new ItemInfo(295, "Seeds").SetStackSize(64);
            Wheat = new ItemInfo(296, "Wheat").SetStackSize(64);
            Bread = new ItemInfo(297, "Bread");
            LeatherCap = new ItemInfo(298, "Leather Cap");
            LeatherTunic = new ItemInfo(299, "Leather Tunic");
            LeatherPants = new ItemInfo(300, "Leather Pants");
            LeatherBoots = new ItemInfo(301, "Leather Boots");
            ChainHelmet = new ItemInfo(302, "Chain Helmet");
            ChainChestplate = new ItemInfo(303, "Chain Chestplate");
            ChainLeggings = new ItemInfo(304, "Chain Leggings");
            ChainBoots = new ItemInfo(305, "Chain Boots");
            IronHelmet = new ItemInfo(306, "Iron Helmet");
            IronChestplate = new ItemInfo(307, "Iron Chestplate");
            IronLeggings = new ItemInfo(308, "Iron Leggings");
            IronBoots = new ItemInfo(309, "Iron Boots");
            DiamondHelmet = new ItemInfo(310, "Diamond Helmet");
            DiamondChestplate = new ItemInfo(311, "Diamond Chestplate");
            DiamondLeggings = new ItemInfo(312, "Diamond Leggings");
            DiamondBoots = new ItemInfo(313, "Diamond Boots");
            GoldHelmet = new ItemInfo(314, "Gold Helmet");
            GoldChestplate = new ItemInfo(315, "Gold Chestplate");
            GoldLeggings = new ItemInfo(316, "Gold Leggings");
            GoldBoots = new ItemInfo(317, "Gold Boots");
            Flint = new ItemInfo(318, "Flint").SetStackSize(64);
            RawPorkchop = new ItemInfo(319, "Raw Porkchop");
            CookedPorkchop = new ItemInfo(320, "Cooked Porkchop");
            Painting = new ItemInfo(321, "Painting").SetStackSize(64);
            GoldenApple = new ItemInfo(322, "Golden Apple");
            Sign = new ItemInfo(323, "Sign");
            WoodenDoor = new ItemInfo(324, "Door");
            Bucket = new ItemInfo(325, "Bucket");
            WaterBucket = new ItemInfo(326, "Water Bucket");
            LavaBucket = new ItemInfo(327, "Lava Bucket");
            Minecart = new ItemInfo(328, "Minecart");
            Saddle = new ItemInfo(329, "Saddle");
            IronDoor = new ItemInfo(330, "Iron Door");
            RedstoneDust = new ItemInfo(331, "Redstone Dust").SetStackSize(64);
            Snowball = new ItemInfo(332, "Snowball").SetStackSize(16);
            Boat = new ItemInfo(333, "Boat");
            Leather = new ItemInfo(334, "Leather").SetStackSize(64);
            Milk = new ItemInfo(335, "Milk");
            ClayBrick = new ItemInfo(336, "Clay Brick").SetStackSize(64);
            Clay = new ItemInfo(337, "Clay").SetStackSize(64);
            SugarCane = new ItemInfo(338, "Sugar Cane").SetStackSize(64);
            Paper = new ItemInfo(339, "Paper").SetStackSize(64);
            Book = new ItemInfo(340, "Book").SetStackSize(64);
            Slimeball = new ItemInfo(341, "Slimeball").SetStackSize(64);
            StorageMinecart = new ItemInfo(342, "Storage Miencart");
            PoweredMinecart = new ItemInfo(343, "Powered Minecart");
            Egg = new ItemInfo(344, "Egg").SetStackSize(16);
            Compass = new ItemInfo(345, "Compass");
            FishingRod = new ItemInfo(346, "Fishing Rod");
            Clock = new ItemInfo(347, "Clock");
            GlowstoneDust = new ItemInfo(348, "Glowstone Dust").SetStackSize(64);
            RawFish = new ItemInfo(349, "Raw Fish");
            CookedFish = new ItemInfo(350, "Cooked Fish");
            Dye = new ItemInfo(351, "Dye").SetStackSize(64);
            Bone = new ItemInfo(352, "Bone").SetStackSize(64);
            Sugar = new ItemInfo(353, "Sugar").SetStackSize(64);
            Cake = new ItemInfo(354, "Cake");
            Bed = new ItemInfo(355, "Bed");
            RedstoneRepeater = new ItemInfo(356, "Redstone Repeater").SetStackSize(64);
            Cookie = new ItemInfo(357, "Cookie").SetStackSize(8);
            Map = new ItemInfo(358, "Map");
            GoldMusicDisc = new ItemInfo(2256, "Gold Music Disc");
            GreenMusicDisc = new ItemInfo(2257, "Green Music Disc");
        }
    }
}
