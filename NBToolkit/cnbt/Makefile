# -----------------------------------------------------------------------------
# "THE BEER-WARE LICENSE" (Revision 42):
# <webmaster@flippeh.de> wrote this file. As long as you retain this notice you
# can do whatever you want with this stuff. If we meet some day, and you think
# this stuff is worth it, you can buy me a beer in return. Lukas Niederbremer.
# -----------------------------------------------------------------------------

CFLAGS=-g -Wall

all: nbtreader datlevel blockreplacer

nbtreader: main.o libnbt.a
	gcc main.o -L. -lnbt -lz -o nbtreader -g

datlevel: datlevel.c libnbt.a
	gcc datlevel.c -L. -lnbt -lz -lm -o datlevel -g -Wall

blockreplacer: blockreplace.c libnbt.a
	gcc blockreplace.c -L. -lnbt -lz -o blockreplace -g -Wall

main.o: main.c

libnbt.a:
	gcc -g -c nbt.c -o nbt.o
	gcc -g -c endianness.c -o endianness.o
	ar -rcs libnbt.a nbt.o endianness.o
#nbt.o: nbt.h nbt.c
#endianness.o: endianness.h endianness.c
