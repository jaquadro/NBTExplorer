// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NBTExplorer
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuAbout { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuQuit { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuOpen { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuOpenFolder { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuOpenMinecraft { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuSave { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuCut { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuCopy { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuPaste { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuRename { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuEditValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuDelete { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuMoveUp { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuMoveDown { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuFind { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuFindNext { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertByte { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertShort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertInt { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertLong { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertFloat { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertDouble { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertByteArray { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertIntArray { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertString { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertList { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem _menuInsertCompound { get; set; }

		[Action ("ActionOpen:")]
		partial void ActionOpen (MonoMac.Foundation.NSObject sender);

		[Action ("ActionOpenFolder:")]
		partial void ActionOpenFolder (MonoMac.Foundation.NSObject sender);

		[Action ("ActionOpenMinecraft:")]
		partial void ActionOpenMinecraft (MonoMac.Foundation.NSObject sender);

		[Action ("ActionSave:")]
		partial void ActionSave (MonoMac.Foundation.NSObject sender);

		[Action ("ActionCut:")]
		partial void ActionCut (MonoMac.Foundation.NSObject sender);

		[Action ("ActionCopy:")]
		partial void ActionCopy (MonoMac.Foundation.NSObject sender);

		[Action ("ActionPaste:")]
		partial void ActionPaste (MonoMac.Foundation.NSObject sender);

		[Action ("ActionRename:")]
		partial void ActionRename (MonoMac.Foundation.NSObject sender);

		[Action ("ActionEditValue:")]
		partial void ActionEditValue (MonoMac.Foundation.NSObject sender);

		[Action ("ActionDelete:")]
		partial void ActionDelete (MonoMac.Foundation.NSObject sender);

		[Action ("ActionMoveUp:")]
		partial void ActionMoveUp (MonoMac.Foundation.NSObject sender);

		[Action ("ActionMoveDown:")]
		partial void ActionMoveDown (MonoMac.Foundation.NSObject sender);

		[Action ("ActionFind:")]
		partial void ActionFind (MonoMac.Foundation.NSObject sender);

		[Action ("ActionFindNext:")]
		partial void ActionFindNext (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertByte:")]
		partial void ActionInsertByte (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertShort:")]
		partial void ActionInsertShort (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertInt:")]
		partial void ActionInsertInt (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertLong:")]
		partial void ActionInsertLong (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertFloat:")]
		partial void ActionInsertFloat (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertDouble:")]
		partial void ActionInsertDouble (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertByteArray:")]
		partial void ActionInsertByteArray (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertIntArray:")]
		partial void ActionInsertIntArray (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertString:")]
		partial void ActionInsertString (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertList:")]
		partial void ActionInsertList (MonoMac.Foundation.NSObject sender);

		[Action ("ActionInsertCompound:")]
		partial void ActionInsertCompound (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (_menuAbout != null) {
				_menuAbout.Dispose ();
				_menuAbout = null;
			}

			if (_menuQuit != null) {
				_menuQuit.Dispose ();
				_menuQuit = null;
			}

			if (_menuOpen != null) {
				_menuOpen.Dispose ();
				_menuOpen = null;
			}

			if (_menuOpenFolder != null) {
				_menuOpenFolder.Dispose ();
				_menuOpenFolder = null;
			}

			if (_menuOpenMinecraft != null) {
				_menuOpenMinecraft.Dispose ();
				_menuOpenMinecraft = null;
			}

			if (_menuSave != null) {
				_menuSave.Dispose ();
				_menuSave = null;
			}

			if (_menuCut != null) {
				_menuCut.Dispose ();
				_menuCut = null;
			}

			if (_menuCopy != null) {
				_menuCopy.Dispose ();
				_menuCopy = null;
			}

			if (_menuPaste != null) {
				_menuPaste.Dispose ();
				_menuPaste = null;
			}

			if (_menuRename != null) {
				_menuRename.Dispose ();
				_menuRename = null;
			}

			if (_menuEditValue != null) {
				_menuEditValue.Dispose ();
				_menuEditValue = null;
			}

			if (_menuDelete != null) {
				_menuDelete.Dispose ();
				_menuDelete = null;
			}

			if (_menuMoveUp != null) {
				_menuMoveUp.Dispose ();
				_menuMoveUp = null;
			}

			if (_menuMoveDown != null) {
				_menuMoveDown.Dispose ();
				_menuMoveDown = null;
			}

			if (_menuFind != null) {
				_menuFind.Dispose ();
				_menuFind = null;
			}

			if (_menuFindNext != null) {
				_menuFindNext.Dispose ();
				_menuFindNext = null;
			}

			if (_menuInsertByte != null) {
				_menuInsertByte.Dispose ();
				_menuInsertByte = null;
			}

			if (_menuInsertShort != null) {
				_menuInsertShort.Dispose ();
				_menuInsertShort = null;
			}

			if (_menuInsertInt != null) {
				_menuInsertInt.Dispose ();
				_menuInsertInt = null;
			}

			if (_menuInsertLong != null) {
				_menuInsertLong.Dispose ();
				_menuInsertLong = null;
			}

			if (_menuInsertFloat != null) {
				_menuInsertFloat.Dispose ();
				_menuInsertFloat = null;
			}

			if (_menuInsertDouble != null) {
				_menuInsertDouble.Dispose ();
				_menuInsertDouble = null;
			}

			if (_menuInsertByteArray != null) {
				_menuInsertByteArray.Dispose ();
				_menuInsertByteArray = null;
			}

			if (_menuInsertIntArray != null) {
				_menuInsertIntArray.Dispose ();
				_menuInsertIntArray = null;
			}

			if (_menuInsertString != null) {
				_menuInsertString.Dispose ();
				_menuInsertString = null;
			}

			if (_menuInsertList != null) {
				_menuInsertList.Dispose ();
				_menuInsertList = null;
			}

			if (_menuInsertCompound != null) {
				_menuInsertCompound.Dispose ();
				_menuInsertCompound = null;
			}
		}
	}
}
