all: windows

libnbt.a:
	gcc -g -I./cnbt -c cnbt/nbt.c -o cnbt/nbt.o
	gcc -g -I./cnbt -c cnbt/endianness.c -o cnbt/endianness.o
	ar -rcs cnbt/libnbt.a cnbt/nbt.o cnbt/endianness.o

windows: libnbt.a
	gcc -g oregen.c chunk.c main.c -o oregen.exe -I./cnbt -L./cnbt -lnbt /mingw/lib/libz.a

linux: libnbt.a
	gcc -g oregen.c chunk.c main.c -o oregen -I./cnbt -L./cnbt -lnbt -lz -lm