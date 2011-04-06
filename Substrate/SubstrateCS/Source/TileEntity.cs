using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map
{
    using NBT;
    using Utility;

    public class TileEntity : INBTObject<TileEntity>, ICopyable<TileEntity>
    {
        public static readonly NBTCompoundNode BaseSchema = new NBTCompoundNode("")
        {
            new NBTScalerNode("id", NBT_Type.TAG_STRING),
            new NBTScalerNode("x", NBT_Type.TAG_INT),
            new NBTScalerNode("y", NBT_Type.TAG_INT),
            new NBTScalerNode("z", NBT_Type.TAG_INT),
        };

        private string _id;
        private int _x;
        private int _y;
        private int _z;

        public string ID
        {
            get { return _id; }
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public TileEntity (string id)
        {
            _id = id;
        }

        public TileEntity (TileEntity te)
        {
            _id = te._id;
            _x = te._x;
            _y = te._y;
            _z = te._z;
        }

        public bool LocatedAt (int x, int y, int z)
        {
            return _x == x && _y == y && _z == z;
        }

        #region ICopyable<TileEntity> Members

        public virtual TileEntity Copy ()
        {
            return new TileEntity(this);
        }

        #endregion

        #region INBTObject<TileEntity> Members

        public virtual TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToNBTString();
            _x = ctree["x"].ToNBTInt();
            _y = ctree["y"].ToNBTInt();
            _z = ctree["z"].ToNBTInt();

            return this;
        }

        public virtual TileEntity LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual NBT_Value BuildTree ()
        {
            NBT_Compound tree = new NBT_Compound();
            tree["id"] = new NBT_String(_id);
            tree["x"] = new NBT_Int(_x);
            tree["y"] = new NBT_Int(_y);
            tree["z"] = new NBT_Int(_z);

            return tree;
        }

        public virtual bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, BaseSchema).Verify();
        }

        #endregion
    }

    public class TileEntityFurnace : TileEntity, IItemContainer
    {
        public static readonly NBTCompoundNode FurnaceSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Furnace"),
            new NBTScalerNode("BurnTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("CookTime", NBT_Type.TAG_SHORT),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, ItemCollection.ListSchema),
        });

        private const int _CAPACITY = 3;

        private short _burnTime;
        private short _cookTime;

        private ItemCollection _items;

        public int BurnTime
        {
            get { return _burnTime; }
            set { _burnTime = (short)value; }
        }

        public int CookTime
        {
            get { return _cookTime; }
            set { _cookTime = (short)value; }
        }

        public TileEntityFurnace ()
            : base("Furnace")
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public TileEntityFurnace (TileEntity te)
            : base (te)
        {
            TileEntityFurnace tec = te as TileEntityFurnace;
            if (tec != null) {
                _cookTime = tec._cookTime;
                _burnTime = tec._burnTime;
                _items = tec._items.Copy();
            }
            else {
                _items = new ItemCollection(_CAPACITY);
            }
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityFurnace(this);
        }

        #endregion

        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _items; }
        }

        #endregion

        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _burnTime = ctree["BurnTime"].ToNBTShort();
            _cookTime = ctree["CookTime"].ToNBTShort();

            NBT_List items = ctree["Items"].ToNBTList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["BurnTime"] = new NBT_Short(_burnTime);
            tree["CookTime"] = new NBT_Short(_cookTime);
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, FurnaceSchema).Verify();
        }

        #endregion
    }

    public class TileEntitySign : TileEntity
    {
        public static readonly NBTCompoundNode SignSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Sign"),
            new NBTScalerNode("Text1", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text2", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text3", NBT_Type.TAG_STRING),
            new NBTScalerNode("Text4", NBT_Type.TAG_STRING),
        });

        private string _text1 = "";
        private string _text2 = "";
        private string _text3 = "";
        private string _text4 = "";

        public string Text1
        {
            get { return _text1; }
            set { _text1 = value.Substring(0, 12); }
        }

        public string Text2
        {
            get { return _text2; }
            set { _text2 = value.Substring(0, 12); }
        }

        public string Text3
        {
            get { return _text3; }
            set { _text3 = value.Substring(0, 12); }
        }

        public string Text4
        {
            get { return _text4; }
            set { _text4 = value.Substring(0, 12); }
        }

        public TileEntitySign ()
            : base("Sign")
        {
        }

        public TileEntitySign (TileEntity te)
            : base(te)
        {
            TileEntitySign tes = te as TileEntitySign;
            if (tes != null) {
                _text1 = tes._text1;
                _text2 = tes._text2;
                _text3 = tes._text3;
                _text4 = tes._text4;
            }
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntitySign(this);
        }

        #endregion

        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _text1 = ctree["Text1"].ToNBTString();
            _text2 = ctree["Text2"].ToNBTString();
            _text3 = ctree["Text3"].ToNBTString();
            _text4 = ctree["Text4"].ToNBTString();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Text1"] = new NBT_String(_text1);
            tree["Text2"] = new NBT_String(_text2);
            tree["Text3"] = new NBT_String(_text3);
            tree["Text4"] = new NBT_String(_text4);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SignSchema).Verify();
        }

        #endregion
    }

    public class TileEntityMobSpawner : TileEntity
    {
        public static readonly NBTCompoundNode MobSpawnerSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "MobSpawner"),
            new NBTScalerNode("EntityId", NBT_Type.TAG_STRING),
            new NBTScalerNode("Delay", NBT_Type.TAG_SHORT),
        });

        private short _delay;
        private string _entityID;

        public int Delay
        {
            get { return _delay; }
            set { _delay = (short)value; }
        }

        public string EntityID
        {
            get { return _entityID; }
            set { _entityID = value; }
        }

        public TileEntityMobSpawner ()
            : base("MobSpawner")
        {
        }

        public TileEntityMobSpawner (TileEntity te)
            : base(te)
        {
            TileEntityMobSpawner tes = te as TileEntityMobSpawner;
            if (tes != null) {
                _delay = tes._delay;
                _entityID = tes._entityID;
            }
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityMobSpawner(this);
        }

        #endregion

        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _delay = ctree["Delay"].ToNBTShort();
            _entityID = ctree["EntityID"].ToNBTString();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["EntityID"] = new NBT_String(_entityID);
            tree["Delay"] = new NBT_Short(_delay);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MobSpawnerSchema).Verify();
        }

        #endregion
    }

    public class TileEntityChest : TileEntity, IItemContainer
    {
        public static readonly NBTCompoundNode ChestSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Chest"),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, ItemCollection.ListSchema),
        });

        private const int _CAPACITY = 27;

        private ItemCollection _items;

        public TileEntityChest ()
            : base("Chest")
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public TileEntityChest (TileEntity te)
            : base(te)
        {
            TileEntityChest tec = te as TileEntityChest;
            if (tec != null) {
                _items = tec._items.Copy();
            }
            else {
                _items = new ItemCollection(_CAPACITY);
            }
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityChest(this);
        }

        #endregion

        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _items; }
        }

        #endregion

        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            NBT_List items = ctree["Items"].ToNBTList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ChestSchema).Verify();
        }

        #endregion
    }

    public class TileEntityMusic : TileEntity
    {
        public static readonly NBTCompoundNode MusicSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Music"),
            new NBTScalerNode("note", NBT_Type.TAG_BYTE),
        });

        private byte _note;

        public int Note
        {
            get { return _note; }
            set { _note = (byte)value; }
        }

        public TileEntityMusic ()
            : base("Music")
        {
        }

        public TileEntityMusic (TileEntity te)
            : base(te)
        {
            TileEntityMusic tes = te as TileEntityMusic;
            if (tes != null) {
                _note = tes._note;
            }
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityMusic(this);
        }

        #endregion

        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _note = ctree["Note"].ToNBTByte();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Note"] = new NBT_Byte(_note);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MusicSchema).Verify();
        }

        #endregion
    }

    public class TileEntityTrap : TileEntity, IItemContainer
    {
        public static readonly NBTCompoundNode TrapSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Trap"),
            new NBTListNode("Items", NBT_Type.TAG_COMPOUND, ItemCollection.ListSchema),
        });

        private const int _CAPACITY = 8;

        private ItemCollection _items;

        public TileEntityTrap ()
            : base("Trap")
        {
            _items = new ItemCollection(_CAPACITY);
        }

        public TileEntityTrap (TileEntity te)
            : base(te)
        {
            TileEntityTrap tec = te as TileEntityTrap;
            if (tec != null) {
                _items = tec._items.Copy();
            }
            else {
                _items = new ItemCollection(_CAPACITY);
            }
        }

        #region ICopyable<TileEntity> Members

        public override TileEntity Copy ()
        {
            return new TileEntityTrap(this);
        }

        #endregion

        #region IItemContainer Members

        public ItemCollection Items
        {
            get { return _items; }
        }

        #endregion

        #region INBTObject<TileEntity> Members

        public override TileEntity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            NBT_List items = ctree["Items"].ToNBTList();
            _items = new ItemCollection(_CAPACITY).LoadTree(items);

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Items"] = _items.BuildTree();

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, TrapSchema).Verify();
        }

        #endregion
    }
}
