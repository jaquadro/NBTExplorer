using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map.NBT
{
    class JSONSerializer
    {
        public static string Serialize (NBT_Value tag)
        {
            StringBuilder str = new StringBuilder();

            if (tag.GetNBTType() == NBT_Type.TAG_COMPOUND) {
                SerializeCompound(tag as NBT_Compound, str, 0);
            }
            else if (tag.GetNBTType() == NBT_Type.TAG_LIST) {
                SerializeList(tag as NBT_List, str, 0);
            }
            else {
                SerializeScaler(tag, str);
            }

            str.AppendLine();

            return str.ToString();
        }

        private static void SerializeCompound (NBT_Compound tag, StringBuilder str, int level)
        {
            if (tag.Count == 0) {
                str.Append("{ }");
                return;
            }

            str.AppendLine();
            AddLine(str, "{", level);

            IEnumerator<KeyValuePair<string, NBT_Value>> en = tag.GetEnumerator();
            bool first = true;
            while (en.MoveNext()) {
                if (!first) {
                    str.Append(",");
                    str.AppendLine();
                }

                KeyValuePair<string, NBT_Value> item = en.Current;
                Add(str, "\"" + item.Key + "\": ", level + 1);

                if (item.Value.GetNBTType() == NBT_Type.TAG_COMPOUND) {
                    SerializeCompound(item.Value as NBT_Compound, str, level + 1);
                }
                else if (item.Value.GetNBTType() == NBT_Type.TAG_LIST) {
                    SerializeList(item.Value as NBT_List, str, level + 1);
                }
                else {
                    SerializeScaler(item.Value, str);
                }

                first = false;
            }

            str.AppendLine();
            Add(str, "}", level);
        }

        private static void SerializeList (NBT_List tag, StringBuilder str, int level)
        {
            if (tag.Count == 0) {
                str.Append("[ ]");
                return;
            }

            str.AppendLine();
            AddLine(str, "[", level);

            IEnumerator<NBT_Value> en = tag.GetEnumerator();
            bool first = true;
            while (en.MoveNext()) {
                if (!first) {
                    str.Append(",");
                }

                NBT_Value item = en.Current;

                if (item.GetNBTType() == NBT_Type.TAG_COMPOUND) {
                    SerializeCompound(item as NBT_Compound, str, level + 1);
                }
                else if (item.GetNBTType() == NBT_Type.TAG_LIST) {
                    SerializeList(item as NBT_List, str, level + 1);
                }
                else {
                    if (!first) {
                        str.AppendLine();
                    }
                    Indent(str, level + 1);
                    SerializeScaler(item, str);
                }

                
                first = false;
            }

            str.AppendLine();
            Add(str, "]", level);
        }

        private static void SerializeScaler (NBT_Value tag, StringBuilder str)
        {
            NBT_Type type = tag.GetNBTType();
            switch (tag.GetNBTType()) {
                case NBT_Type.TAG_STRING:
                    str.Append("\"" + tag.ToNBTString().Data + "\"");
                    break;

                case NBT_Type.TAG_BYTE:
                    str.Append(tag.ToNBTByte().Data);
                    break;

                case NBT_Type.TAG_SHORT:
                    str.Append(tag.ToNBTShort().Data);
                    break;

                case NBT_Type.TAG_INT:
                    str.Append(tag.ToNBTInt().Data);
                    break;

                case NBT_Type.TAG_LONG:
                    str.Append(tag.ToNBTLong().Data);
                    break;

                case NBT_Type.TAG_FLOAT:
                    str.Append(tag.ToNBTFloat().Data);
                    break;

                case NBT_Type.TAG_DOUBLE:
                    str.Append(tag.ToNBTDouble().Data);
                    break;

                case NBT_Type.TAG_BYTE_ARRAY:
                    str.Append("null");
                    break;
            }
        }

        private static void AddLine (StringBuilder str, string line, int level)
        {
            Indent(str, level);
            str.AppendLine(line);
        }

        private static void Add (StringBuilder str, string line, int level)
        {
            Indent(str, level);
            str.Append(line);
        }

        private static void Indent (StringBuilder str, int count)
        {
            for (int i = 0; i < count; i++) {
                str.Append("\t");
            }
        }
    }
}
