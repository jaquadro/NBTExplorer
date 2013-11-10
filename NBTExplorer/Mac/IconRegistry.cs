using System;
using System.Collections.Generic;
using MonoMac.AppKit;

namespace NBTExplorer.Mac
{
	public class IconRegistry
	{
		private Dictionary<Type, NSImage> _iconRegistry;
		
		public IconRegistry ()
		{
			_iconRegistry = new Dictionary<Type, NSImage>();
		}

		public NSImage DefaultIcon { get; set; }
		
		public NSImage Lookup (Type type)
		{
			if (_iconRegistry.ContainsKey(type))
				return _iconRegistry[type];
			else
				return DefaultIcon;
		}
		
		public void Register (Type type, NSImage icon)
		{
			_iconRegistry[type] = icon;
		}
	}
}

