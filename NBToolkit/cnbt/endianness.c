#include <stdint.h>

#include "endianness.h"

int get_endianness()
{
    union
    {
        uint32_t i;
        char c[4];
    } t = { 0x01020304 };

    return t.c[0] == 1;
}

uint64_t swpd(double d)
{
    int i;

    uint64_t res;
    unsigned char *dest = (unsigned char *)&res;
    unsigned char *src  = (unsigned char *)&d;

    for (i = 0; i < 8; ++i)
        dest[i] = src[7 - i];

    return res;
}

double uswpd(uint64_t d)
{
    int i;

    double res;
    unsigned char *src  = (unsigned char *)&res;
    unsigned char *dest = (unsigned char *)&d;

    for (i = 0; i < 8; ++i)
        dest[i] = src[7 - i];

    return res;
}

float swapf(float fx)
{
    float rv;

    char *ftc = (char *)&fx;
    char *rtf = (char *)&rv;

    rtf[0] = ftc[3];
    rtf[1] = ftc[2];
    rtf[2] = ftc[1];
    rtf[3] = ftc[0];

    return rv;
}

double swapd(double dx)
{
    double rv;

    char *ftc = (char *)&dx;
    char *rtv = (char *)&rv;

    rtv[0] = ftc[7];
    rtv[1] = ftc[6];
    rtv[2] = ftc[5];
    rtv[3] = ftc[4];
    rtv[4] = ftc[3];
    rtv[5] = ftc[2];
    rtv[6] = ftc[1];
    rtv[7] = ftc[0];

    return rv;
}

void swaps(uint16_t *x)
{
    *x = (*x >> 8) | (*x << 8);
}

void swapi(uint32_t *x)
{
    *x = (*x >> 24) |
         ((*x<<8) & 0x00FF0000) |
         ((*x>>8) & 0x0000FF00) |
         (*x << 24);
}

void swapl(uint64_t *x)
{
    *x = (*x>>56) | 
         ((*x<<40) & 0x00FF000000000000ULL) |
         ((*x<<24) & 0x0000FF0000000000ULL) |
         ((*x<<8)  & 0x000000FF00000000ULL) |
         ((*x>>8)  & 0x00000000FF000000ULL) |
         ((*x>>24) & 0x0000000000FF0000ULL) |
         ((*x>>40) & 0x000000000000FF00ULL) |
         (*x<<56);
}
