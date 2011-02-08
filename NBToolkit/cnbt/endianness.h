#ifndef ENDIANNESS_H
#define ENDIANNESS_H

#include <stdint.h>

#define L_ENDIAN 0
#define B_ENDIAN    1

int get_endianness();

uint64_t swpd(double d);
double uswpd(uint64_t d);

float swapf(float);
double swapd(double);
void swaps(uint16_t *x);
void swapi(uint32_t *x);
void swapl(uint64_t *x);

#endif
