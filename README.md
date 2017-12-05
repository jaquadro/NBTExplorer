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
Minimally, your system needs the `mono-core` and `mono-winforms` packages, or whatever package set is equivalent. [This blog post](http://blog2.vorburger.ch/2017/02/how-build-mono-app-like-eg-minecraft.html) explains how to build and run e.g. on Fedora Linux:

    sudo dnf install mono-core mono-winforms mono-devel mono-basic
    git clone git@github.com:jaquadro/NBTExplorer.git ; cd NBTExplorer
    xbuild /p:TargetFrameworkVersion="v4.5"
    mono ./NBTExplorer/bin/Release/NBTExplorer.exe <your-minecraft-world/>

_Just ignore the wixproj Installer project related build error._

### Mac

A separate Mac version with a native UI is available.  All Mono dependencies are included within the app package.
Minimum supported OS is OSX 10.8 Mountain Lion, but it may run on versions as early as Snow Leopard.

The Windows version of NBTExplorer can still be used if the Mac version does not work.  You will need to install the
Mono runtime, and then run NBTExplorer with Mono from the command line.
