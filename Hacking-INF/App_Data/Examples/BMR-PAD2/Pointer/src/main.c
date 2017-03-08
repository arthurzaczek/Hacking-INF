#include <stdio.h>
#include <stdlib.h>

void print(int i, float f, char c);

void change(int *i, float *f, char *c)
{
    *i = 17;
    *f = 37.2;
    *c = 'b';
}

int main()
{
    int i = 42;
    float f = 4.2;
    char c = 'x';

    printf("Initial: \n");
    print(i, f, c);

    change(&i, &f, &c);
    printf("Changed: \n");
    print(i, f, c);
}


#include "our_main.c"
