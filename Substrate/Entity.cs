using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;
    using Utility;

    public class Entity
    {
        private string _entityID;
        private double _posX;
        private double _posY;
        private double _posZ;
        private double _motionX;
        private double _motionY;
        private double _motionZ;
        private float _rotationYaw;
        private float _rotationPitch;
        private float _fallDistance;
        private short _fire;
        private short _air;
        private byte _onGround;


        #region Predefined Schemas

        public static readonly NBTCompoundNode BaseSchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", NBT_Type.TAG_STRING),
            new NBTListNode("Pos", NBT_Type.TAG_DOUBLE, 3),
            new NBTListNode("Motion", NBT_Type.TAG_DOUBLE, 3),
            new NBTListNode("Rotation", NBT_Type.TAG_FLOAT, 2),
            new NBTScalerNode("FallDistance", NBT_Type.TAG_FLOAT),
            new NBTScalerNode("Fire", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Air", NBT_Type.TAG_SHORT),
            new NBTScalerNode("OnGround", NBT_Type.TAG_BYTE),
        };

        public static readonly NBTCompoundNode MobSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Mob"),
            new NBTScalerNode("AttackTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("DeathTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Health", NBT_Type.TAG_SHORT),
            new NBTScalerNode("HurtTime", NBT_Type.TAG_SHORT),
        });

        public static readonly NBTCompoundNode MonsterSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Monster"),
        });

        public static readonly NBTCompoundNode CreeperSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Creeper"),
        });

        public static readonly NBTCompoundNode SkeletonSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Skeleton"),
        });

        public static readonly NBTCompoundNode SpiderSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Spider"),
        });

        public static readonly NBTCompoundNode GiantSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Giant"),
        });

        public static readonly NBTCompoundNode ZombieSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Zombie"),
        });

        public static readonly NBTCompoundNode PigZombieSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "PigZombie"),
        });

        public static readonly NBTCompoundNode GhastSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Ghast"),
        });

        public static readonly NBTCompoundNode PigSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Pig"),
            new NBTScalerNode("Saddle", NBT_Type.TAG_BYTE),
        });

        public static readonly NBTCompoundNode SheepSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Sheep"),
            new NBTScalerNode("Sheared", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Color", NBT_Type.TAG_BYTE),
        });

        public static readonly NBTCompoundNode CowSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Cow"),
        });

        public static readonly NBTCompoundNode ChickenSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Chicken"),
        });

        public static readonly NBTCompoundNode Slimechema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Slime"),
            new NBTScalerNode("Size", NBT_Type.TAG_INT),
        });

        public static readonly NBTCompoundNode WolfSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Wolf"),
            new NBTScalerNode("Owner", NBT_Type.TAG_STRING),
            new NBTScalerNode("Sitting", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Angry", NBT_Type.TAG_BYTE),
        });

        #endregion
    }

    public class MobEntity : Entity
    {

    }
}
