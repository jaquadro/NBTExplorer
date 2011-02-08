/*
 * -----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" (Revision 42):
 * <webmaster@flippeh.de> wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think
 * this stuff is worth it, you can buy me a beer in return. Lukas Niederbremer.
 * -----------------------------------------------------------------------------
 *
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <getopt.h>
#include <time.h>
#include <math.h>

#include "nbt.h"

void display_version();
void display_help();
void print_usage(FILE *, const char *);
int valid_level(nbt_tag *root);

char *time_to_string(long);

#define SHOW -2
#define OFF -1

struct coord
{
    int x;
    int y;
    int z;
};

int main(int argc, char **argv)
{
    static int opt_verbose = 0;
    static int opt_short   = 0;

    long opt_time = OFF;
    int opt_lp = OFF;
    int opt_snow  = OFF;
    int opt_spawn = OFF;

    struct coord spawn_coords;

    /* Command line processing */
    for (;;)
    {
        static struct option long_options[] =
        {
            { "spawn",     optional_argument, NULL, 's' },
            { "version",   no_argument,       NULL, 'v' },
            { "help",      no_argument,       NULL, 'h' },
            { "verbose",   no_argument,       &opt_verbose, 1 },
            { "short",     no_argument,       &opt_short, 1 },
            { "snow",      optional_argument, NULL, 'S' },
            { "time",      optional_argument, NULL, 't' },
            { "last-played", no_argument,     NULL, 'l' },
            { NULL,        no_argument,       NULL, 0 }
        };

        int option_index = 0;
        int opt;


        opt = getopt_long(argc, argv, "s::vhS::t::la", long_options, &option_index);

        if (opt == -1)
            break;

        switch (opt)
        {
            case 0:
                if (long_options[option_index].flag != 0)
                    break;

                break;

            case 'v':
                display_version();

                return EXIT_SUCCESS;

            case 'h':
                print_usage(stdout, argv[0]);
                printf("\n");
                display_help();

                return EXIT_SUCCESS;

            case 's':
                opt_spawn = 0;

                if (optarg == NULL)
                    opt_spawn = SHOW;
                else
                {
                    opt_spawn = 0;

                    /* Try parsing the string */
                    if (sscanf(optarg, "%d,%d,%d", &spawn_coords.x, 
                                &spawn_coords.y, 
                                &spawn_coords.z) < 3)
                    {
                        fprintf(stderr, "Could not parse string \"%s\"\n", 
                                optarg);

                        opt_spawn = -1;

                        break;
                    }

                }

                break;

            case 'S':
                if (optarg != NULL)
                {
                    if   ((strcmp(optarg, "1") == 0)
                            || (strcmp(optarg, "true") == 0)
                            || (strcmp(optarg, "yes") == 0))
                        opt_snow = 1;
                    else
                        opt_snow = 0;
                }
                else
                    opt_snow = SHOW;

                break;

            case 'l':
                opt_lp = SHOW;

                break;

            case 't':
                if (optarg != NULL)
                {
                    /* We want to change the time */
                    int h, m, s;

                    if (sscanf(optarg, "%02d:%02d:%02d", &h, &m, &s) < 3)
                    {
                        fprintf(stderr, "Could not parse string \"%s\"\n",
                                optarg);

                    }
                    else
                    {
                        opt_time = (h-6) * 3600;
                        opt_time += m * 60;
                        opt_time += s;

                        opt_time = ceil(opt_time / 3.6);

                        if (opt_time < 0)
                            opt_time = 24000 - abs(opt_time);
                    }                    
                }
                else
                    opt_time = SHOW;

                break;

            case 'a':
                opt_snow = opt_snow == OFF ? SHOW : opt_snow;
                opt_time = opt_time == OFF ? SHOW : opt_time;
                opt_lp   = opt_lp   == OFF ? SHOW : opt_lp;

                break;
        }
    }   

    if (opt_verbose)
    {
        printf("Settings...\n");
        printf(" ~> Changing spawn:\t%s\n", 
                opt_spawn < 0 ? "Unchanged"
                : (opt_spawn ? "View"
                    : "Manual"));

        printf(" ~> Snow:\t\t%s\n",
                opt_snow < 0 ? "Unchanged"
                : (opt_snow ? "On"
                    : "Off"));

        printf(" ~> Set time:\t\t%s\n",
                opt_time >= 0 ? time_to_string(opt_time)
                : "Unchanged");

        printf("\n");
    }

    /* Make sure we still have some arguments (at least one) left */
    if (optind < argc)
    {
        nbt_file *nbt;

        char *file = argv[optind];

        if (nbt_init(&nbt) != NBT_OK)
        {
            fprintf(stderr, "FATAL: Couldn't initialize NBT structure... ");
            fprintf(stderr, "do you have like... no RAM?\n");

            return EXIT_FAILURE;
        }

        if (opt_verbose) printf("Trying to parse \"%s\"... ", file);

        if (nbt_parse(nbt, file) != NBT_OK)
        {
            if (opt_verbose) printf("failure\n");

            fprintf(stderr, "FATAL: Couldn't parse NBT file\n");

            return EXIT_FAILURE;
        }
        else
            if (opt_verbose)
                printf("success!\n");

        if (opt_verbose) printf("Checking file... ");

        /* Validate file */
        if (valid_level(nbt->root) != 0)
        {
            if (opt_verbose) printf("failure\n");

            fprintf(stderr, "FATAL: This is NOT a valid level file\n");

            return EXIT_FAILURE;
        }
        else
        {
            /* data will not be NULL, no check needed */
            nbt_tag *data = nbt_find_tag_by_name("Data", nbt->root);

            if (opt_verbose) printf("success!\n\n");

            if (opt_snow != OFF)
            {
                nbt_tag *snow = nbt_find_tag_by_name("SnowCovered", data);

                if (!opt_short)
                    printf("Snow is ");

                if (opt_snow == SHOW)
                    printf("%s\n", *(char *)snow->value == 1 ? "enabled"
                            : "disabled");
                else
                {
                    nbt_set_byte(snow, opt_snow);

                    printf("%s\n", opt_snow ? "enabled"
                            : "disabled");
                }

            }

            if (opt_time == SHOW || opt_time >= 0)
            {
                nbt_tag *time = nbt_find_tag_by_name("Time", data);

                if (!opt_short) printf("Current ingame time: ");

                if (opt_time == SHOW)
                    printf("%s\n", time_to_string(*(long *)time->value));
                else
                {
                    /* Change */
                    nbt_set_long(time, opt_time);

                    printf("%s\n", time_to_string(opt_time));
                }
            }

            if (opt_lp != OFF)
            {
                nbt_tag *lp = nbt_find_tag_by_name("LastPlayed", data);
                time_t t = *((size_t *)lp->value) / 1000;

                if (!opt_short) printf("Last played: ");

                printf("%s", ctime(&t));
            }

            if (opt_spawn != OFF)
            {
                nbt_tag *sx = nbt_find_tag_by_name("SpawnX", data);
                nbt_tag *sy = nbt_find_tag_by_name("SpawnY", data);
                nbt_tag *sz = nbt_find_tag_by_name("SpawnZ", data);

                int32_t *x = nbt_cast_int(sx);
                int32_t *y = nbt_cast_int(sy);
                int32_t *z = nbt_cast_int(sz);

                if ((x == NULL) || (y == NULL) || (z == NULL))
                    fprintf(stderr, "Could't cast X, Y or Z tags\n");

                if ((sz == NULL) || (sy == NULL) || (sx == NULL))
                    fprintf(stderr, "Couldn't find X, Y or Z tags.\n");
                else
                {
                    if (opt_spawn != SHOW)
                    {
                        nbt_set_int(sx, spawn_coords.x);
                        nbt_set_int(sy, spawn_coords.y);
                        nbt_set_int(sz, spawn_coords.z);
                    }                   

                    x = nbt_cast_int(sx);
                    y = nbt_cast_int(sy);
                    z = nbt_cast_int(sz);

                    if (!opt_short)
                        printf("The spawn is at ");

                    printf("X%d, Y%d, Z%d\n", *x, *y, *z);
                }
            }
        }

        nbt_write(nbt, file);
        nbt_free(nbt);
    }
    else
        print_usage(stderr, argv[0]);

    return EXIT_SUCCESS;
}

void display_version()
{
    printf("DatLevel (level.dat) v0.2\n");
    return;
}

void display_help()
{
    printf("  -v, --version            display program version\n");
    printf("  -h, --help               display this help\n");
    printf("      --verbose            be verbose\n");
    printf("      --short              show less\n");
    printf("  -s, --spawn[=x,y,z]      display or set spawn coords\n");
    printf("  -S, --snow[=1|0]         view or toggle snow\n");
    printf("  -t, --time[=HH:MM:SS]    display or set time\n");
    printf("  -l, --last-played        display date of last playing\n");
    printf("  -a                       show everything\n");

    printf("\n");

    return;
}

void print_usage(FILE *stream, const char *progname)
{
    fprintf(stream, "Usage: %s [OPTION..] <file>\n", progname);

    return;
}

int valid_level(nbt_tag *root)
{
    if (root->type == TAG_COMPOUND)
    {
        nbt_tag *data = nbt_find_tag_by_name("Data", root);

        if (data != NULL)
        {
            if (data->type == TAG_COMPOUND)
            {
                /* ENABLE IF YOU ONLY WANT SINGLEPLAYER MAPS
                 * nbt_tag *player = nbt_find_tag_by_name("Player", data);

                 if (player != NULL)*/
                return 0;
            }
        }
    }

    return 1;
}

char *time_to_string(long time)
{
    static char ret[9];

    long real_time = (((time + 6000) % 24000) * 3.6);

    int s = real_time % 60;
    int m = real_time / 60;
    int h = m / 60;

    m = m % 60;

    snprintf(ret, 9, "%02d:%02d:%02d", h, m, s);

    return ret;
}
