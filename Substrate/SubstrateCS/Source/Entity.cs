using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public interface IEntityContainer
    {
        List<Entity> FindEntities (string id);
        List<Entity> FindEntities (Predicate<Entity> match);

        bool AddEntity (Entity ent);

        int RemoveEntities (string id);
        int RemoveEntities (Predicate<Entity> match);
    }

    public class Entity : INBTObject<Entity>, ICopyable<Entity>
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

        private string _id;
        private Vector3 _pos;
        private Vector3 _motion;
        private Orientation _rotation;

        private float _fallDistance;
        private short _fire;
        private short _air;
        private byte _onGround;

        private string _world;

        public string ID
        {
            get { return _id; }
        }

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

        public Entity (string id)
        {
            _id = id;
        }

        public Entity (Entity e)
        {
            _id = e._id;

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
            _world = e._world;
        }


        #region INBTObject<Entity> Members

        public virtual Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null) {
                return null;
            }

            _id = ctree["id"].ToNBTString();

            NBT_List pos = ctree["Pos"].ToNBTList();
            _pos = new Vector3();
            _pos.X = pos[0].ToNBTDouble();
            _pos.Y = pos[1].ToNBTDouble();
            _pos.Z = pos[2].ToNBTDouble();

            NBT_List motion = ctree["Motion"].ToNBTList();
            _motion = new Vector3();
            _motion.X = motion[0].ToNBTDouble();
            _motion.Y = motion[1].ToNBTDouble();
            _motion.Z = motion[2].ToNBTDouble();

            NBT_List rotation = ctree["Rotation"].ToNBTList();
            _rotation = new Orientation();
            _rotation.Yaw = rotation[0].ToNBTFloat();
            _rotation.Pitch = rotation[1].ToNBTFloat();

            _fire = ctree["Fire"].ToNBTShort();
            _air = ctree["Air"].ToNBTShort();
            _onGround = ctree["OnGround"].ToNBTByte();

            NBT_Value world;
            ctree.TryGetValue("World", out world);

            if (world != null) {
                _world = world.ToNBTString();
            }

            return this;
        }

        public virtual Entity LoadTreeSafe (NBT_Value tree)
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

            NBT_List pos = new NBT_List(NBT_Type.TAG_DOUBLE);
            pos.Add(new NBT_Double(_pos.X));
            pos.Add(new NBT_Double(_pos.Y));
            pos.Add(new NBT_Double(_pos.Z));
            tree["Position"] = pos;

            NBT_List motion = new NBT_List(NBT_Type.TAG_DOUBLE);
            motion.Add(new NBT_Double(_motion.X));
            motion.Add(new NBT_Double(_motion.Y));
            motion.Add(new NBT_Double(_motion.Z));
            tree["Motion"] = motion;

            NBT_List rotation = new NBT_List(NBT_Type.TAG_FLOAT);
            rotation.Add(new NBT_Float((float)_rotation.Yaw));
            rotation.Add(new NBT_Float((float)_rotation.Pitch));
            tree["Rotation"] = rotation;

            tree["FallDistance"] = new NBT_Float(_fallDistance);
            tree["Fire"] = new NBT_Short(_fire);
            tree["Air"] = new NBT_Short(_air);
            tree["OnGround"] = new NBT_Byte(_onGround);

            if (_world != null) {
                tree["World"] = new NBT_String(_world);
            }

            return tree;
        }

        public virtual bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, BaseSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public virtual Entity Copy ()
        {
            return new Entity(this);
        }

        #endregion
    }

}