using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.NBT
{
    public class JSONSerializer
    {
        public static string Serialize (TagValue tag)
        {
            return Serialize(tag, 0);
        }

        public static string Serialize (TagValue tag, int level)
        {
            StringBuilder str = new StringBuilder();

            if (tag.GetTagType() == TagType.TAG_COMPOUND) {
                SerializeCompound(tag as TagCompound, str, level);
            }
            else if (tag.GetTagType() == TagType.TAG_LIST) {
                SerializeList(tag as TagList, str, level);
            }
            else {
                SerializeScaler(tag, str);
            }

            return str.ToString();
        }

        private static void SerializeCompound (TagCompound tag, StringBuilder str, int level)
        {
            if (tag.Count == 0) {
                str.Append("{ }");
                return;
            }

            str.AppendLine();
            AddLine(str, "{", level);

            IEnumerator<KeyValuePair<string, TagValue>> en = tag.GetEnumerator();
            bool first = true;
            while (en.MoveNext()) {
                if (!first) {
                    str.Append(",");
                    str.AppendLine();
                }

                KeyValuePair<string, TagValue> item = en.Current;
                Add(str, "\"" + item.Key + "\": ", level + 1);

                if (item.Value.GetTagType() == TagType.TAG_COMPOUND) {
                    SerializeCompound(item.Value as TagCompound, str, level + 1);
                }
                else if (item.Value.GetTagType() == TagType.TAG_LIST) {
                    SerializeList(item.Value as TagList, str, level + 1);
                }
                else {
                    SerializeScaler(item.Value, str);
                }

                first = false;
            }

            str.AppendLine();
            Add(str, "}", level);
        }

        private static void SerializeList (TagList tag, StringBuilder str, int level)
        {
            if (tag.Count == 0) {
                str.Append("[ ]");
                return;
            }

            str.AppendLine();
            AddLine(str, "[", level);

            IEnumerator<TagValue> en = tag.GetEnumerator();
            bool first = true;
            while (en.MoveNext()) {
                if (!first) {
                    str.Append(",");
                }

                TagValue item = en.Current;

                if (item.GetTagType() == TagType.TAG_COMPOUND) {
                    SerializeCompound(item as TagCompound, str, level + 1);
                }
                else if (item.GetTagType() == TagType.TAG_LIST) {
                    SerializeList(item as TagList, str, level + 1);
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

        private static void SerializeScaler (TagValue tag, StringBuilder str)
        {
            TagType type = tag.GetTagType();
            switch (tag.GetTagType()) {
                case TagType.TAG_STRING:
                    str.Append("\"" + tag.ToTagString().Data + "\"");
                    break;

                case TagType.TAG_BYTE:
                    str.Append(tag.ToTagByte().Data);
                    break;

                case TagType.TAG_SHORT:
                    str.Append(tag.ToTagShort().Data);
                    break;

                case TagType.TAG_INT:
                    str.Append(tag.ToTagInt().Data);
                    break;

                case TagType.TAG_LONG:
                    str.Append(tag.ToTagLong().Data);
                    break;

                case TagType.TAG_FLOAT:
                    str.Append(tag.ToTagFloat().Data);
                    break;

                case TagType.TAG_DOUBLE:
                    str.Append(tag.ToTagDouble().Data);
                    break;

                case TagType.TAG_BYTE_ARRAY:
                    str.Append(Convert.ToBase64String(tag.ToTagByteArray().Data));
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
