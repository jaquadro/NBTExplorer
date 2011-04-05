using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;
    using Utility;

    public class Entity
    {
        private NBT_Compound _tree;

        public NBT_Compound Root
        {
            get { return _tree; }
        }

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

        public static readonly NBTCompoundNode PigSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Saddle", NBT_Type.TAG_BYTE),
        });

        public static readonly NBTCompoundNode SheepSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Sheared", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Color", NBT_Type.TAG_BYTE),
        });

        public static readonly NBTCompoundNode Slimechema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Size", NBT_Type.TAG_INT),
        });

        public static readonly NBTCompoundNode WolfSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Owner", NBT_Type.TAG_STRING),
            new NBTScalerNode("Sitting", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Angry", NBT_Type.TAG_BYTE),
        });
    }

    public class MobEntity : Entity
    {

    }
}
