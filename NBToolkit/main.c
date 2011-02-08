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
#include "chunk.h"

void print_usage (const char *name) {
	fprintf(stderr, "Usage: %s <world dir> <tool> <ore_id> [options]\n", name);
	fprintf(stderr, "Available tools:\n");
	fprintf(stderr, "    oregen      : Generate new ore deposits\n");
	fprintf(stderr, "    replace     : Replace one block type with another\n\n");
	fprintf(stderr, "Usage and options for 'oregen':\n");
	fprintf(stderr, "    %s <world_dir> oregen <ore_id> [options]\n", name);
	fprintf(stderr, "    Ore IDs:\n");
	fprintf(stderr, "        16 = Coal, 15 = Iron, 14 = Gold, 73 = Redstone, 56 = Diamond, 21 = Lapis\n");
	fprintf(stderr, "    Options:\n");
	fprintf(stderr, "        -r <num>    : Number of rounds ore is generated per chunk\n");
	fprintf(stderr, "        -mn <num>   : Minimum depth ore is generated (0-127)\n");
	fprintf(stderr, "        -mx <num>   : Maximum depth ore is generated (0-127)\n");
	fprintf(stderr, "        -s <num>    : Size of ore deposit (0-63) -- Defaults:\n");
	fprintf(stderr, "                      Coal = 16, Iron/Gold = 8, Redstone/Diamond/Lapis = 7\n");
	fprintf(stderr, "        -oo         : Take precedence over all existing ores\n");
	fprintf(stderr, "        -ob         : Take precedence over all existing blocks (excluding air)\n");
	fprintf(stderr, "        -oa         : Take precedence over all existing blocks (including air)\n");
	fprintf(stderr, "        -oi <id>    : Take precedence over a specific block type\n\n");
	fprintf(stderr, "Usage and options for 'replace':\n");
	fprintf(stderr, "    %s <world_dir> replace <block_id_1> <block_id_2> [options]\n", name);
	fprintf(stderr, "    Options:\n");
	fprintf(stderr, "        -mn <num>   : Minimum depth ore is generated (0-127)\n");
	fprintf(stderr, "        -mx <num>   : Maximum depth ore is generated (0-127)\n");
	fprintf(stderr, "        -p <num>    : Probability that an individual block is replaced (0.0 - 1.0)\n\n");
	fprintf(stderr, "Common Options:\n");
	/*fprintf(stderr, "    -cb <time>	    : Only update chunks created before time (as unix timestamp)\n");
	fprintf(stderr, "    -ca <time>	    : Only update chunks created after time (as unix timestamp)\n");*/
	fprintf(stderr, "    -mb <time>  : Only update chunks modified before time (as unix timestamp)\n");
	fprintf(stderr, "    -ma <time>  : Only update chunks modified after time (as unix timestamp)\n");
	fprintf(stderr, "    -bb <x1> <y1> <x2> <y2>\n");
	fprintf(stderr, "                : Only update chunks with coordinates bounded by the rectangle\n");
	fprintf(stderr, "                  by (x1, y1) and (x2, y2), given as decimal chunk coordinates\n");
	fprintf(stderr, "    -e <id>     : Only update chunks that contain blocks of the given id\n");
	fprintf(stderr, "    -ne <id>    : Only update chunks that don't contain blocks of the given id\n");
	fprintf(stderr, "    -d <num>    : Set block-specific data values (0-15)\n");
	fprintf(stderr, "    -v          : Verbose output\n");
	fprintf(stderr, "    -vv         : Very verbose output\n");
}

void parse_options (int argc, char **argv, struct options *opt, struct options_gen_ore * ore_opt) {
	int i;
	int t;
	
	ore_opt->rounds = 0;
	ore_opt->min_depth = 0;
	ore_opt->max_depth = 0;
	ore_opt->size = 0;
	
	for (i = 0; i < argc; i++) {
		if (strcmp(argv[i], "-r") == 0) {
			ore_opt->set |= OPT_ROUNDS;
			sscanf(argv[i+1], "%d", &ore_opt->rounds);
		}
		
		if (strcmp(argv[i], "-mn") == 0) {
			ore_opt->set |= OPT_MIN;
			sscanf(argv[i+1], "%d", &ore_opt->min_depth);
			ore_opt->min_depth %= 128;
		}
		
		if (strcmp(argv[i], "-mx") == 0) {
			ore_opt->set |= OPT_MAX;
			sscanf(argv[i+1], "%d", &ore_opt->max_depth);
			ore_opt->max_depth %= 128;
		}
		
		if (strcmp(argv[i], "-s") == 0) {
			ore_opt->set |= OPT_SIZE;
			sscanf(argv[i+1], "%d", &ore_opt->size);
			ore_opt->size %= 64;
		}
		
		if (strcmp(argv[i], "-cb") == 0) {
			opt->set |= OPT_C_TIME;
			sscanf(argv[i+1], "%ud", &opt->c_time);
		}
		
		if (strcmp(argv[i], "-ca") == 0) {
			opt->set |= OPT_C_TIME;
			opt->set |= OPT_C_AFT;
			sscanf(argv[i+1], "%ud", &opt->c_time);
		}
		
		if (strcmp(argv[i], "-mb") == 0) {
			opt->set |= OPT_M_TIME;
			sscanf(argv[i+1], "%ud", &opt->m_time);
		}
		
		if (strcmp(argv[i], "-ma") == 0) {
			opt->set |= OPT_M_TIME;
			opt->set |= OPT_M_AFT;
			sscanf(argv[i+1], "%ud", &opt->m_time);
		}
		
		if (strcmp(argv[i], "-oo") == 0) {
			ore_opt->set |= OPT_OV_ORE;
		}
		
		if (strcmp(argv[i], "-oa") == 0) {
			ore_opt->set |= OPT_OV_ALL;
		}
		
		if (strcmp(argv[i], "-ob") == 0) {
			ore_opt->set |= OPT_OV_BLK;
		}
		
		if (strcmp(argv[i], "-bb") == 0) {
			opt->set |= OPT_BBOX;
			sscanf(argv[i+1], "%d", &opt->x1);
			sscanf(argv[i+2], "%d", &opt->y1);
			sscanf(argv[i+3], "%d", &opt->x2);
			sscanf(argv[i+4], "%d", &opt->y2);
			
			if (opt->x1 > opt->x2) {
				t = opt->x1;
				opt->x1 = opt->x2;
				opt->x2 = t;
			}
			
			if (opt->y1 > opt->y2) {
				t = opt->y1;
				opt->y1 = opt->y2;
				opt->y2 = t;
			}
		}
		
		if (strcmp(argv[i], "-d") == 0) {
			ore_opt->set |= OPT_DATA;
			sscanf(argv[i+1], "%d", &ore_opt->data);
			ore_opt->data %= 16;
		}
		
		if (strcmp(argv[i], "-v") == 0) {
			opt->set |= OPT_V;
		}
		
		if (strcmp(argv[i], "-vv") == 0) {
			opt->set |= OPT_V;
			opt->set |= OPT_VV;
		}
	}
}

int main(int argc, char **argv)
{
	struct options opt;
	struct options_gen_ore ore_opt;
	
	opt.set = 0;
	ore_opt.set = 0;
	
    /* argv[1] == chunk file
     * argv[2] == ore id
     */

	int i;
    int ore_id = -1; /* No valid minecraft ID */
    
    srand ( time(NULL) );

    /* Validate arguments */
    if ((argc < 3))
    {
        print_usage(argv[0]);
        
        return EXIT_FAILURE;
    }
    else
    {
        /* Parse the parameters */
		if (sscanf(argv[2], "%d", &ore_id))
		{
            ore_id %= 256; /* Make sure it's 8-bit sized */
            ore_opt.ore_id = ore_id;
        }
        else
        {
            fprintf(stderr, "Ore ID has to be a number\n");

            return EXIT_FAILURE;
        }
        
        parse_options(argc, argv, &opt, &ore_opt);
        
        for (i = 0; i < ORE_COUNT; i++) {
        	if (ore_list[i].block_id == ore_id) {
        		if (!(ore_opt.set & OPT_ROUNDS)) {
        			ore_opt.rounds = ore_list[i].rounds;
        		}
        		if (!(ore_opt.set & OPT_MIN)) {
        			ore_opt.min_depth = ore_list[i].min_depth;
        		}
        		if (!(ore_opt.set & OPT_MAX)) {
        			ore_opt.max_depth = ore_list[i].max_depth;
        		}
        		if (!(ore_opt.set & OPT_SIZE)) {
        			ore_opt.size = ore_list[i].size;
        		}
        	}
        }

		return update_all_chunks(argv[1], pf_gen_ore, &opt, &ore_opt);
	}
}
