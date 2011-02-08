#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>

#include "nbt.h"

#define INDEXAT(x,y,z) ((y) + ((z) * 128 + (x) * 128 * 16))

void print_usage(const char *program_name)
{
    fprintf(stderr, "Usage: %s <chunk file> <old id> <new id> "
                    "[min. height] [max. height]\n",
                    program_name);

    return;
}

int valid_chunk(nbt_tag *nbtroot)
{
    /* Check valid root element */
    nbt_tag *root = nbt_find_tag_by_name("Level", nbtroot);

    if ((root != NULL) && 
        (strcmp(root->name, "Level") == 0) && (root->type == TAG_COMPOUND))
    {
        nbt_tag *blocks = nbt_find_tag_by_name("Blocks", root);

        if ((blocks != NULL) && (blocks->type == TAG_BYTE_ARRAY))
        {
            nbt_byte_array *arr = (nbt_byte_array *)blocks->value;

            if (arr->length == 32768)
                return 0; /* Valid at last. */
        }
    }

    return 1;
}

int main(int argc, char **argv)
{
    /* argv[1] == chunk file
     * argv[2] == old id
     * argv[3] == new id 
     * argv[4] == opt: min height
     * argv[5] == opt: max height
     */

    int old_id = -1; /* No valid minecraft ID */
    int new_id = -1; /* ditto */

    int min = -1;
    int max = -1;

    /* Validate arguments */
    if ((argc < 4) || ((argc > 4) && (argc < 6)))
    {
        print_usage(argv[0]);
        
        return EXIT_FAILURE;
    }
    else
    {
        /* Parse the parameters */
        if (strcmp(argv[2], "a") == 0)
            old_id = -1;
        else if (sscanf(argv[2], "%d", &old_id))
            old_id %= 256; /* Make sure it's 8-bit sized */
        else
        {
            fprintf(stderr, "Old ID has to be either 'a' or a number\n");

            return EXIT_FAILURE;
        }

        if (sscanf(argv[3], "%d", &new_id) != 1)
        {
            fprintf(stderr, "New ID has to be a number\n");

            return EXIT_FAILURE;
        }
        else
            new_id %= 256; /* Make sure it's 8-bit sized */


        /* Do we have limits? */
        if (argc == 6)
        {
            if ((sscanf(argv[4], "%d", &min) != 1) ||
                (sscanf(argv[5], "%d", &max) != 1))
            {
                fprintf(stderr, "Couldn't make sense out of the parameters\n");

                return EXIT_FAILURE;
            }
            else
            {
                if (min > max)
                {
                    fprintf(stderr, "Minimum height can not be higher than "
                                    "maximum height\n");

                    return EXIT_FAILURE;
                }

                /* Make sure they're between 0 and 128 */
                min %= 129;
                max %= 129;
            }
        }
        else
        {
            min = 0;
            max = 128;
        }


        nbt_file *nf;

        if (nbt_init(&nf) != NBT_OK)
        {
            fprintf(stderr, "nbt_init(): Failed. Get some RAM\n");

            return EXIT_FAILURE;
        }

        /* Try parsing */
        if (nbt_parse(nf, argv[1]) != NBT_OK)
        {
            fprintf(stderr, "nbt_parse(): Failed - RAM?\n");

            return EXIT_FAILURE;
        }

        /* Validate chunk file */
        if (valid_chunk(nf->root) == 0)
        {
            /* Do the processing */
            int x, y, z;

            nbt_byte_array *arr;
            nbt_tag *blocks = nbt_find_tag_by_name("Blocks", 
                                    nbt_find_tag_by_name("Level", nf->root));

            /* 'blocks' cannot be NULL as we already confirmed a valid file */
            assert(blocks != NULL);
            /* Now that we have this out of our way, let's get to it */

            arr = nbt_cast_byte_array(blocks);

            printf("DEBUG: Replacing %d with %d\n", old_id, new_id);

            if (min >= 0)
                printf("DEBUG: Only between %d and %d height\n", min, max);


            for (y = min; y < max; ++y)
            {
                for (x = 0; x < 16; ++x)
                {
                    for (z = 0; z < 16; ++z)
                    {
                        int index = INDEXAT(x, y, z);
                        
                        if ((old_id == -1) || (arr->content[index] == old_id))
                            arr->content[index] = new_id;
                    }
                }
            }

            nbt_write(nf, argv[0]);
            nbt_free(nf);

        }
        else
        {
            fprintf(stderr, "File \"%s\" is not a valid chunk\n", argv[1]);
            nbt_free(nf);

            return EXIT_FAILURE;
        }
    }

    return EXIT_SUCCESS;
}
