#include "oniguruma.h"

ONIG_EXTERN
regex_t *onigwrap_create(char *pattern, int len, int ignoreCase);

ONIG_EXTERN
void onigwrap_region_free(OnigRegion *region);

ONIG_EXTERN
void onigwrap_free(regex_t *reg);

ONIG_EXTERN
int onigwrap_index_in(regex_t *reg, char *charPtr, int offset, int length);

ONIG_EXTERN
OnigRegion *onigwrap_search(regex_t *reg, char *charPtr, int offset, int length);

ONIG_EXTERN
int onigwrap_pos(OnigRegion *region, int nth);

ONIG_EXTERN
int onigwrap_len(OnigRegion *region, int nth);

