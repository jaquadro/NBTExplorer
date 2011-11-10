using System;
using System.Collections.Generic;
using Substrate.Nbt;
using System.IO;
using Substrate.Core;

namespace Substrate.Data
{
    public class BetaDataManager : DataManager, INbtObject<BetaDataManager>
    {
        private static SchemaNodeCompound _schema = new SchemaNodeCompound()
        {
            new SchemaNodeScaler("map", TagType.TAG_SHORT),
        };

        private TagNodeCompound _source;

        private NbtWorld _world;

        private short _mapId;

        private MapManager _maps;

        public BetaDataManager (NbtWorld world)
        {
            _world = world;

            _maps = new MapManager(_world);
        }

        public override int CurrentMapId
        {
            get { return _mapId; }
            set { _mapId = (short)value; }
        }

        public new MapManager Maps
        {
            get { return _maps; }
        }

        protected override IMapManager GetMapManager ()
        {
            return _maps;
        }

        public override bool Save ()
        {
            if (_world == null) {
                return false;
            }

            try {
                string path = Path.Combine(_world.Path, _world.DataDirectory);
                NBTFile nf = new NBTFile(Path.Combine(path, "idcounts.dat"));

                Stream zipstr = nf.GetDataOutputStream(CompressionType.None);
                if (zipstr == null) {
                    NbtIOException nex = new NbtIOException("Failed to initialize uncompressed NBT stream for output");
                    nex.Data["DataManager"] = this;
                    throw nex;
                }

                new NbtTree(BuildTree() as TagNodeCompound).WriteTo(zipstr);
                zipstr.Close();

                return true;
            }
            catch (Exception ex) {
                Exception lex = new Exception("Could not save idcounts.dat file.", ex);
                lex.Data["DataManager"] = this;
                throw lex;
            }
        }

        #region INBTObject<DataManager>

        public virtual BetaDataManager LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _mapId = ctree["map"].ToTagShort();

            _source = ctree.Copy() as TagNodeCompound;

            return this;
        }

        public virtual BetaDataManager LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual TagNode BuildTree ()
        {
            TagNodeCompound tree = new TagNodeCompound();

            tree["map"] = new TagNodeLong(_mapId);

            if (_source != null) {
                tree.MergeFrom(_source);
            }

            return tree;
        }

        public virtual bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, _schema).Verify();
        }

        #endregion
    }
}
