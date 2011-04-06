using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map
{
    using NBT;
    using Utility;

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

    public class EntityMob : Entity
    {
        public static readonly NBTCompoundNode MobSchema = BaseSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Mob"),
            new NBTScalerNode("AttackTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("DeathTime", NBT_Type.TAG_SHORT),
            new NBTScalerNode("Health", NBT_Type.TAG_SHORT),
            new NBTScalerNode("HurtTime", NBT_Type.TAG_SHORT),
        });

        private short _attackTime;
        private short _deathTime;
        private short _health;
        private short _hurtTime;

        public int AttackTime
        {
            get { return _attackTime; }
            set { _attackTime = (short)value; }
        }

        public int DeathTime
        {
            get { return _deathTime; }
            set { _deathTime = (short)value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = (short)value; }
        }

        public int HurtTime
        {
            get { return _hurtTime; }
            set { _hurtTime = (short)value; }
        }

        public EntityMob ()
            : base("Mob")
        {
        }

        public EntityMob (string id)
            : base(id)
        {
        }

        public EntityMob (Entity e)
            : base(e)
        {
            EntityMob e2 = e as EntityMob;
            if (e2 != null) {
                _attackTime = e2._attackTime;
                _deathTime = e2._deathTime;
                _health = e2._health;
                _hurtTime = e2._hurtTime;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _attackTime = ctree["AttackTime"].ToNBTShort();
            _deathTime = ctree["DeathTime"].ToNBTShort();
            _health = ctree["Health"].ToNBTShort();
            _hurtTime = ctree["HurtTime"].ToNBTShort();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["AttackTime"] = new NBT_Short(_attackTime);
            tree["DeathTime"] = new NBT_Short(_deathTime);
            tree["Health"] = new NBT_Short(_health);
            tree["HurtTime"] = new NBT_Short(_hurtTime);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MobSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityMob(this);
        }

        #endregion
    }

    public class EntityMonster : EntityMob
    {
        public static readonly NBTCompoundNode MonsterSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Monster"),
        });

        public EntityMonster ()
            : base("Monster")
        {
        }

        public EntityMonster (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, MonsterSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityMonster(this);
        }

        #endregion
    }

    public class EntityCreeper : EntityMob
    {
        public static readonly NBTCompoundNode CreeperSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Creeper"),
        });

        public EntityCreeper ()
            : base("Creeper")
        {
        }

        public EntityCreeper (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, CreeperSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityCreeper(this);
        }

        #endregion
    }

    public class EntitySkeleton : EntityMob
    {
        public static readonly NBTCompoundNode SkeletonSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Skeleton"),
        });

        public EntitySkeleton ()
            : base("Skeleton")
        {
        }

        public EntitySkeleton (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SkeletonSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySkeleton(this);
        }

        #endregion
    }

    public class EntitySpider : EntityMob
    {
        public static readonly NBTCompoundNode SpiderSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Spider"),
        });

        public EntitySpider ()
            : base("Spider")
        {
        }

        public EntitySpider (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SpiderSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySpider(this);
        }

        #endregion
    }

    public class EntityGiant : EntityMob
    {
        public static readonly NBTCompoundNode GiantSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Giant"),
        });

        public EntityGiant ()
            : base("Giant")
        {
        }

        public EntityGiant (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, GiantSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityGiant(this);
        }

        #endregion
    }

    public class EntityZombie : EntityMob
    {
        public static readonly NBTCompoundNode ZombieSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Zombie"),
        });

        public EntityZombie ()
            : base("Zombie")
        {
        }

        public EntityZombie (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ZombieSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityZombie(this);
        }

        #endregion
    }

    public class EntitySlime : EntityMob
    {
        public static readonly NBTCompoundNode SlimeSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Slime"),
            new NBTScalerNode("Size", NBT_Type.TAG_INT),
        });

        private int _size;

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public EntitySlime ()
            : base("Slime")
        {
        }

        public EntitySlime (Entity e)
            : base(e)
        {
            EntitySlime e2 = e as EntitySlime;
            if (e2 != null) {
                _size = e2._size;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _size = ctree["Size"].ToNBTInt();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Size"] = new NBT_Int(_size);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SlimeSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySlime(this);
        }

        #endregion
    }

    public class EntityPigZombie : EntityMob
    {
        public static readonly NBTCompoundNode PigZombieSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "PigZombie"),
        });

        public EntityPigZombie ()
            : base("PigZombie")
        {
        }

        public EntityPigZombie (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, PigZombieSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityPigZombie(this);
        }

        #endregion
    }

    public class EntityGhast : EntityMob
    {
        public static readonly NBTCompoundNode GhastSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Ghast"),
        });

        public EntityGhast ()
            : base("Ghast")
        {
        }

        public EntityGhast (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, GhastSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityGhast(this);
        }

        #endregion
    }

    public class EntityPig : EntityMob
    {
        public static readonly NBTCompoundNode PigSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Pig"),
            new NBTScalerNode("Saddle", NBT_Type.TAG_BYTE),
        });

        private bool _saddle;

        public bool HasSaddle
        {
            get { return _saddle; }
            set { _saddle = value; }
        }

        public EntityPig ()
            : base("Pig")
        {
        }

        public EntityPig (Entity e)
            : base(e)
        {
            EntityPig e2 = e as EntityPig;
            if (e2 != null) {
                _saddle = e2._saddle;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _saddle = ctree["Saddle"].ToNBTByte() == 1;

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Saddle"] = new NBT_Byte((byte)(_saddle ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, PigSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityPig(this);
        }

        #endregion
    }

    public class EntitySheep : EntityMob
    {
        public static readonly NBTCompoundNode SheepSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Pig"),
            new NBTScalerNode("Sheared", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Color", NBT_Type.TAG_BYTE),
        });

        private bool _sheared;
        private byte _color;

        public bool IsSheared
        {
            get { return _sheared; }
            set { _sheared = value; }
        }

        public int Color
        {
            get { return _color; }
            set { _color = (byte)value; }
        }

        public EntitySheep ()
            : base("Sheep")
        {
        }

        public EntitySheep (Entity e)
            : base(e)
        {
            EntitySheep e2 = e as EntitySheep;
            if (e2 != null) {
                _sheared = e2._sheared;
                _color = e2._color;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _sheared = ctree["Sheared"].ToNBTByte() == 1;
            _color = ctree["Color"].ToNBTByte();

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Sheared"] = new NBT_Byte((byte)(_sheared ? 1 : 0));
            tree["Color"] = new NBT_Byte(_color);

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, SheepSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntitySheep(this);
        }

        #endregion
    }

    public class EntityCow : EntityMob
    {
        public static readonly NBTCompoundNode CowSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Cow"),
        });

        public EntityCow ()
            : base("Cow")
        {
        }

        public EntityCow (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, CowSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityCow(this);
        }

        #endregion
    }

    public class EntityChicken : EntityMob
    {
        public static readonly NBTCompoundNode ChickenSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Chicken"),
        });

        public EntityChicken ()
            : base("Chicken")
        {
        }

        public EntityChicken (Entity e)
            : base(e)
        {
        }


        #region INBTObject<Entity> Members

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, ChickenSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityChicken(this);
        }

        #endregion
    }

    public class EntityWolf : EntityMob
    {
        public static readonly NBTCompoundNode WolfSchema = MobSchema.MergeInto(new NBTCompoundNode("")
        {
            new NBTStringNode("id", "Wolf"),
            new NBTScalerNode("Owner", NBT_Type.TAG_STRING),
            new NBTScalerNode("Sitting", NBT_Type.TAG_BYTE),
            new NBTScalerNode("Angry", NBT_Type.TAG_BYTE),
        });

        private string _owner;
        private bool _sitting;
        private bool _angry;

        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public bool IsSitting
        {
            get { return _sitting; }
            set { _sitting = value; }
        }

        public bool IsAngry
        {
            get { return _angry; }
            set { _angry = value; }
        }

        public EntityWolf ()
            : base("Wolf")
        {
        }

        public EntityWolf (Entity e)
            : base(e)
        {
            EntityWolf e2 = e as EntityWolf;
            if (e2 != null) {
                _owner = e2._owner;
                _sitting = e2._sitting;
                _angry = e2._angry;
            }
        }


        #region INBTObject<Entity> Members

        public override Entity LoadTree (NBT_Value tree)
        {
            NBT_Compound ctree = tree as NBT_Compound;
            if (ctree == null || base.LoadTree(tree) == null) {
                return null;
            }

            _owner = ctree["Owner"].ToNBTString();
            _sitting = ctree["Sitting"].ToNBTByte() == 1;
            _angry = ctree["Angry"].ToNBTByte() == 1;

            return this;
        }

        public override NBT_Value BuildTree ()
        {
            NBT_Compound tree = base.BuildTree() as NBT_Compound;
            tree["Owner"] = new NBT_String(_owner);
            tree["Sitting"] = new NBT_Byte((byte)(_sitting ? 1 : 0));
            tree["Angry"] = new NBT_Byte((byte)(_angry ? 1 : 0));

            return tree;
        }

        public override bool ValidateTree (NBT_Value tree)
        {
            return new NBTVerifier(tree, WolfSchema).Verify();
        }

        #endregion


        #region ICopyable<Entity> Members

        public override Entity Copy ()
        {
            return new EntityPig(this);
        }

        #endregion
    }
}