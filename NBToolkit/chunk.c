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

#include "chunk.h"

int valid_chunk(nbt_tag *nbtroot)
{
    /* Check valid root element */
    nbt_tag *root = nbt_find_tag_by_name("Level", nbtroot);
    
    if ((root == NULL) || (strcmp(root->name, "Level") != 0) || (root->type != TAG_COMPOUND))
    	return 1;
    
    /* Check valid blocks element */
    nbt_tag *blocks = nbt_find_tag_by_name("Blocks", root);
    if ((blocks == NULL) || (blocks->type != TAG_BYTE_ARRAY))
    	return 1;
    
    nbt_byte_array *arr = (nbt_byte_array *)blocks->value;
    if (arr->length != 32768)
    	return 1;
    	
    /* Check valid data element */
    nbt_tag *data = nbt_find_tag_by_name("Data", root);
    if ((data == NULL) || (data->type != TAG_BYTE_ARRAY))
    	return 1;
    
    arr = (nbt_byte_array *)data->value;
    if (arr->length != 16384)
    	return 1;
    
    /* Check valid SkyLight element */
    nbt_tag *sky = nbt_find_tag_by_name("SkyLight", root);
    if ((sky == NULL) || (sky->type != TAG_BYTE_ARRAY))
    	return 1;
    
    arr = (nbt_byte_array *)sky->value;
    if (arr->length != 16384)
    	return 1;
    
    /* Check valid BlockLight element */
    nbt_tag *light = nbt_find_tag_by_name("BlockLight", root);
    if ((light == NULL) || (light->type != TAG_BYTE_ARRAY))
    	return 1;
    
    arr = (nbt_byte_array *)light->value;
    if (arr->length != 16384)
    	return 1;
    	
    /* Check valid HeightMap element */
    nbt_tag *hmap = nbt_find_tag_by_name("HeightMap", root);
    if ((hmap == NULL) || (hmap->type != TAG_BYTE_ARRAY))
    	return 1;
    
    arr = (nbt_byte_array *)hmap->value;
    if (arr->length != 256)
    	return 1;
    	
    return 0;
}

int check_base32 (char * str) {
	int l = strlen(str);
	int i;
	
	for (i = 0; i < l; i++) {
		if (!(str[i] >= '0' && str[i] <= '9') && !(str[i] >= 'a' && str[i] <= 'z')) {
			return 0;
		}
	}
	
	return 1;
}

int check_chunkname (char * str) {
	int l = strlen(str);
	int i, j;
	
	if (str[0] != 'c' || str[1] != '.') {
		return 0;
	}
	
	j = 0;
	
	for (i = 2; i < l; i++) {
		if (j == 2) {
			break;
		}
		
		if (str[i] == '.') {
			j++;
			continue;
		}
		
		if (!(str[i] >= '0' && str[i] <= '9') && !(str[i] >= 'a' && str[i] <= 'z') && str[i] != '-') {
			return 0;
		}
	}
	
	if (str[i] != 'd' || str[i+1] != 'a' || str[i+2] != 't') {
		return 0;
	}
	
	return 1;
}

void chunk_to_coords (char * str, struct chunk_coords * cc) {
	int l = strlen(str);
	int i = 2;
	
	cc->x = 0;
	cc->y = 0;
	
	int negate = 0;
	for (; i < l; i++) {
		if (str[i] == '.') {
			break;
		}
		
		if (str[i] == '-') {
			negate = 1;
		}		
		else if (str[i] >= '0' && str[i] <= '9') {
			cc->x += (str[i] - '0');
		}
		else if (str[i] >= 'a' && str[i] <= 'z') {
			cc->x += (str[i] - 'a' + 10);
		}
		
		cc->x *= 36;
	}
	
	if (negate) {
		cc->x *= -1;
		negate = 0;
	}
	
	for (i = i + 1; i < l; i++) {
		if (str[i] == '.') {
			break;
		}
		
		if (str[i] == '-') {
			negate = 1;
		}		
		else if (str[i] >= '0' && str[i] <= '9') {
			cc->y += (str[i] - '0');
		}
		else if (str[i] >= 'a' && str[i] <= 'z') {
			cc->y += (str[i] - 'a' + 10);
		}
		
		cc->y *= 36;
	}
	
	if (negate) {
		cc->y *= -1;
		negate = 0;
	}
	
	cc->x /= 36;
	cc->y /= 36;
}

int update_chunk (char * file, char * name, pf_type pf, struct options * opt, void * pf_opt) {
	nbt_file *nf;
	int i;

    if (nbt_init(&nf) != NBT_OK)
    {
        fprintf(stderr, "nbt_init(): Failed. Get some RAM\n");
        return EXIT_FAILURE;
    }
    
    if (nbt_parse(nf, file) != NBT_OK)
    {
        fprintf(stderr, "Malformed chunk detected: %s\n", name);
        return EXIT_FAILURE;
    }
    
    if (valid_chunk(nf->root) == 0)
    {
        /*nbt_byte_array *arr;
        nbt_byte_array *dat;
        nbt_tag *blocks = nbt_find_tag_by_name("Blocks", nbt_find_tag_by_name("Level", nf->root));
        nbt_tag *data = nbt_find_tag_by_name("Data", nbt_find_tag_by_name("Level", nf->root));*/

        /* 'blocks' cannot be NULL as we already confirmed a valid file */
        /*assert(blocks != NULL);
        assert(data != NULL);

        arr = nbt_cast_byte_array(blocks);
        dat = nbt_cast_byte_array(data);
        
        if (opt->set & OPT_VV) {
	        fprintf(stderr, "Generating %d (rounds %d, min %d, max %d, size %d)\n", ore_id, opt->rounds, opt->min_depth, opt->max_depth, opt->size);
	    }
        
        for (i = 0; i < opt->rounds; i++) {
        	if (opt->set & OPT_VV) {
	        	fprintf(stderr, "Generating round %d...\n", i);
	        }
        	gen_ore(arr, dat, ore_id, opt);
        }*/
        
        pf(nf, opt, pf_opt);
        
   		nbt_write(nf, file);
    	nbt_free(nf);
	}
	else {
		fprintf(stderr, "Malformed chunk detected: %s\n", name);
        nbt_free(nf);

        return EXIT_FAILURE;
	}
	
	return EXIT_SUCCESS;
}

int update_all_chunks (char *path, pf_type pf, struct options * opt, void * pf_opt) {
	DIR *tld;
	DIR *sld;
	DIR *fld;
	struct dirent *tld_str;
	struct dirent *sld_str;
	struct dirent *fld_str;
	
	fprintf(stderr, "Processing...\n");
	
	tld = opendir(path);
	if (!tld) {
		fprintf(stderr, "Could not open directory %s - %s\n", path, sys_errlist[errno]);
		return EXIT_FAILURE;
	}
	
	//fprintf(stderr, "Scanning %s...\n", path);
	
	// Scan world directory
	while (1) {

		tld_str = readdir(tld);
		if (!tld_str) {
			break;
		}
		
		char pathbuf[255];
		sprintf(pathbuf, "%s/%s", path, tld_str->d_name);

		struct stat statbuf;
		if (stat(pathbuf, &statbuf) != 0) {
			fprintf(stderr, "Stat failed on %s\n", pathbuf);
			closedir(tld);
			return EXIT_FAILURE;
		}

		if (S_ISDIR(statbuf.st_mode) && check_base32(tld_str->d_name)) {
			
			sld = opendir(pathbuf);
			if (!sld) {
				fprintf(stderr, "Could not open directory %s\n", pathbuf);
				closedir(tld);
				return EXIT_FAILURE;
			}
			
			//fprintf(stderr, "Scanning %s...\n", pathbuf);
			
			// Scan first-tier chunk directories
			while (1) {
				sld_str = readdir(sld);
				if (!sld_str) {
					break;
				}
			
				sprintf(pathbuf, "%s/%s/%s", path, tld_str->d_name, sld_str->d_name);
				
				if (stat(pathbuf, &statbuf) != 0) {
					fprintf(stderr, "Stat failed on %s\n", pathbuf);
					closedir(sld);
					closedir(tld);
					return EXIT_FAILURE;
				}
				
				if (S_ISDIR(statbuf.st_mode) && check_base32(sld_str->d_name)) {
					
					fld = opendir(pathbuf);
					if (!fld) {
						fprintf(stderr, "Could not open directory %s\n", pathbuf);
						closedir(sld);
						closedir(tld);
						return EXIT_FAILURE;
					}
					
					//fprintf(stderr, "Scanning %s...\n", pathbuf);
					
					// Scan second-tier chunk directories
					while (1) {
						fld_str = readdir(fld);
						if (!fld_str) {
							break;
						}
						
						sprintf(pathbuf, "%s/%s/%s/%s", path, tld_str->d_name, sld_str->d_name, fld_str->d_name);
						
						if (stat(pathbuf, &statbuf) != 0) {
							fprintf(stderr, "Stat failed on %s\n", pathbuf);
							closedir(fld);
							closedir(sld);
							closedir(tld);
							return EXIT_FAILURE;
						}
						
						if (S_ISREG(statbuf.st_mode) && check_chunkname(fld_str->d_name)) {
							if (opt->set & OPT_M_TIME) {
								if (opt->set & OPT_M_AFT) {
									if (statbuf.st_mtime < opt->m_time) {
										continue;
									}
								}
								else if (statbuf.st_mtime > opt->m_time) {
									continue;
								}
							}
							
							// Check optional bounds
							if (opt->set & OPT_BBOX) {
								struct chunk_coords cc;
								chunk_to_coords(fld_str->d_name, &cc);
								
								if (cc.x < opt->x1 || cc.x > opt->x2 || cc.y < opt->y1 || cc.y > opt->y2) {
									continue;
								}
							}
							
							if (opt->set & OPT_V) {
								fprintf(stderr, "Updating chunk %s/%s/%s...\n", tld_str->d_name, sld_str->d_name, fld_str->d_name);
							}
							
							char cname[255];
							sprintf(cname, "%s/%s/%s", tld_str->d_name, sld_str->d_name, fld_str->d_name);
							
							if (update_chunk(pathbuf, cname, pf, opt, pf_opt) != EXIT_SUCCESS) {
								if (opt->set & OPT_V) {
									fprintf(stderr, "Failed to update chunk.  Skipping...\n");
								}
								/*closedir(fld);
								closedir(sld);
								closedir(tld);
								return EXIT_FAILURE;*/
							}
						}
					}
					
					closedir(fld);
				}
				
			}
			
			closedir(sld);
		}
		
	}	
	
	closedir(tld);
	
	fprintf(stderr, "World updated\n");
	
	return EXIT_SUCCESS;
}
