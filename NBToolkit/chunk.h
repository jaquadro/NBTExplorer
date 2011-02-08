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

#ifndef NBT_CHUNK_H_
#define NBT_CHUNK_H_

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

#define OPT_C_TIME	0x0001
#define OPT_M_TIME	0x0002
#define OPT_ROUNDS	0x0004
#define OPT_MIN		0x0008
#define OPT_MAX		0x0010
#define OPT_SIZE	0x0020
#define OPT_OV_ORE	0x0040
#define OPT_OV_ALL	0x0080
#define OPT_C_AFT	0x0100
#define OPT_M_AFT	0x0200
#define OPT_OV_BLK	0x0400
#define OPT_BBOX	0x0800
#define OPT_DATA	0x1000
#define OPT_V		0x2000
#define OPT_VV		0x4000

struct options {
	unsigned long set;
	unsigned long c_time;
	unsigned long m_time;
	int rounds;
	int min_depth;
	int max_depth;
	int size;
	int x1;
	int y1;
	int x2;
	int y2;
	int data;
};

struct chunk_coords {
	int x;
	int y;
};

typedef int (* pf_type)(nbt_file * nf, struct options * opt, void * pf_opt);

int valid_chunk(nbt_tag *nbtroot);
int check_base32 (char * str);
int check_chunkname (char * str);
void chunk_to_coords (char * str, struct chunk_coords * cc);

int update_chunk (char * file, char * name, pf_type pf, struct options * opt, void * pf_opt);
int update_all_chunks (char *path, pf_type pf, struct options * opt, void * pf_opt);

#endif

