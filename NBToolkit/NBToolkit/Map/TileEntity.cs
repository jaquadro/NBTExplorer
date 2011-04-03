using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;
    using Utility;

    public class TileEntity : ICopyable<TileEntity>
    {
        private NBT_Compound _tree;

        public NBT_Compound Root
        {
            get { return _tree; }
        }

        public TileEntity (string id)
        {
            _tree = new NBT_Compound();
            _tree["id"] = new NBT_String(id);
            _tree["x"] = new NBT_Int();
            _tree["y"] = new NBT_Int();
            _tree["z"] = new NBT_Int();
        }

        public TileEntity (NBT_Compound tree)
        {
            _tree = tree;
        }

        public bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, BaseSchema);
            return v.Verify();
        }

        public bool Verify (NBTSchemaNode schema)
        {
            NBTVerifier v = new NBTVerifier(Root, schema);
            return v.Verify();
        }

        public bool LocatedAt (int lx, int ly, int lz)
        {
            return _tree["x"].ToNBTInt().Data == lx &&
                _tree["y"].ToNBTInt().Data == ly &&
                _tree["z"].ToNBTInt().Data == lz;
        }

        public bool Relocate (int lx, int ly, int lz)
        {
            if (lx >= 0 && lx < BlockManager.CHUNK_XLEN &&
                ly >= 0 && ly < BlockManager.CHUNK_YLEN &&
                lz >= 0 && lz < BlockManager.CHUNK_ZLEN) {
                _tree["x"].ToNBTInt().Data = lx;
                _tree["y"].ToNBTInt().Data = ly;
                _tree["z"].ToNBTInt().Data = lz;
                return true;
            }

            return false;
        }

        #region Predefined Schemas

        public static readonly NBTCompoundNode InventorySchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Damage", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Count", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Slot", NBT_Type.TAG_BYTE),
        };

        public static readonly NBTCompoundNode BaseSchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", NBT_Type.TAG_STRING),
            new NBTScalerNode("x", NBT_Type.TAG_INT),
            new NBTScalerNode("y", NBT_Type.TAG_INT),
            new NBTScalerNode("z", NBT_Type.TAG_INT),
        };

        public static readonly NBTCompoundNode FurnaceSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Furnace"),
            new NBTScalerNode("BurnTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("CookTime", NBT_Type.TAG_SHORT),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, InventorySchema),
        });

        public static readonly NBTCompoundNode SignSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Sign"),
            new NBTScalerNode("Text1", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text2", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text3", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text4", NBT_Type.TAG_STRING),
        });

        public static readonly NBTCompoundNode MonsterSpawnerSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "MonsterSpawner"),
            new NBTScalerNode("EntityId", NBT_Type.TAG_STRING),
            new NBTScalerNode("Delay", NBT_Type.TAG_SHORT),
        });

        public static readonly NBTCompoundNode ChestSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Chest"),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, InventorySchema),
        });

        public static readonly NBTCompoundNode MusicSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Music"),
            new NBTScalerNode("note", NBT_Type.TAG_BYTE),
        });

        public static readonly NBTCompoundNode TrapSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Trap"),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, InventorySchema),
        });

        #endregion

        #region ICopyable<TileEntity> Members

        public TileEntity Copy ()
        {
            return new TileEntity(_tree.Copy() as NBT_Compound);
        }

        #endregion
    }
}
