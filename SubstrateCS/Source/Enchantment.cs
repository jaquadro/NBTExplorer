using System;
using System.Collections.Generic;
using Substrate.Nbt;
using Substrate.Core;

namespace Substrate
{
    /// <summary>
    /// Represents an enchantment that can be applied to some <see cref="Item"/>s.
    /// </summary>
    public class Enchantment : INbtObject<Enchantment>, ICopyable<Enchantment>
    {
        private static readonly SchemaNodeCompound _schema = new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("id", TagType.TAG_SHORT),
            new SchemaNodeScaler("lvl", TagType.TAG_SHORT),
        };

        private TagNodeCompound _source;

        private short _id;
        private short _level;

        /// <summary>
        /// Constructs a blank <see cref="Enchantment"/>.
        /// </summary>
        public Enchantment ()
        {
        }

        /// <summary>
        /// Constructs an <see cref="Enchantment"/> from a given id and level.
        /// </summary>
        /// <param name="id">The id (type) of the enchantment.</param>
        /// <param name="level">The level of the enchantment.</param>
        public Enchantment (int id, int level)
        {
            _id = (short)id;
            _level = (short)level;
        }

        #region Properties

        /// <summary>
        /// Gets an <see cref="EnchantmentInfo"/> entry for this enchantment's type.
        /// </summary>
        public EnchantmentInfo Info
        {
            get { return EnchantmentInfo.EnchantmentTable[_id]; }
        }

        /// <summary>
        /// Gets or sets the current type (id) of the enchantment.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = (short)value; }
        }

        /// <summary>
        /// Gets or sets the level of the enchantment.
        /// </summary>
        public int Level
        {
            get { return _level; }
            set { _level = (short)value; }
        }

        /// <summary>
        /// Gets a <see cref="SchemaNode"/> representing the schema of an enchantment.
        /// </summary>
        public static SchemaNodeCompound Schema
        {
            get { return _schema; }
        }

        #endregion

        #region INbtObject<Enchantment> Members

        /// <inheritdoc />
        public Enchantment LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToTagShort();
            _level = ctree["lvl"].ToTagShort();

            _source = ctree.Copy() as TagNodeCompound;

            return this;
        }

        /// <inheritdoc />
        public Enchantment LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        /// <inheritdoc />
        public TagNode BuildTree ()
        {
            TagNodeCompound tree = new TagNodeCompound();
            tree["id"] = new TagNodeShort(_id);
            tree["lvl"] = new TagNodeShort(_level);

            if (_source != null) {
                tree.MergeFrom(_source);
            }

            return tree;
        }

        /// <inheritdoc />
        public bool ValidateTree (TagNode tree)
        {
            return new NbtVerifier(tree, _schema).Verify();
        }

        #endregion

        #region ICopyable<Enchantment> Members

        /// <inheritdoc />
        public Enchantment Copy ()
        {
            Enchantment ench = new Enchantment(_id, _level);

            if (_source != null) {
                ench._source = _source.Copy() as TagNodeCompound;
            }

            return ench;
        }

        #endregion
    }
}
