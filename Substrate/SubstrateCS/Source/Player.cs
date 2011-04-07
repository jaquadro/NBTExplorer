using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public class Player : UntypedEntity, INBTObject<Player>, ICopyable<Player>
    {
        public static readonly NBTCompoundNode PlayerSchema = UTBaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTScalerNode("Dimension", NBT_Type.TAG_INT),
            new NBTListNode("Inventory", NBT_Type.TAG_COMPOUND, ItemCollection.InventorySchema),
            new NBTScalerNode("World", NBT_Type.TAG_STRING, NBTOptions.OPTIONAL),
            new NBTScalerNode("Sleeping", NBT_Type.TAG_BYTE, NBTOptions.CREATE_ON_MISSING),
            new NBTScalerNode("SleepTimer", NBT_Type.TAG_SHORT, NBTOptions.CREATE_ON_MISSING),
            new NBTScalerNode("SpawnX", NBT_Type.TAG_INT, NBTOptions.OPTIONAL),
            new NBTScalerNode("SpawnY", NBT_Type.TAG_INT, NBTOptions.OPTIONAL),
            new NBTScalerNode("SpawnZ", NBT_Type.TAG_INT, NBTOptions.OPTIONAL),
        });

        private int _dimension;
        private byte _sleeping;
        private short _sleepTimer;
        private int? _spawnX;
        private int? _spawnY;
        private int? _spawnZ;

        private string? _world;

        private ItemCollection _inventory;

        public Player ()
            : base()
        {
        }

        public Player (Player p)
            : base(p)
        {

        }


        #region INBTObject<Player> Members

        public virtual new Player LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            return this;
        }

        public virtual new Player LoadTreeSafe (NBT_Value tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual new NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;

            return tree;
        }

        public virtual new bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, PlayerSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public virtual new Player Copy ()
        {
            return new Player(this);
        }

        #endregion
    }
}