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

#include "oregen.h"

const struct ore_record ore_list[] = {
	{ "Coal", BLOCK_COAL, 20, 0, 128, 16 },
	{ "Iron", BLOCK_IRON, 20, 0, 64, 8 },
	{ "Gold", BLOCK_GOLD, 2, 0, 32, 8 },
	{ "Redstone", BLOCK_REDSTONE, 8, 0, 16, 7 },
	{ "Diamond", BLOCK_DIAMOND, 1, 0, 16, 7 },
	{ "Lapis", BLOCK_LAPIS, 1, 0, 32, 7 },
};

void update_block (nbt_byte_array *arr, nbt_byte_array *dat, int x, int y, int z, short ore_id, struct options *opt, struct options_gen_ore * ore_opt) {
	int index = INDEXAT(x, y, z);
	
	assert (index >= 0);
	assert (index < 32768);

	if (
		((ore_opt->set & OPT_OV_ALL) && (arr->content[index] != ore_opt->ore_id)) ||
		((ore_opt->set & OPT_OV_ORE) && (
			arr->content[index] == BLOCK_COAL || arr->content[index] == BLOCK_IRON ||
			arr->content[index] == BLOCK_GOLD || arr->content[index] == BLOCK_REDSTONE ||
			arr->content[index] == BLOCK_DIAMOND || arr->content[index] == BLOCK_LAPIS ||
			arr->content[index] == BLOCK_DIRT || arr->content[index] == BLOCK_GRAVEL) && (arr->content[index] != ore_opt->ore_id)) ||
		((ore_opt->set & OPT_OV_BLK) && (arr->content[index] != BLOCK_AIR) && (arr->content[index] != ore_opt->ore_id)) ||
		(arr->content[index] == BLOCK_STONE)
	) {
		arr->content[index] = ore_opt->ore_id;
		
		if (opt->set & OPT_VV) {
			fprintf(stderr, "Added block at %d,%d,%d\n", x, y, z);
		}
		
		if (ore_opt->set & OPT_DATA) {
			if (index % 2 == 0) {
				dat->content[index / 2] = (dat->content[index / 2] & 0xF0) | ore_opt->data;
			}
			else {
				dat->content[index / 2] = (dat->content[index / 2] & 0x0F) | (ore_opt->data << 4);
			}
		}
	}
}

void gen_ore (nbt_byte_array *arr, nbt_byte_array *dat, short ore_id, struct options *opt, struct options_gen_ore * ore_opt)
{     
	int i;
	short ore_rounds = ore_opt->rounds;
	short ore_min = ore_opt->min_depth;
	short ore_max = ore_opt->max_depth;
	short ore_size = ore_opt->size;

	float x_scale = 0.25 + (rand() % 3000) / 4000.0;
	float y_scale = 0.25 + (rand() % 3000) / 4000.0;
	float z_scale = 0.25 + (rand() % 3000) / 4000.0;
	
	if (opt->set & OPT_VV) {
		fprintf(stderr, "Selected scale: %f, %f, %f\n", x_scale, y_scale, z_scale);
	}
	
	float x_len = ore_size / 8.0 * x_scale;
	float z_len = ore_size / 8.0 * z_scale;
	float y_len = (ore_size / 16.0 + 2.0) * y_scale;
	
	if (opt->set & OPT_VV) {
		fprintf(stderr, "Selected length: %f, %f, %f\n", x_len, y_len, z_len);
	}
	
	float xpos = (rand() % (16000 - (int)(x_len * 1000.0))) / 1000.0;
	float zpos = (rand() % (16000 - (int)(z_len * 1000.0))) / 1000.0;
	float ypos = ore_min + (rand() % ((int)((ore_max - ore_min) * 1000.0))) / 1000.0 + 2.0;
	
	if (opt->set & OPT_VV) {
		fprintf(stderr, "Selected initial position: %f, %f, %f\n", xpos, ypos, zpos);
	}
	
	int sample_size = 2 * ore_size;
	float fuzz = 0.25;
	
	float x_step = x_len / sample_size;
	float y_step = y_len / sample_size;
	float z_step = z_len / sample_size;
	
	for (i = 0; i < sample_size; i++) {
		int tx = floor(xpos + i * x_step);
		int ty = floor(ypos + i * y_step);
		int tz = floor(zpos + i * z_step);
		int txp = floor(xpos + i * x_step + fuzz);
		int typ = floor(ypos + i * y_step + fuzz);
		int tzp = floor(zpos + i * z_step + fuzz);
		
		if (tx < 0) tx = 0;
		if (ty < 0) ty = 0;
		if (tz < 0) tz = 0;
		
		if (tx >= 16) tx = 15;
		if (ty >= 128) ty = 127;
		if (tz >= 16) tz = 15;
		
		if (txp < 0) txp = 0;
		if (typ < 0) typ = 0;
		if (tzp < 0) tzp = 0;
		
		if (txp >= 16) txp = 15;
		if (typ >= 128) typ = 127;
		if (tzp >= 16) tzp = 15;
		
		update_block(arr, dat, tx, ty, tz, ore_id, opt, ore_opt);
		update_block(arr, dat, txp, ty, tz, ore_id, opt, ore_opt);
		update_block(arr, dat, tx, typ, tz, ore_id, opt, ore_opt);
		update_block(arr, dat, tx, ty, tzp, ore_id, opt, ore_opt);
		update_block(arr, dat, txp, typ, tz, ore_id, opt, ore_opt);
		update_block(arr, dat, tx, typ, tzp, ore_id, opt, ore_opt);
		update_block(arr, dat, txp, ty, tzp, ore_id, opt, ore_opt);
		update_block(arr, dat, txp, typ, tzp, ore_id, opt, ore_opt);
	}
}

int pf_gen_ore (nbt_file * nf, struct options * opt, void * pf_opt) {
	int i;
	struct options_gen_ore * ore_opt = (struct options_gen_ore *)pf_opt;
	
	nbt_byte_array *arr;
    nbt_byte_array *dat;
    nbt_tag *blocks = nbt_find_tag_by_name("Blocks", nbt_find_tag_by_name("Level", nf->root));
    nbt_tag *data = nbt_find_tag_by_name("Data", nbt_find_tag_by_name("Level", nf->root));

    /* 'blocks' cannot be NULL as we already confirmed a valid file */
    assert(blocks != NULL);
    assert(data != NULL);

    arr = nbt_cast_byte_array(blocks);
    dat = nbt_cast_byte_array(data);
        
    if (opt->set & OPT_VV) {
	    fprintf(stderr, "Generating %d (rounds %d, min %d, max %d, size %d)\n", 
	    	ore_opt->ore_id, ore_opt->rounds, ore_opt->min_depth, ore_opt->max_depth, ore_opt->size);
	}
        
    for (i = 0; i < ore_opt->rounds; i++) {
    	if (opt->set & OPT_VV) {
	      	fprintf(stderr, "Generating round %d...\n", i);
	    }
        gen_ore(arr, dat, ore_opt->ore_id, opt, ore_opt);
    }
    
    return EXIT_SUCCESS;
}
