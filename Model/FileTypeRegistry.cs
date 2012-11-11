using System;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    public delegate bool NamePatternTestFunc (string path);
    public delegate DataNode NodeCreateFunc (string path);

    public class FileTypeRecord
    {
        public NamePatternTestFunc NamePatternTest { get; set; }
        public NodeCreateFunc NodeCreate { get; set; }
    }

    public class FileTypeRegistry
    {
        private static Dictionary<Type, FileTypeRecord> _registry = new Dictionary<Type, FileTypeRecord>();

        public static FileTypeRecord Lookup (Type type)
        {
            if (_registry.ContainsKey(type))
                return _registry[type];
            else
                return null;
        }

        public static void Register (Type type, FileTypeRecord record)
        {
            _registry[type] = record;
        }

        public static void Register<T> (FileTypeRecord record)
        {
            Register(typeof(T), record);
        }

        public static IEnumerable<KeyValuePair<Type, FileTypeRecord>> RegisteredTypes
        {
            get 
            {
                foreach (var item in _registry)
                    yield return item;
            }
        }

        static FileTypeRegistry ()
        {
            try {
                Register<NbtFileDataNode>(new FileTypeRecord() {
                    NamePatternTest = NbtFileDataNode.SupportedNamePattern,
                    NodeCreate = NbtFileDataNode.TryCreateFrom,
                });

                Register<RegionFileDataNode>(new FileTypeRecord() {
                    NamePatternTest = RegionFileDataNode.SupportedNamePattern,
                    NodeCreate = RegionFileDataNode.TryCreateFrom,
                });

                Register<CubicRegionDataNode>(new FileTypeRecord() {
                    NamePatternTest = CubicRegionDataNode.SupportedNamePattern,
                    NodeCreate = CubicRegionDataNode.TryCreateFrom,
                });
            }
            catch (Exception) {
                //Program.StaticInitFailure(e);
            }
        }
    }
}
