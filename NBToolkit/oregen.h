/* OreGen - MineCraft Ore Generator */
/*
Copyright (C) 2011 by Justin Aquadro

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#ifndef OREGEN_H_
#define OREGEN_H_

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <time.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <dirent.h>
#include <errno.h>
#include <math.h>

#include "cnbt/nbt.h"
#include "chunk.h"

#define INDEXAT(x,y,z) ((y) + ((z) * 128 + (x) * 128 * 16))

#define BLOCK_AIR 0
#define BLOCK_STONE 1
#define BLOCK_DIRT 3
#define BLOCK_GRAVEL 13
#define BLOCK_COAL 16
#define BLOCK_IRON 15
#define BLOCK_GOLD 14
#define BLOCK_REDSTONE 73
#define BLOCK_DIAMOND 56
#define BLOCK_LAPIS 21

struct ore_record {
	char * name;
	short block_id;
	short rounds;
	short min_depth;
	short max_depth;
	short size;
};

struct options_gen_ore {
	unsigned long set;
	int ore_id;
	int rounds;
	int min_depth;
	int max_depth;
	int size;
	int data;
};

#define ORE_COUNT 6

const struct ore_record ore_list[ORE_COUNT];

/* Attempt to update single block in chunk */
void update_block (nbt_byte_array *arr, nbt_byte_array *dat, int x, int y, int z, short ore_id, struct options *opt, struct options_gen_ore * ore_opt);

/* Generate randomized ore parameters for chunk and update blocks */
void gen_ore (nbt_byte_array *arr, nbt_byte_array *dat, short ore_id, struct options *opt, struct options_gen_ore * ore_opt);

/* Callback function to chunk API */
int pf_gen_ore (nbt_file * nf, struct options * opt, void * pf_opt);

#endif

