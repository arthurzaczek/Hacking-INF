#include <stdio.h>
#include <stdlib.h>

void print(int i, float f, char c);

void change(int *i, float *f, char *c)
{
    /// change i to 17;
    /// change f to 37.2;
    /// change c to 'b';
}

int main()
{
    int i = 42;
    float f = 4.2;
    char c = 'x';

    printf("Initial: \n");
    print(i, f, c);

    /// call change here
    change ....

    printf("Changed: \n");
    print(i, f, c);
}
