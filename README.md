# NBTExplorer

NBTExplorer is an open-source NBT editor for all common sources of NBT data.  It's mainly intended for editing [Minecraft](http://www.minecraft.net) game data.

## Supported Formats

NBTExplorer supports reading and writing the following formats:

* Standard NBT files (e.g. level.dat)
* Schematic files
* Uncompressed NBT files (e.g. idcounts.dat)
* Minecraft region files (*.mcr)
* Minecraft anvil files (*.mca)
* Cubic Chunks region files (r2*.mcr, r2*.mca)

## System Requirements

### Windows

Windows XP or later, .NET Framework 2.0 or later.

### Linux

NBTExplorer is compatible with recent Mono runtimes, at least 2.6 or later.
Minimally, your system needs the `mono-core` and `mono-winforms` packages, or whatever package set is equivalent.


### Mac

macOS development has stopped as it has become very difficult to maintain. There is an old release that requires macOS 10.8 or greater, however it is not compatible with macOS 10.15 Catalina as Catalina dropped support for 32-bit applications. If you want to use newer versions of NBTExplorer, you can try using Mono, just like on Linux. You can install Mono through HomeBrew with `brew install mono` or as a .pkg file from Mono's website. However, because Mono uses Carbon to replace Winforms, and Carbon is considered Depricated and was no longer supported on macOS 10.13 High Sierra and removed on 10.15 Catalina, you may run into some issues. Another option could be using WINE (is not an emulator) to run NBTExplorer, though this has yet to be confirmed to work. 
