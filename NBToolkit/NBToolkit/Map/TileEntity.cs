using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map
{
    using NBT;
    using Utility;

    public class TileEntity : ICopyable<TileEntity>
    {
        protected NBT_Compound _tree;

        public NBT_Compound Root
        {
            get { return _tree; }
        }

        public string ID
        {
            get { return _tree["id"].ToNBTString(); }
        }

        public int X
        {
            get { return _tree["x"].ToNBTInt(); }
            set { _tree["x"] = new NBT_Int(value); }
        }

        public int Y
        {
            get { return _tree["y"].ToNBTInt(); }
            set { _tree["y"] = new NBT_Int(value); }
        }

        public int Z
        {
            get { return _tree["z"].ToNBTInt(); }
            set { _tree["z"] = new NBT_Int(value); }
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

        public TileEntity (NBTSchemaNode schema)
        {
            _tree = schema.BuildDefaultTree() as NBT_Compound;
        }

        public virtual bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, BaseSchema);
            return v.Verify();
        }

        public bool Verify (NBTSchemaNode schema)
        {
            NBTVerifier v = new NBTVerifier(Root, schema);
            return v.Verify();
        }

        public bool LocatedAt (int x, int y, int z)
        {
            return _tree["x"].ToNBTInt().Data == x &&
                _tree["y"].ToNBTInt().Data == y &&
                _tree["z"].ToNBTInt().Data == z;
        }

        #region ICopyable<TileEntity> Members

        public virtual TileEntity Copy ()
        {
            return new TileEntity(_tree.Copy() as NBT_Compound);
        }

        #endregion

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

        public static readonly NBTCompoundNode MobSpawnerSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "MobSpawner"),
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
    }

    public class TileEntityFurnace : TileEntity, IItemContainer
    {
        protected const int _capacity = 3;

        protected ItemCollection _items;

        public int BurnTime
        {
            get { return _tree["BurnTime"].ToNBTShort(); }
            set { _tree["BurnTime"] = new NBT_Short((short)value); }
        }

        public int CookTime
        {
            get { return _tree["CookTime"].ToNBTShort(); }
            set { _tree["CookTime"] = new NBT_Short((short)value); }
        }

        public TileEntityFurnace (NBT_Compound tree)
            : base(tree)
        {
            NBT_List items = tree["Items"].ToNBTList();

            if (items.Count == 0) {
                tree["Items"] = new NBT_List(NBT_Type.TAG_COMPOUND);
                items = _tree["Items"].ToNBTList();
            }

            _items = new ItemCollection(items, _capacity);
        }

        public override bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, TileEntity.FurnaceSchema);
            return v.Verify();
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityFurnace(_tree.Copy() as NBT_Compound);
        }

        #endregion

        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _items; }
        }

        #endregion
    }

    public class TileEntitySign : TileEntity
    {
        public string Text1
        {
            get { return _tree["Text1"].ToNBTString(); }
            set { _tree["Text1"] = new NBT_String(value.Substring(0, 12)); }
        }

        public string Text2
        {
            get { return _tree["Text2"].ToNBTString(); }
            set { _tree["Text2"] = new NBT_String(value.Substring(0, 12)); }
        }

        public string Text3
        {
            get { return _tree["Text3"].ToNBTString(); }
            set { _tree["Text3"] = new NBT_String(value.Substring(0, 12)); }
        }

        public string Text4
        {
            get { return _tree["Text4"].ToNBTString(); }
            set { _tree["Text4"] = new NBT_String(value.Substring(0, 12)); }
        }

        public TileEntitySign (NBT_Compound tree)
            : base(tree)
        {
        }

        public override bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, TileEntity.SignSchema);
            return v.Verify();
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntitySign(_tree.Copy() as NBT_Compound);
        }

        #endregion
    }

    public class TileEntityMobSpawner : TileEntity
    {
        public int Delay
        {
            get { return _tree["Delay"].ToNBTShort(); }
            set { _tree["Delay"] = new NBT_Short((short)value); }
        }

        public string EntityID
        {
            get { return _tree["EntityID"].ToNBTString(); }
            set { _tree["EntityID"] = new NBT_String(value); }
        }

        public TileEntityMobSpawner (NBT_Compound tree)
            : base(tree)
        {
        }

        public override bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, TileEntity.MobSpawnerSchema);
            return v.Verify();
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityMobSpawner(_tree.Copy() as NBT_Compound);
        }

        #endregion
    }

    public class TileEntityChest : TileEntity, IItemContainer
    {
        protected const int _capacity = 27;

        protected ItemCollection _items;

        public TileEntityChest (NBT_Compound tree)
            : base(tree)
        {
            NBT_List items = tree["Items"].ToNBTList();

            if (items.Count == 0) {
                tree["Items"] = new NBT_List(NBT_Type.TAG_COMPOUND);
                items = _tree["Items"].ToNBTList();
            }

            _items = new ItemCollection(items, _capacity);
        }

        public override bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, TileEntity.ChestSchema);
            return v.Verify();
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityChest(_tree.Copy() as NBT_Compound);
        }

        #endregion

        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _items; }
        }

        #endregion
    }

    public class TileEntityMusic : TileEntity
    {
        public int Note
        {
            get { return _tree["Note"].ToNBTByte(); }
            set { _tree["Note"] = new NBT_Byte((byte)value); }
        }

        public TileEntityMusic (NBT_Compound tree)
            : base(tree)
        {
        }

        public override bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, TileEntity.MusicSchema);
            return v.Verify();
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityMusic(_tree.Copy() as NBT_Compound);
        }

        #endregion
    }

    public class TileEntityTrap : TileEntity, IItemContainer
    {
        protected const int _capacity = 8;

        protected ItemCollection _items;

        public TileEntityTrap (NBT_Compound tree)
            : base(tree)
        {
            NBT_List items = tree["Items"].ToNBTList();

            if (items.Count == 0) {
                tree["Items"] = new NBT_List(NBT_Type.TAG_COMPOUND);
                items = _tree["Items"].ToNBTList();
            }

            _items = new ItemCollection(items, _capacity);
        }

        public override bool Verify ()
        {
            NBTVerifier v = new NBTVerifier(Root, TileEntity.TrapSchema);
            return v.Verify();
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityTrap(_tree.Copy() as NBT_Compound);
        }

        #endregion

        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _items; }
        }

        #endregion
    }
}
