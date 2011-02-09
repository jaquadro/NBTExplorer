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

#include "replace.h"

int pf_replace (nbt_file * nf, struct options * opt, void * pf_opt) {
	int x, y, z;
	struct options_replace * rep_opt = (struct options_replace *)pf_opt;
	
	nbt_byte_array *arr;
    nbt_byte_array *dat;
    nbt_tag *blocks = nbt_find_tag_by_name("Blocks", nbt_find_tag_by_name("Level", nf->root));
    nbt_tag *data = nbt_find_tag_by_name("Data", nbt_find_tag_by_name("Level", nf->root));

    /* 'blocks' cannot be NULL as we already confirmed a valid file */
    assert(blocks != NULL);
    assert(data != NULL);

    arr = nbt_cast_byte_array(blocks);
    dat = nbt_cast_byte_array(data);

    for (y = rep_opt->min_depth; y <= rep_opt->max_depth; y++) {
    	for (x = 0; x < 16; x++) {
    		for (z = 0; z < 16; z++) {
    			if (rep_opt->set & OPT_RANDOM) {
	    			float c = (rand() % 1000) / 1000.0;
	    			if (c > rep_opt->p) {
	    				continue;
	    			}
	    		}

    			int index = INDEXAT(x, y, z);
    			if (arr->content[index] == rep_opt->old_id) {
    				arr->content[index] = rep_opt->new_id;
    				
    				if (opt->set & OPT_VV) {
						fprintf(stderr, "Replaced block at %d,%d,%d\n", x, y, z);
					}
					
					if (rep_opt->set & OPT_DATA) {
						if (index % 2 == 0) {
							dat->content[index / 2] = (dat->content[index / 2] & 0xF0) | rep_opt->data;
						}
						else {
							dat->content[index / 2] = (dat->content[index / 2] & 0x0F) | (rep_opt->data << 4);
						}
					}
    			}
    		}
    	}
    }

    return EXIT_SUCCESS;
}
