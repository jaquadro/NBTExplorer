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
#include <zlib.h>
#include <string.h>

#include "endianness.h"
#include "nbt.h"

/* Initialization subroutine(s) */
int nbt_init(nbt_file **nbt)
{
    if ((*nbt = malloc(sizeof(nbt_file))) == NULL)
        return NBT_EMEM;

    (*nbt)->root = NULL;

    return NBT_OK;
}

/* Parser */
int nbt_parse(nbt_file *nbt, const char *filename)
{
	int code = 0;
	
    if ((nbt->fp = gzopen(filename, "rb")) == Z_NULL)
        return NBT_EGZ;

    nbt->root = malloc(sizeof(nbt_tag));
    if (nbt->root == NULL)
        return NBT_EMEM;

    code = nbt_read_tag(nbt, &(nbt->root));

    gzclose(nbt->fp);
    
    if (code < 0)
    	return code;

    if (nbt->root == NULL)
        return NBT_ERR;

    return NBT_OK;
}

int nbt_read_tag(nbt_file *nbt, nbt_tag **parent)
{
    nbt_type type = 0;
    int code = 0;

    /* Read the type */
    if (gzread(nbt->fp, &type, 1) == -1)
    	return NBT_EGZ;

    (*parent)->type = type;
    (*parent)->name = NULL;
    (*parent)->value = NULL;

    if (type != TAG_END) /* TAG_END has no name */
        code = nbt_read_string(nbt, &((*parent)->name));
        
    /* negative value indicates error */
    if (code < 0)
    	return code;

    code = nbt_read(nbt, type, &((*parent)->value));
    
    if (code < 0)
    	return code;

    return type;
}

int nbt_read(nbt_file *nbt, nbt_type type, void **parent)
{
	int code = 0;
	
    switch (type)
    {
        case TAG_END:
            break; 

        case TAG_BYTE:
            code = nbt_read_byte(nbt, (char **)parent);

            break;

        case TAG_SHORT:
            code = nbt_read_short(nbt, (int16_t **)parent);

            break;

        case TAG_INT:
            code = nbt_read_int(nbt, (int32_t **)parent);

            break;

        case TAG_LONG:
            code = nbt_read_long(nbt, (int64_t **)parent);

            break;

        case TAG_FLOAT:
            code = nbt_read_float(nbt, (float **)parent);

            break;

        case TAG_DOUBLE:
            code = nbt_read_double(nbt, (double **)parent);

            break;

        case TAG_STRING:
            ;; /* to make it shut up about the variable declaration */
            char *string = NULL;
            
            code = nbt_read_string(nbt, &string);
            *parent = string;

            break;

        case TAG_BYTE_ARRAY:
            ;; /* ... */
            unsigned char *bytestring;
            int32_t len = nbt_read_byte_array(nbt, &bytestring);
            
            if (len < 0) {
            	code = len;
            	break;
            }
            
            nbt_byte_array *t = malloc(sizeof(nbt_byte_array));
            t->length = len;
            t->content = bytestring;

            *parent = t;

            break;

        case TAG_LIST:
            ;; 
            char type;
            void **target;
            int32_t length = nbt_read_list(nbt, &type, &target);
            
            if (length < 0) {
            	code = length;
            	break;
            }

            nbt_list *l = malloc(sizeof(nbt_list));
            l->length = length;
            l->type = type;
            l->content = target;

            *parent = l;

            break;

        case TAG_COMPOUND:
            ;;
            nbt_compound *c = malloc(sizeof(nbt_compound));
            nbt_tag **tags = NULL;

            int32_t lc = nbt_read_compound(nbt, &tags);
            
            if (lc < 0) {
            	code = lc;
            	break;
            }

            c->tags = tags;
            c->length = lc;

            *parent = c;
    }
    
    if (code < 0)
    	return code;
    
    return type; /* Use to abort looping in TAG_Read_compound on TAG_END */
}

int nbt_read_byte(nbt_file *nbt, char **out)
{
    char t;

    if (gzread(nbt->fp, &t, sizeof(t)) == -1)
    	return NBT_EGZ;

    *out = malloc(sizeof(char));
    memcpy(*out, &t, sizeof(char));

    return 0;
}

int nbt_read_short(nbt_file *nbt, int16_t **out)
{
    int16_t t;

    if (gzread(nbt->fp, &t, sizeof(t)) == -1)
    	return NBT_EGZ;
    	
    if (get_endianness() == L_ENDIAN)
        swaps((uint16_t *)&t);

    *out = malloc(sizeof(int16_t));
    memcpy(*out, &t, sizeof(int16_t));

    
    return 0;
}

int nbt_read_int(nbt_file *nbt, int32_t **out)
{
    int32_t t;

    if (gzread(nbt->fp, &t, sizeof(t)) == -1)
    	return NBT_EGZ;
    	
    if (get_endianness() == L_ENDIAN)
        swapi((uint32_t *)&t);
    
    *out = malloc(sizeof(int32_t));
    memcpy(*out, &t, sizeof(int32_t));

    return 0;
}

int nbt_read_long(nbt_file *nbt, int64_t **out)
{
    int64_t t;

    if (gzread(nbt->fp, &t, sizeof(t)) == -1)
    	return NBT_EGZ;
    	
    if (get_endianness() == L_ENDIAN)
        swapl((uint64_t *)&t);

    *out = malloc(sizeof(int64_t));
    memcpy(*out, &t, sizeof(int64_t));

    return 0;
}

int nbt_read_float(nbt_file *nbt, float **out)
{
    float t;

    if (gzread(nbt->fp, &t, sizeof(t)) == -1)
    	return NBT_EGZ;
    	
    if (get_endianness() == L_ENDIAN)
        t = swapf(t);

    *out = malloc(sizeof(float));
    memcpy(*out, &t, sizeof(float));

    return 0;
}

int nbt_read_double(nbt_file *nbt, double **out)
{
    double t;

    if (gzread(nbt->fp, &t, sizeof(t)) == -1)
    	return NBT_EGZ;
    	
    if (get_endianness() == L_ENDIAN)
        t = swapd(t);

    *out = malloc(sizeof(double));
    memcpy(*out, &t, sizeof(double));

    return 0;
}

int nbt_read_byte_array(nbt_file *nbt, unsigned char **out)
{
    int32_t len;

    if (gzread(nbt->fp, &len, sizeof(len)) == -1)
    	return NBT_EGZ;
    	
    if (get_endianness() == L_ENDIAN)
        swapi((uint32_t *)&len);

	if (len < 0)
		return NBT_ERR;
		
    *out = malloc(len);
    if (gzread(nbt->fp, *out, len) == -1) {
    	free(*out);
    	return NBT_EGZ;
    }
    
    return len;
}

int nbt_read_string(nbt_file *nbt, char **out)
{
    int16_t len;

    if (gzread(nbt->fp, &len, sizeof(len)) == -1)
    	return NBT_EGZ;
    	
    if (get_endianness() == L_ENDIAN)
        swaps((uint16_t *)&len);
        
    if (len < 0)
    	return NBT_ERR;

    *out = malloc(len + 1);
    memset(*out, 0, len + 1);
    if (gzread(nbt->fp, *out, len) == -1) {
    	free(*out);
    	return NBT_EGZ;
    }

    return len;
}

int32_t nbt_read_list(nbt_file *nbt, char *type_out, void ***target)
{
    char type;
    int32_t len;
    int i;

    if (gzread(nbt->fp, &type, 1) == -1)
    	return NBT_EGZ;
    	
    *type_out = type;
    
    if (type > TAG_MAX)
    	return NBT_ERR;

    if (gzread(nbt->fp, &len, sizeof(len)) == -1)
    	return NBT_EGZ;

    if (get_endianness() == L_ENDIAN)
        swapi((uint32_t *)&len);

	if (len < 0)
		return NBT_ERR;

    *target = malloc(len * sizeof(void *));

    for (i = 0; i < len; ++i)
        if (nbt_read(nbt, type, &((*target)[i])) == -1) {
        	free(*target);
        	return NBT_EGZ;
        }

    return len;
}

int32_t nbt_read_compound(nbt_file *nbt, nbt_tag ***listptr)
{
    int32_t i;

    *listptr = malloc(sizeof(nbt_tag *)); 

    for (i = 0;; ++i)
    {
        (*listptr)[i] = malloc(sizeof(nbt_tag));
        nbt_type last = nbt_read_tag(nbt, &((*listptr)[i]));

        *listptr = realloc(*listptr, sizeof(nbt_tag *) * (i+2));

        if (last == TAG_END)
        {
            //(*listptr)[++i] = NULL;
            free((*listptr)[i]); /* This is an ugly, UGLY hack, let's remove 
                                    this ASAP! */

            break;
        }
    }

    return i;
}

/* Cleanup subroutines */

int nbt_free(nbt_file *nbt)
{
    if (nbt->root != NULL)
        nbt_free_tag(nbt->root);

    free(nbt);

    return NBT_OK;
}

int nbt_free_tag(nbt_tag *t)
{
    free(t->name);
    nbt_free_type(t->type, t->value);
    free(t);

    return 0;
}

int nbt_free_type(nbt_type type, void *payload)
{
    if (payload == NULL)
        return 0;

    switch (type)
    {
        case TAG_BYTE:
        case TAG_SHORT:
        case TAG_INT:
        case TAG_LONG:
        case TAG_FLOAT:
        case TAG_DOUBLE:
        case TAG_STRING:
            free(payload);
            break;
        case TAG_LIST:
            nbt_free_list(payload);
            break;
        case TAG_BYTE_ARRAY:
            nbt_free_byte_array(payload);
            break;
        case TAG_COMPOUND:
            nbt_free_compound(payload);
            break;
        case TAG_END: /* Why the hell? */
            return 1;
    }

    return 0;
}

int nbt_free_list(nbt_list *l)
{
    int i;

    for (i = 0; i < l->length; ++i)
        nbt_free_type(l->type, l->content[i]);

    free(l->content);
    free(l);

    return 0;
}

int nbt_free_byte_array(nbt_byte_array *a)
{
    free(a->content);
    free(a);
    
    return 0;
}

int nbt_free_compound(nbt_compound *c)
{
    int i;

    for (i = 0; i < c->length; ++i)
    {
        free(c->tags[i]->name);
        nbt_free_type(c->tags[i]->type, c->tags[i]->value);
        free(c->tags[i]);
    }
 
    free(c->tags);
    free(c);

    return 0;
}

char *nbt_type_to_string(nbt_type t)
{
    static char *str;

    switch (t)
    {
        case TAG_END:
            str = "TAG_END";
            break;

        case TAG_BYTE:
            str = "TAG_BYTE";
            break;

        case TAG_SHORT:
            str = "TAG_SHORT";
            break;

        case TAG_INT:
            str = "TAG_INT";
            break;

        case TAG_LONG:
            str = "TAG_LONG";
            break;

        case TAG_FLOAT:
            str = "TAG_FLOAT";
            break;

        case TAG_DOUBLE:
            str = "TAG_DOUBLE";
            break;

        case TAG_BYTE_ARRAY:
            str = "TAG_BYTE_ARRAY";
            break;

        case TAG_STRING:
            str = "TAG_STRING";
            break;

        case TAG_LIST:
            str = "TAG_LIST";
            break;

        case TAG_COMPOUND:
            str = "TAG_COMPOUND";
            break;

        default:
            str = "TAG_Unknown";
            break;
    }

    return str;
}

void nbt_print_tag(nbt_tag *t, int indent)
{
    if (t->type == TAG_END)
        return;

    nbt_print_indent(indent);
    printf("%s(\"%s\"): ",
            nbt_type_to_string(t->type),
            t->name);

    nbt_print_value(t->type, t->value, indent);
}

void nbt_print_indent(int lv)
{
    int i = 0;

    for (i = 0; i < lv; ++i)
        printf("   ");

    return;
}
    
void nbt_print_value(nbt_type t, void *v, int n)
{
    int i;
    int indent = n;
    char type = (char)t;

    //printf("%s", indentation);

    switch (type)
    {
        case TAG_BYTE:
            printf("0x%02X (%d)", *((char *)v), *((char *)v));
            break;

        case TAG_SHORT:
            printf("%d", *((int16_t *)v));
            break;

        case TAG_INT:
            ;; long t = *((int32_t *)v);
            printf("%ld", t);
            break;

        case TAG_LONG:
            ;; long long tl = *((int64_t *)v);
            printf("%lld", tl);
            break;

        case TAG_FLOAT:
            printf("%f", *((float *)v));
            break;

        case TAG_DOUBLE:
            printf("%f", *((double *)v));
            break;

        case TAG_STRING:
            printf("\"%s\"", (char *)v);
            break;

        case TAG_BYTE_ARRAY:
            ;;

            nbt_byte_array *arr = (nbt_byte_array *)v;
            nbt_print_byte_array(arr->content, arr->length);
            break;

        case TAG_COMPOUND:
            ;;
            nbt_compound *c = (nbt_compound *)v;
            
            printf("(%d entries) { \n", c->length);
            indent++;

            for (i = 0; i < c->length; ++i)
                nbt_print_tag(c->tags[i], indent);

            nbt_print_indent(--indent);
            printf("}\n");

            break;

        case TAG_LIST:
            ;;
            nbt_list *l = (nbt_list *)v;

            printf("(%d entries) { \n", l->length);
            indent++;

            for (i = 0; i < l->length; ++i)
            {
                nbt_print_indent(indent);

                printf("%s: ", nbt_type_to_string(l->type));
                void **content = l->content;
                nbt_print_value(l->type, content[i], indent);

            }

            nbt_print_indent(--indent);
            printf("}\n");

            break;

        default:
            printf("<not implemented: 0x%02X>", type);
    }

    printf("\n");

    return;
}

void nbt_print_byte_array(unsigned char *ba, int len)
{
    int i;

    printf("(%d entries) [", len);
    for (i = 0; i < len; ++i)
    {
        printf("%02X", ba[i]);

        if (i == (len - 1))
            printf(" ");
        else
            printf(", ");
    }

    printf("]");

    return;
}

int nbt_change_value(nbt_tag *tag, void *val, size_t size)
{
    void *t = malloc(size);
    if (t != NULL)
    {
        nbt_free_type(tag->type, tag->value);
 
        memcpy(t, val, size);
        tag->value = t;

        return 0;
    }

    return 1;
}

int nbt_change_name(nbt_tag *tag, const char *newname)
{
    char *tmp = malloc(strlen(newname) + 1);
    if (tmp != NULL)
    {
        strcpy(tmp, newname);

        free(tag->name);
        tag->name = tmp;

        return 0;
    }

    return 1;
}

void nbt_add_list_item(void *item, nbt_tag *parent)
{
    nbt_list *l = NULL;

    if (parent->type != TAG_LIST)
        return;

    l = nbt_cast_list(parent);
    
    if ((l->content = realloc(l->content, sizeof(void *) * (l->length + 1))) != NULL)
    {
        l->content[l->length++] = item;
    }    

    return;
}
    

nbt_tag *nbt_add_tag(nbt_tag *child, nbt_tag *parent)
{
    nbt_compound *c = NULL;

    if (parent->type != TAG_COMPOUND)
        return NULL;

    c = nbt_cast_compound(parent);

    nbt_tag **tags_temp = NULL;
    tags_temp = realloc(c->tags, sizeof(nbt_tag *) * (c->length + 1));

    if (tags_temp != NULL)
    {
        c->tags = tags_temp;
        c->length++;

        c->tags[c->length - 1] = child;

        return child;
    }

    return NULL;
}

void nbt_remove_tag(nbt_tag *target, nbt_tag *parent)
{
    int i;
    int count = 0;
    nbt_tag **templist = NULL;
    nbt_compound *tmp = (nbt_compound *)parent->value;

    if (parent->type != TAG_COMPOUND)
        return;

    templist = malloc(sizeof(nbt_tag *));

    for (i = 0; i < tmp->length; ++i)
    {
        if (tmp->tags[i] != target)
        {
            templist[count] = tmp->tags[i];
            templist = realloc(templist, sizeof(nbt_tag *) * (count+2));

            ++count;
        }
        else
        {
            nbt_free_tag(tmp->tags[i]);
        }
    }

    free(tmp->tags);
    tmp->tags = templist;
    tmp->length = count;

    return;
}

void nbt_remove_list_item(void *target, nbt_tag *parent)
{
    if (parent->type == TAG_LIST)
    {
        nbt_list *target_list = nbt_cast_list(parent);

        void **new_list = malloc(sizeof(void *) * target_list->length);

        if (new_list != NULL)
        {
            int i, j = 0;

            memset(new_list, 0, sizeof(void *) * target_list->length);

            for (i = 0; i < target_list->length; ++i)
                if (target_list->content[i] != target)
                    new_list[j++] = target_list->content[i];

            nbt_set_list(parent, new_list, j, target_list->type);

            free(new_list);
        }
    }
        
    return;
}

nbt_tag *nbt_find_tag_by_name(const char *needle, nbt_tag *haystack)
{
    if (haystack->type == TAG_COMPOUND)
    {
        nbt_compound *c = (nbt_compound *)haystack->value;
        int i;

        for (i = 0; i < c->length; ++i)
            if (strcmp(c->tags[i]->name, needle) == 0)
                return c->tags[i];
    }

    return NULL;
}

int nbt_write(nbt_file *nbt, const char *filename)
{
    if ((nbt->fp = gzopen(filename, "wb")) == Z_NULL)
        return NBT_EGZ;

    if (nbt->root != NULL)
    {
        int size = nbt_write_tag(nbt, nbt->root);

        gzclose(nbt->fp);

        return size;
    }
   
    return NBT_ERR;
}

int nbt_write_tag(nbt_file *nbt, nbt_tag *tag)
{
    int size = 0;

    size += gzwrite(nbt->fp, &(tag->type), sizeof(char));

    if (tag->type != TAG_END)
    {
        /* Every tag but TAG_END has a name */
        size += nbt_write_string(nbt, tag->name);
        size += nbt_write_value(nbt, tag->type, tag->value);
    }

    return size;
}

int nbt_write_value(nbt_file *nbt, nbt_type t, void *value)
{
    int written = 0;

    switch (t)
    {
        case TAG_END: /* WHY is this even in? */
            break;

        case TAG_BYTE:
            written = nbt_write_byte(nbt, (char *)value);

            break;

        case TAG_SHORT:
            written = nbt_write_short(nbt, (int16_t *)value);

            break;

        case TAG_INT:
            written = nbt_write_int(nbt, (int32_t *)value);

            break;

        case TAG_LONG:
            written = nbt_write_long(nbt, (int64_t *)value);

            break;

        case TAG_FLOAT:
            written = nbt_write_float(nbt, (float *)value);

            break;

        case TAG_DOUBLE:
            written = nbt_write_double(nbt, (double *)value);

            break;

        case TAG_STRING:
            written = nbt_write_string(nbt, (char *)value);

            break;

        case TAG_BYTE_ARRAY:
            written = nbt_write_byte_array(nbt, (nbt_byte_array *)value);

            break;

        case TAG_LIST:
            written = nbt_write_list(nbt, (nbt_list *)value);

            break;

        case TAG_COMPOUND:
            written = nbt_write_compound(nbt, (nbt_compound *)value);

            break;

        default:
            /* Maybe moan about a very unknown tag? Not yet... */
            break;

    }

    return written;
}

int nbt_write_byte(nbt_file *nbt, char *val)
{
    /* bytes, simple enough */
    return gzwrite(nbt->fp, val, sizeof(char));
}

int nbt_write_short(nbt_file *nbt, int16_t *val)
{
    int16_t temp = *val;

    /* Needs swapping first? */
    if (get_endianness() == L_ENDIAN)
        swaps((uint16_t *)&temp);

    return gzwrite(nbt->fp, &temp, sizeof(int16_t));
}

int nbt_write_int(nbt_file *nbt, int32_t *val)
{
    int32_t temp = *val;

    if (get_endianness() == L_ENDIAN)
        swapi((uint32_t *)&temp);

    return gzwrite(nbt->fp, &temp, sizeof(int32_t));
}

int nbt_write_long(nbt_file *nbt, int64_t *val)
{
    int64_t temp = *val;

    if (get_endianness() == L_ENDIAN)
        swapl((uint64_t *)&temp);

    return gzwrite(nbt->fp, &temp, sizeof(int64_t));
}

int nbt_write_float(nbt_file *nbt, float *val)
{
    float temp = *val;

    if (get_endianness() == L_ENDIAN)
        temp = swapf(temp);

    return gzwrite(nbt->fp, &temp, sizeof(float));
}

int nbt_write_double(nbt_file *nbt, double *val)
{
    double temp = *val;

    if (get_endianness() == L_ENDIAN)
        temp = swapd(temp);

    return gzwrite(nbt->fp, &temp, sizeof(double));
}

int nbt_write_string(nbt_file *nbt, char *val)
{
    int size = 0;
    int16_t len = strlen(val);

    /* Write length first */
    size += nbt_write_short(nbt, &len);

    /* Write content */
    size += gzwrite(nbt->fp, val, len);

    return size;
}

int nbt_write_byte_array(nbt_file *nbt, nbt_byte_array *val)
{
    int size = 0;
    
    /* Length first again, then content */
    size += nbt_write_int(nbt, &(val->length));
    size += gzwrite(nbt->fp, val->content, val->length);

    return size;
}

int nbt_write_list(nbt_file *nbt, nbt_list *val)
{
    int i;
    int size = 0;

    /* Write type id first */
    size += nbt_write_byte(nbt, (char *)&(val->type));
    size += nbt_write_int(nbt, &(val->length));

    for (i = 0; i < val->length; ++i)
        size += nbt_write_value(nbt, val->type, val->content[i]);

    return size;    
}

int nbt_write_compound(nbt_file *nbt, nbt_compound *val)
{
    int endtag = 0;
    int i;
    int size = 0;

    for (i = 0; i < val->length; ++i)
        size += nbt_write_tag(nbt, val->tags[i]);

    size += gzwrite(nbt->fp, &endtag, sizeof(char));

    return size;
}

char *nbt_cast_byte(nbt_tag *t)
{
    if (t->type != TAG_BYTE) return NULL;

    return (char *)t->value;
}

int16_t *nbt_cast_short(nbt_tag *t)
{
    if (t->type != TAG_SHORT) return NULL;

    return (int16_t *)t->value;
}

int32_t *nbt_cast_int(nbt_tag *t)
{
    if (t->type != TAG_INT) return NULL;

    return (int32_t *)t->value;
}

int64_t *nbt_cast_long(nbt_tag *t)
{
    if (t->type != TAG_LONG) return NULL;

    return (int64_t *)t->value;
}

float *nbt_cast_float(nbt_tag *t)
{
    if (t->type != TAG_FLOAT) return NULL;

    return (float *)t->value;
}

double *nbt_cast_double(nbt_tag *t)
{
    if (t->type != TAG_DOUBLE) return NULL;

    return (double *)t->value;
}

char *nbt_cast_string(nbt_tag *t)
{
    if (t->type != TAG_STRING) return NULL;

    return (char *)t->value;
}

nbt_list *nbt_cast_list(nbt_tag *t)
{
    if (t->type != TAG_LIST) return NULL;

    return (nbt_list *)t->value;
}

nbt_byte_array *nbt_cast_byte_array(nbt_tag *t)
{
    if (t->type != TAG_BYTE_ARRAY) return NULL;

    return (nbt_byte_array *)t->value;
}

nbt_compound *nbt_cast_compound(nbt_tag *t)
{
    if (t->type != TAG_COMPOUND) return NULL;

    return (nbt_compound *)t->value;
}

int nbt_set_byte(nbt_tag *t, char v)
{
    if (t->type != TAG_BYTE) return 1;

    return nbt_change_value(t, &v, sizeof(v));
}

int nbt_set_short(nbt_tag *t, int16_t v)
{
    if (t->type != TAG_SHORT) return 1;

    return nbt_change_value(t, &v, sizeof(v));
}

int nbt_set_int(nbt_tag *t, int32_t v)
{
    if (t->type != TAG_INT) return 1;

    return nbt_change_value(t, &v, sizeof(v));
}

int nbt_set_long(nbt_tag *t, int64_t v)
{
    if (t->type != TAG_LONG) return 1;

    return nbt_change_value(t, &v, sizeof(v));
}

int nbt_set_float(nbt_tag *t, float v)
{
    if (t->type != TAG_FLOAT) return 1;

    return nbt_change_value(t, &v, sizeof(v));
}

int nbt_set_double(nbt_tag *t, double v)
{
    if (t->type != TAG_DOUBLE) return 1;

    return nbt_change_value(t, &v, sizeof(v));
}

int nbt_set_string(nbt_tag *t, char *v)
{
    if (t->type != TAG_STRING) return 1;

    return nbt_change_value(t, v, strlen(v) + 1);
}

int nbt_set_list(nbt_tag *t, void **v, int len, nbt_type type)
{
    nbt_list temp;

    if (t->type != TAG_LIST) return 1;

    temp.type = type;
    temp.length = len;

    temp.content = malloc(sizeof(void *) * len);
    if (temp.content == NULL)
        return 1;

    memcpy(temp.content, v, sizeof(void *) * len);

    return nbt_change_value(t, &temp, sizeof(temp));
}

int nbt_set_byte_array(nbt_tag *t, unsigned char *v, int len)
{
    nbt_byte_array temp;

    if (t->type != TAG_BYTE_ARRAY) return 1;

    temp.length = len;

    temp.content = malloc(sizeof(unsigned char) * len);
    if (temp.content == NULL)
        return 1;

    memcpy(temp.content, v, len);

    return nbt_change_value(t, &temp, sizeof(temp));
}

int nbt_set_compound(nbt_tag *t, nbt_tag *tags, int len)
{
    nbt_compound temp;

    if (t->type != TAG_COMPOUND) return 1;

    temp.length = len;

    temp.tags = malloc(sizeof(nbt_tag *) * len);
    if (temp.tags == NULL)
        return 1;

    memcpy(temp.tags, tags, len);

    return nbt_change_value(t, &temp, sizeof(temp));
}

int nbt_get_length(nbt_tag *t)
{
    if (t->type == TAG_BYTE_ARRAY)
    {
        nbt_byte_array *ba = nbt_cast_byte_array(t);
        if (ba != NULL)
            return ba->length;
    }
    else if (t->type == TAG_LIST)
    {
        nbt_list *l = nbt_cast_list(t);
        if (l != NULL)
            return l->length;
    }
    else if (t->type == TAG_COMPOUND)
    {
        nbt_compound *c = nbt_cast_compound(t);
        if (c != NULL)
            return c->length;
    }

    return -1;
}

int nbt_get_list_type(nbt_tag *t)
{
    nbt_list *l = NULL;

    if (t->type != TAG_LIST)
        return NBT_ERR;

    l = nbt_cast_list(t);

    return l->type;
}

int nbt_new_tag(nbt_tag **d, nbt_type t, const char *name)
{
    *d = malloc(sizeof(nbt_tag));
    if (*d == NULL)
        return -1;

    (*d)->type  = t;
    (*d)->value = NULL;
    (*d)->name  = NULL;

    if (nbt_change_name(*d, name) != 0)
        return -1;

    return 0;
}

int nbt_new_byte(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_BYTE, name) != 0)
        return -1;

    return nbt_set_byte(*d, 0);
}

int nbt_new_short(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_SHORT, name) != 0)
        return -1;

    return nbt_set_short(*d, 0);
}

int nbt_new_int(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_INT, name) != 0)
        return -1;

    return nbt_set_int(*d, 0);
}

int nbt_new_long(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_LONG, name) != 0)
        return -1;

    return nbt_set_long(*d, 0);
}

int nbt_new_float(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_FLOAT, name) != 0)
        return -1;

    return nbt_set_float(*d, 0.0);
}

int nbt_new_double(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_DOUBLE, name) != 0)
        return -1;

    return nbt_set_double(*d, 0.0);
}

int nbt_new_string(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_STRING, name) != 0)
        return -1;

    return nbt_set_string(*d, "");
}

int nbt_new_byte_array(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_BYTE_ARRAY, name) != 0)
        return -1;

    return nbt_set_byte_array(*d, NULL, 0);
}

int nbt_new_list(nbt_tag **d, const char *name, nbt_type type)
{
    if (nbt_new_tag(d, TAG_LIST, name) != 0)
        return -1;

    return nbt_set_list(*d, NULL, 0, type);
}

int nbt_new_compound(nbt_tag **d, const char *name)
{
    if (nbt_new_tag(d, TAG_COMPOUND, name) != 0)
        return -1;

    return nbt_set_compound(*d, NULL, 0);
}
