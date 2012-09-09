using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{

    // Block Data

    public enum WoodType
    {
        OAK = 0,
        SPRUCE = 1,
        BIRCH = 2,
    }

    public enum LeafType
    {
        OAK = 0,
        SPRUCE = 1,
        BIRCH = 2,
        DARK_OAK = 3,
    }

    [Flags]
    public enum LeafState
    {
        PERMANENT = 0x4,
        DECAY_CHECK = 0x8,
    }

    public enum SaplingType
    {
        OAK = 0,
        SPRUCE = 1,
        BIRCH = 2,
    }

    public enum WaterFlow
    {
        FULL = 0,
        FLOW_1 = 1,
        FLOW_2 = 2,
        FLOW_3 = 3,
        FLOW_4 = 4,
        FLOW_5 = 5,
        FLOW_6 = 6,
        FLOW_7 = 7,
    }

    public enum LavaFlow
    {
        FULL = 0,
        FLOW_1 = 2,
        FLOW_2 = 4,
        FLOW_3 = 6,
    }

    [Flags]
    public enum LiquidState
    {
        FALLING = 0x08,
    }


    public enum DoorHinge
    {
        NORTHEAST = 0,
        SOUTHEAST = 1,
        SOUTHWEST = 2,
        NORTHWEST = 3,
    }

    [Flags]
    public enum DoorState
    {
        SWUNG = 0x04,
        TOPHALF = 0x08,
    }

    public enum WoolColor
    {
        WHITE = 0,
        ORANGE = 1,
        MAGENTA = 2,
        LIGHT_BLUE = 3,
        YELLOW = 4,
        LIGHT_GREEN = 5,
        PINK = 6,
        GRAY = 7,
        LIGHT_GRAY = 8,
        CYAN = 9,
        PURPLE = 10,
        BLUE = 11,
        BROWN = 12,
        DARK_GREEN = 13,
        RED = 14,
        BLACK = 15
    }

    public enum TorchOrientation
    {
        SOUTH = 1,
        NORTH = 2,
        WEST = 3,
        EAST = 4,
        FLOOR = 5,
    }

    public enum RailOrientation
    {
        EASTWEST = 0,
        NORTHSOUTH = 1,
        ASCEND_SOUTH = 2,
        ASCEND_NORTH = 3,
        ASCEND_EAST = 4,
        ASCEND_WEST = 5,
        NORTHEAST = 6,
        SOUTHEAST = 7,
        SOUTHWEST = 8,
        NORTHWEST = 9
    }

    public enum PoweredRailOrientation
    {
        EAST_WEST = 0,
        NORTH_SOUTH = 1,
        ASCEND_SOUTH = 2,
        ASCEND_NORTH = 3,
        ASCEND_EAST = 4,
        ASCEND_WEST = 5,
    }

    [Flags]
    public enum PoweredRailState
    {
        POWERED = 0x08,
    }

    public enum LadderOrientation
    {
        EAST = 2,
        WEST = 3,
        NORTH = 4,
        SOUTH = 5,
    }

    public enum StairOrientation
    {
        ASCEND_SOUTH = 0,
        ASCEND_NORTH = 1,
        ASCEND_WEST = 2,
        ASCEND_EAST = 3,
    }

    [Flags]
    public enum StairInversion
    {
        Inverted = 0x04,
    }

    public enum LeverOrientation
    {
        SOUTH = 1,
        NORTH = 2,
        WEST = 3,
        EAST = 4,
        GROUND_EASTWEST = 5,
        GROUND_NORTHSOUTH = 6,
    }

    [Flags]
    public enum LeverState
    {
        POWERED = 0x08,
    }

    public enum ButtonOrientation
    {
        WEST = 1,
        EAST = 2,
        SOUTH = 3,
        NORTH = 4,
    }

    [Flags]
    public enum ButtonState
    {
        PRESSED = 0x08,
    }

    public enum SignPostOrientation
    {
        WEST = 0,
        WEST_NORTHWEST = 1,
        NORTHWEST = 2,
        NORTH_NORTHWEST = 3,
        NORTH = 4,
        NORTH_NORTHEAST = 5,
        NORTHEAST = 6,
        EAST_NORTHEAST = 7,
        EAST = 8,
        EAST_SOUTHEAST = 9,
        SOUTHEAST = 10,
        SOUTH_SOUTHEAST = 11,
        SOUTH = 12,
        SOUTH_SOUTHWEST = 13,
        SOUTHWEST = 14,
        WEST_SOUTHWEST = 15,
    }

    public enum WallSignOrientation
    {
        EAST = 2,
        WEST = 3,
        NORTH = 4,
        SOUTH = 5,
    }

    public enum FurnaceOrientation
    {
        EAST = 2,
        WEST = 3,
        NORTH = 4,
        SOUTH = 5,
    }

    public enum PumpkinOrientation
    {
        EAST = 0,
        SOUTH = 1,
        WEST = 2,
        NORTH = 3,
    }

    [Flags]
    public enum PressurePlateState
    {
        PRESSED = 0x01,
    }

    public enum SlabType
    {
        STONE = 0,
        SANDSTONE = 1,
        WOOD = 2,
        COBBLESTONE = 3,
        BRICK = 4,
        STONE_BRICK = 5,
    }

    [Flags]
    public enum SlabInversion
    {
        Inverted = 0x08,
    }

    public enum BedOrientation
    {
        WEST = 0,
        NORTH = 1,
        EAST = 2,
        SOUTH = 3,
    }

    public enum CakeState
    {
        PIECES_6 = 0,
        PIECES_5 = 1,
        PIECES_4 = 2,
        PIECES_3 = 3,
        PIECES_2 = 4,
        PIECES_1 = 5,
    }

    [Flags]
    public enum BedState
    {
        HEAD = 0x08,
    }

    public enum RepeaterOrientation
    {
        EAST = 0,
        SOUTH = 1,
        WEST = 2,
        NORTH = 3,
    }

    public enum RepeaterDelay
    {
        DELAY_1 = 0,
        DELAY_2 = 4,
        DELAY_3 = 8,
        DELAY_4 = 12,
    }

    public enum TallGrassType
    {
        DEAD_SHRUB = 0,
        TALL_GRASS = 1,
        FERN = 2,
    }

    public enum TrapdoorOrientation
    {
        WEST = 0,
        EAST = 1,
        SOUTH = 2,
        NORTH = 3,
    }

    [Flags]
    public enum TrapdoorState
    {
        SWUNG = 0x04,
    }

    public enum PistonOrientation
    {
        DOWN = 0,
        UP = 1,
        EAST = 2,
        WEST = 3,
        NORTH = 4,
        SOUTH = 5,
    }

    [Flags]
    public enum PistonBodyState
    {
        EXTENDED = 0x08,
    }

    [Flags]
    public enum PistonHeadState
    {
        STICKY = 0x08,
    }

    public enum StoneBrickType
    {
        NORMAL = 0,
        MOSSY = 1,
        CRACKED = 2,
    }

    public enum HugeMushroomType
    {
        FLESHY = 0,
        CAP_CORNER_NORTHEAST = 1,
        CAP_SIDE_EAST = 2,
        CAP_CORNER_SOUTHEAST = 3,
        CAP_SIDE_NORTH = 4,
        CAP_TOP = 5,
        CAP_SIDE_SOUTH = 6,
        CAP_CORNER_NORTHWEST = 7,
        CAP_SIDE_WEST = 8,
        CAP_CORNER_SOUTHWEST = 9,
        STEM = 10,
    }

    [Flags]
    public enum VineCoverageState
    {
        TOP = 0x0,
        WEST = 0x1,
        NORTH = 0x2,
        EAST = 0x4,
        SOUTH = 0x8,
    }

    public enum FenceGateOrientation
    {
        WEST = 0,
        NORTH = 1,
        EAST = 2,
        SOUTH = 3,
    }

    [Flags]
    public enum FenceGateState
    {
        OPEN = 0x4,
    }

    public enum SilverfishBlockType
    {
        STONE = 0,
        COBBLESTONE = 1,
        STONE_BRICK = 2,
    }

    [Flags]
    public enum BrewingStandState
    {
        NONE = 0x0,
        SLOT_EAST = 0x1,
        SLOT_SOUTHWEST = 0x2,
        SLOT_NORTHWEST = 0x4,
    }

    public enum CauldronLevel
    {
        EMPTY = 0,
        ONE_THIRD = 1,
        TWO_THIRDS = 2,
        FULL = 3,
    }

    [Flags]
    public enum EndPortalState
    {
        EYE_OF_ENDER = 0x4,
    }

    public enum CocoaPlantSize
    {
        SMALL = 0x0,
        MEDIUM = 0x4,
        LARGE = 0x8,
    }

    public enum CocoaPlantDirection
    {
        NORTH = 0,
        EAST = 1,
        SOUTH = 2,
        WEST = 3,
    }

    public enum TripwireHookDirection
    {
        SOUTH = 0,
        WEST = 1,
        NORTH = 2,
        EAST = 3,
    }

    [Flags]
    public enum TripwireHookState
    {
        READY = 0x04,
        ACTIVATED = 0x08,
    }

    [Flags]
    public enum TripwireState
    {
        UNDER_OBJECT = 0x01,
        ACTIVATED = 0x04,
    }

    public enum CobblestoneWallType
    {
        COBBLESTONE = 0,
        MOSS_STONE = 1,
    }

    public enum FlowerPotType
    {
        EMPTY = 0,
        ROSE = 1,
        DANDELION = 2,
        OAK_SAPLING = 3,
        SPRUCE_SAPLING = 4,
        BIRCH_SAPLING = 5,
        JUNGLE_SAPLING = 6,
        RED_MUSHROOM = 7,
        BROWN_MUSHROOM = 8,
        CACTUS = 9,
        DEAD_BUSH = 10,
        FERN = 11,
    }

    public enum HeadType
    {
        SKELETON = 0,
        WITHER_SKELETON = 1,
        ZOMBIE = 2,
        HUMAN = 3,
        CREEPER = 4,
    }

    // Item Data

    public enum CoalType
    {
        COAL = 0,
        CHARCOAL = 1
    }

    public enum DyeType
    {
        INK_SAC = 0,
        ROSE_RED = 1,
        CACTUS_GREEN = 2,
        COCOA_BEANS = 3,
        LAPIS_LAZULI = 4,
        PURPLE_DYE = 5,
        CYAN_DYE = 6,
        LIGHT_GRAY_DYE = 7,
        GRAY_DYE = 8,
        PINK_DYE = 9,
        LIME_DYE = 10,
        DANDELION_YELLOW = 11,
        LIGHT_BLUE_DYE = 12,
        MAGENTA_DYE = 13,
        ORANGE_DYE = 14,
        BONE_MEAL = 15
    }
}
