# [NBTExplorer](https://www.minecraftforum.net/forums/mapping-and-modding-java-edition/minecraft-tools/1262665-nbtexplorer-nbt-editor-for-windows-and-mac)

NBTExplorer is an open-source NBT editor for all common sources of NBT data, and is originally based on [NBTedit](http://www.minecraftforum.net/topic/6661-nbtedit/). The key difference is NBTExplorer's full support for Minecraft .mcr/.mca region files, a directory-tree interface for easily exploring multiple worlds, and support for the latest NBT standard. NBTExplorer is built on top of [Substrate](http://www.minecraftforum.net/topic/245996-sdk-substrate-map-editing-library-for-cnet-090/). It's mainly intended for editing [Minecraft](http://www.minecraft.net) game data.

---
## Supported Formats

NBTExplorer supports reading and writing the following formats:

* Standard NBT files (e.g. level.dat)
* Schematic files
* Uncompressed NBT files (e.g. idcounts.dat)
* Minecraft region files (*.mcr)
* Minecraft anvil files (*.mca)
* Cubic Chunks region files (r2*.mcr, r2*.mca)

##### The NBTExplorer zip and installer packages for Windows now include a second utility, NBTUtil.exe, for command-line control of NBT data. NBTUtil currently has a limited featureset compared to NBTExplorer.

###### I recommend backing up worlds before modifying them with this tool.
|Operating System|Release|
|:-----:|:-----:|
|Windows/Linux|[(Version 2.8.0)](https://github.com/jaquadro/NBTExplorer/releases/tag/v2.8.0-win) [MSI installer or ZIP archive] |
|MacOS|[(Version 2.0.3)](http://hocuspocus.taloncrossing.com/rii/NBTExplorer-Mac-2.0.3.zip)|
###### If you have problems with the native client or you're running OS X 10.6 or older, you can still run the Windows version of NBTExplorer on your Mac by following the Linux instructions and installing the Mono runtime. You may still need to disable GateKeeper quarantine on the downloaded files.

---
## Software Screenshots
![Windows](https://user-images.githubusercontent.com/101172593/185527540-45ab8adb-956e-49ad-a999-85ba351034dd.png)

![MacOS](https://user-images.githubusercontent.com/101172593/185528096-8ba2a1d8-2455-48a5-8474-6ed12bee8004.png)

---
## System Requirements

### Windows

Windows XP or later, .NET Framework 2.0 or later.

### Linux

NBTExplorer is compatible with recent Mono runtimes, at least 2.6 or later.
Minimally, your system needs the `mono-core` and `mono-winforms` packages, or whatever package set is equivalent.

### Mac

A separate Mac version with a native UI is available.  All Mono dependencies are included within the app package.
Minimum supported OS is OSX 10.8 Mountain Lion, but it may run on versions as early as Snow Leopard.

The Windows version of NBTExplorer can still be used if the Mac version does not work.  You will need to install the
Mono runtime, and then run NBTExplorer with Mono from the command line.

---
## Frequently Asked Questions

#### Q: NBTExplorer didn't save my changes -- why?

##### A: It's possible you forgot to save. The more likely answer is NBTExplorer did save your changes, and if you were to immediately re-open your world in NBTExplorer, you would see that your changes are still there. Instead, Minecraft erased or ignored your changes when you loaded your world. The most common trap is editing player settings or inventory on a single-player world and making your changes in a .dat file instead of in level.dat. Minecraft will overwrite your .dat file on load with the contents of the player tag in level.dat.

#### Q: I'm on Windows and NBTExplorer crashed on start / didn't run.

##### A: Did you get a message box listing one or more exceptions? Reply to this thread with the information so you can be better helped.

###### If you didn't get any message box though, or you only received a standard "program has stopped working" message box, then you have deeper issues with your .NET environment. Do any other .NET programs run on your computer? If yes, then try deleting the NBTExplorer.exe.config file that was installed / came with the zip. This will cause NBTExplorer to prefer the .NET 2.x/3.x framework over the 4.x framework, and may allow the program to run if your .NET 4.x framework is toast. You should still do something about that, of course.

#### Q: I'm on Windows and NBTExplorer crashed with a scary warning.

##### A: Windows 8 and later includes a program filter called "SmartScreen" that prevents unrecognized software from running. Its warning looks something like this:

![Smartscreen Error](https://user-images.githubusercontent.com/101172593/185528570-a09f8353-47a1-42c6-9e0d-895441cd1db9.png)

###### If you see this screen, press the "More info" link to get a "Run Anyway" button. This will launch the program, and you shouldn't see the warning again unless you update NBTExplorer.

###### NBTExplorer is SAFE and has an established reputation in the Minecraft community. However, you don't need to take my word at face-value. The source code is publicly available for inspection, and you can download and build your own copy if you so wish.

###### SmartScreen builds up reputation on individual files. So you may see this warning if you download a freshly released update of NBTExplorer, but not if you download a version that has been out for a while. Unfortunately the permanent fix for this is to buy very expensive signing certificates that must be renewed annually, which is not viable for a free tool like this.

#### Q: The Mac version doesn't run on my Mac!

##### A: If you're running OSX 10.6.x or earlier, the Mac version is confirmed to not work. It has been tested and confirmed to work for people on 10.7.x and 10.8.x, although that may still not be a guarantee that it will work for you, or that it will be bug-free.

###### If you're having problems running the Mac-specific version of NBTExplorer, there is still a high probability that you can run the Windows/Linux version instead. thhinds has posted a more detailed set of instructions here. It won't be as pretty -- you'll get a very crude looking version of Windows UI styling instead of native OSX styling, but it should still do the job. I can't do anything about this unless an experienced Cocoa developer that also has some .NET/Mono experience is willing to step up and help me determine why it won't run on older versions of OSX.

#### Q: When I run the Mac version, I see: OSStatus error -67053

##### A: It has been reported that this is caused by Gatekeeper disallowing NBTExplorer from running because it is not a properly signed application. Please refer to the instructions in the "Mac Users" section above for bypassing GateKeeper's check. NOTE: As of August 22, 2013, I have begun signing NBTExplorer. Try downloading the latest version first.
