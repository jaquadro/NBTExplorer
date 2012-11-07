using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace NBTExplorer
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		
		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			mainWindowController.Window.AppDelegate = this;
		}

		public NSMenuItem MenuAbout
		{
			get { return _menuAbout; }
		}

		public NSMenuItem MenuQuit
		{
			get { return _menuQuit; }
		}

		public NSMenuItem MenuOpen
		{
			get { return _menuOpen; }
		}

		public NSMenuItem MenuOpenFolder
		{
			get { return _menuOpenFolder; }
		}

		public NSMenuItem MenuOpenMinecraft
		{
			get { return _menuOpenMinecraft; }
		}

		public NSMenuItem MenuSave
		{
			get { return _menuSave; }
		}

		public NSMenuItem MenuCut
		{
			get { return _menuCut; }
		}

		public NSMenuItem MenuCopy
		{
			get { return _menuCopy; }
		}

		public NSMenuItem MenuPaste
		{
			get { return _menuPaste; }
		}

		public NSMenuItem MenuRename
		{
			get { return _menuRename; }
		}

		public NSMenuItem MenuEditValue
		{
			get { return _menuEditValue; }
		}

		public NSMenuItem MenuDelete
		{
			get { return _menuDelete; }
		}

		public NSMenuItem MenuFind
		{
			get { return _menuFind; }
		}

		public NSMenuItem MenuFindNext
		{
			get { return _menuFindNext; }
		}

		public NSMenuItem MenuInsertByte
		{
			get { return _menuInsertByte; }
		}

		public NSMenuItem MenuInsertShort
		{
			get { return _menuInsertShort; }
		}

		public NSMenuItem MenuInsertInt
		{
			get { return _menuInsertInt; }
		}

		public NSMenuItem MenuInsertLong
		{
			get { return _menuInsertLong; }
		}

		public NSMenuItem MenuInsertFloat
		{
			get { return _menuInsertFloat; }
		}

		public NSMenuItem MenuInsertDouble
		{
			get { return _menuInsertDouble; }
		}

		public NSMenuItem MenuInsertByteArray
		{
			get { return _menuInsertByteArray; }
		}

		public NSMenuItem MenuInsertIntArray
		{
			get { return _menuInsertIntArray; }
		}

		public NSMenuItem MenuInsertString
		{
			get { return _menuInsertString; }
		}

		public NSMenuItem MenuInsertList
		{
			get { return _menuInsertList; }
		}

		public NSMenuItem MenuInsertCompound
		{
			get { return _menuInsertCompound; }
		}

		partial void ActionRename (NSObject sender)
		{
			mainWindowController.Window.ActionRenameValue();
		}

		partial void ActionEditValue (NSObject sender)
		{
			mainWindowController.Window.ActionEditValue();
		}

		partial void ActionDelete (NSObject sender)
		{
			mainWindowController.Window.ActionDeleteValue();
		}
	}
}

