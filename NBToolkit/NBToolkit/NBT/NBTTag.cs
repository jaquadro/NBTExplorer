using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.NBT
{
    public class NBT_Tag
    {
        private NBT_String _name;
        private NBT_Value _value;

        public NBT_String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public NBT_Type Type
        {
            get { return (_value is NBT_Value) ? _value.GetNBTType() : NBT_Type.TAG_END; }
        }

        public NBT_Value Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public NBT_Tag () { }

        public NBT_Tag (NBT_String name)
        {
            _name = name;
        }

        public NBT_Tag (NBT_String name, NBT_Value value)
        {
            _name = name;
            _value = value;
        }

        public NBT_Tag FindTagByName (string name)
        {
            NBT_Compound cval = _value as NBT_Compound;
            if (cval == null) {
                throw new InvalidTagException();
            }

            return cval.FindTagByName(name);
        }

        public NBT_Tag AddTag (string name, NBT_Value value)
        {
            NBT_Compound cval = _value as NBT_Compound;
            if (cval == null) {
                throw new InvalidTagException();
            }

            NBT_Tag tag = new NBT_Tag(name, value);

            cval.AddTag(tag);
            return tag;
        }

        public NBT_Tag AddTag (NBT_Tag tag)
        {
            NBT_Compound cval = _value as NBT_Compound;
            if (cval == null) {
                throw new InvalidTagException();
            }

            tag.AddTag(tag);
            return tag;
        }

        public bool RemoveTag (string name)
        {
            NBT_Compound cval = _value as NBT_Compound;
            if (cval == null) {
                throw new InvalidTagException();
            }

            return cval.RemoveTag(name);
        }
    }
}
