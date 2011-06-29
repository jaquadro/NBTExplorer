using System;
using System.Collections.Generic;
using System.Text;
using Substrate.Core;
using Substrate.NBT;

namespace Substrate
{
    public interface IEntityContainer
    {
        List<Entity> FindEntities (string id);
        List<Entity> FindEntities (Predicate<Entity> match);

        bool AddEntity (Entity ent);

        int RemoveEntities (string id);
        int RemoveEntities (Predicate<Entity> match);
    }

    public class UntypedEntity : INBTObject<UntypedEntity>, ICopyable<UntypedEntity>
    {
        public class Vector3
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }

        public class Orientation
        {
            public double Pitch { get; set; }
            public double Yaw { get; set; }
        }

        public static readonly SchemaNodeCompound UTBaseSchema = new SchemaNodeCompound("")
        {
            new SchemaNodeList("Pos", TagType.TAG_DOUBLE, 3),
            new SchemaNodeList("Motion", TagType.TAG_DOUBLE, 3),
            new SchemaNodeList("Rotation", TagType.TAG_FLOAT, 2),
            new SchemaNodeScaler("FallDistance", TagType.TAG_FLOAT),
            new SchemaNodeScaler("Fire", TagType.TAG_SHORT),
            new SchemaNodeScaler("Air", TagType.TAG_SHORT),
            new SchemaNodeScaler("OnGround", TagType.TAG_BYTE),
        };

        private Vector3 _pos;
        private Vector3 _motion;
        private Orientation _rotation;

        private float _fallDistance;
        private short _fire;
        private short _air;
        private byte _onGround;

        public Vector3 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Vector3 Motion
        {
            get { return _motion; }
            set { _motion = value; }
        }

        public Orientation Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public double FallDistance
        {
            get { return _fallDistance; }
            set { _fallDistance = (float)value; }
        }

        public int Fire
        {
            get { return _fire; }
            set { _fire = (short)value; }
        }

        public int Air
        {
            get { return _air; }
            set { _air = (short)value; }
        }

        public bool IsOnGround
        {
            get { return _onGround == 1; }
            set { _onGround = (byte)(value ? 1 : 0); }
        }

        public UntypedEntity ()
        {
            _pos = new Vector3();
            _motion = new Vector3();
            _rotation = new Orientation();
        }

        public UntypedEntity (UntypedEntity e)
        {
            _pos = new Vector3();
            _pos.X = e._pos.X;
            _pos.Y = e._pos.Y;
            _pos.Z = e._pos.Z;

            _motion = new Vector3();
            _motion.X = e._motion.X;
            _motion.Y = e._motion.Y;
            _motion.Z = e._motion.Z;

            _rotation = new Orientation();
            _rotation.Pitch = e._rotation.Pitch;
            _rotation.Yaw = e._rotation.Yaw;

            _fallDistance = e._fallDistance;
            _fire = e._fire;
            _air = e._air;
            _onGround = e._onGround;
        }


        #region INBTObject<Entity> Members

        public UntypedEntity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null) {
                return null;
            }

            TagNodeList pos = ctree["Pos"].ToTagList();
            _pos = new Vector3();
            _pos.X = pos[0].ToTagDouble();
            _pos.Y = pos[1].ToTagDouble();
            _pos.Z = pos[2].ToTagDouble();

            TagNodeList motion = ctree["Motion"].ToTagList();
            _motion = new Vector3();
            _motion.X = motion[0].ToTagDouble();
            _motion.Y = motion[1].ToTagDouble();
            _motion.Z = motion[2].ToTagDouble();

            TagNodeList rotation = ctree["Rotation"].ToTagList();
            _rotation = new Orientation();
            _rotation.Yaw = rotation[0].ToTagFloat();
            _rotation.Pitch = rotation[1].ToTagFloat();

            _fire = ctree["Fire"].ToTagShort();
            _air = ctree["Air"].ToTagShort();
            _onGround = ctree["OnGround"].ToTagByte();

            return this;
        }

        public UntypedEntity LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public TagNode BuildTree ()
        {
            TagNodeCompound tree = new TagNodeCompound();

            TagNodeList pos = new TagNodeList(TagType.TAG_DOUBLE);
            pos.Add(new TagNodeDouble(_pos.X));
            pos.Add(new TagNodeDouble(_pos.Y));
            pos.Add(new TagNodeDouble(_pos.Z));
            tree["Pos"] = pos;

            TagNodeList motion = new TagNodeList(TagType.TAG_DOUBLE);
            motion.Add(new TagNodeDouble(_motion.X));
            motion.Add(new TagNodeDouble(_motion.Y));
            motion.Add(new TagNodeDouble(_motion.Z));
            tree["Motion"] = motion;

            TagNodeList rotation = new TagNodeList(TagType.TAG_FLOAT);
            rotation.Add(new TagNodeFloat((float)_rotation.Yaw));
            rotation.Add(new TagNodeFloat((float)_rotation.Pitch));
            tree["Rotation"] = rotation;

            tree["FallDistance"] = new TagNodeFloat(_fallDistance);
            tree["Fire"] = new TagNodeShort(_fire);
            tree["Air"] = new TagNodeShort(_air);
            tree["OnGround"] = new TagNodeByte(_onGround);

            return tree;
        }

        public bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, UTBaseSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public UntypedEntity Copy ()
        {
            return new UntypedEntity(this);
        }

        #endregion
    }

    public class Entity : UntypedEntity, INBTObject<Entity>, ICopyable<Entity>
    {
        public static readonly SchemaNodeCompound BaseSchema = UTBaseSchema.MergeInto(new SchemaNodeCompound("")
        {
            new SchemaNodeScaler("id", TagType.TAG_STRING),
        });

        private string _id;

        public string ID
        {
            get { return _id; }
        }

        public Entity (string id)
            : base()
        {
            _id = id;
        }

        public Entity (Entity e)
            : base(e)
        {
            _id = e._id;
        }


        #region INBTObject<Entity> Members

        public virtual new Entity LoadTree (TagNode tree)
        {
            TagNodeCompound ctree = tree as TagNodeCompound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _id = ctree["id"].ToTagString();

            return this;
        }

        public virtual new Entity LoadTreeSafe (TagNode tree)
        {
            if (!ValidateTree(tree)) {
                return null;
            }

            return LoadTree(tree);
        }

        public virtual new TagNode BuildTree ()
        {
            TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
            tree["id"] = new TagNodeString(_id);

            return tree;
        }

        public virtual new bool ValidateTree (TagNode tree)
        {
            return new NBTVerifier(tree, BaseSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public virtual new Entity Copy ()
        {
            return new Entity(this);
        }

        #endregion
    }

}