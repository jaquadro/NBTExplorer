using System;
using System.Collections.Generic;

namespace NBTExplorer.Windows
{
    public class IconRegistry
    {
        private Dictionary<Type, int> _iconRegistry;

        public IconRegistry ()
        {
            _iconRegistry = new Dictionary<Type, int>();
        }

        public int DefaultIcon { get; set; }

        public int Lookup (Type type)
        {
            if (_iconRegistry.ContainsKey(type))
                return _iconRegistry[type];
            else
                return DefaultIcon;
        }

        public int Lookup<T> ()
        {
            return Lookup(typeof(T));
        }

        public void Register (Type type, int iconIndex)
        {
            _iconRegistry[type] = iconIndex;
        }
    }
}
