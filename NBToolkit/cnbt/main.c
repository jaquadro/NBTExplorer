/*
 * -----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" (Revision 42):
 * <webmaster@flippeh.de> wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think
 * this stuff is worth it, you can buy me a beer in return. Lukas Niederbremer.
 * -----------------------------------------------------------------------------
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <getopt.h>

#include "endianness.h"
#include "nbt.h"

static int opt_dupe = 0;

void dump_nbt(char *filename);

int main(int argc, char **argv)
{
    int c;
    static int opt_dump = 0;

    //opterr = 0;

    for (;;)
    {
        static struct option long_options[] =
        {
            {"dump",    no_argument, &opt_dump, 1},
            {"version", no_argument, NULL, 'v'},
            {"copy",    no_argument, &opt_dupe, 1},
            {NULL,      no_argument, NULL, 0}
        };

        int option_index = 0;

        c = getopt_long(argc, argv, "dv", long_options, &option_index);

        if (c == -1)
            break;

        switch (c)
        {
            case 0:
                if (long_options[option_index].flag != 0)
                    break;

                break;

            case 'v':
                printf("nbttool 1.0 (%s, %s)\n", __DATE__, __TIME__);

                return EXIT_SUCCESS;

            case '?':
                break;
        }
    }

    if (optind < argc)
    {
        /* There is more in argv */

        if (opt_dump)
            dump_nbt(argv[optind]); 
    }


    return 0;
}

void dump_nbt(char *filename)
{
    int i;
    nbt_file *nbt = NULL;
    nbt_tag *ba;
    nbt_tag *p;
    nbt_list *l;

    /* Enough parameters given? */
    if (nbt_init(&nbt) != NBT_OK)
    {
        fprintf(stderr, "NBT_Init(): Failure initializing\n");

        return;
    }

    /* Try parsing */
    if (nbt_parse(nbt, filename) != NBT_OK)
    {
        fprintf(stderr, "NBT_Parse(): Error\n");

        return;
    }

    ba = nbt_find_tag_by_name("listTest (long)", nbt->root);

    if (ba != NULL)
    {
        l = nbt_cast_list(ba);
        int64_t **c = (int64_t **)l->content;

        for (i = 0; i < l->length; ++i)
        {
            if (*(c[i]) == 13)
            {
                int64_t *testval = malloc(sizeof(int64_t));
                *testval = 50;


                //nbt_remove_list_item(c[i], ba);
                nbt_add_list_item(testval, ba);
                break;
            }
        }

        nbt_print_tag(ba, 0);
    }

    nbt_print_tag(nbt->root, 0);
    nbt_free(nbt);

    return;
}
