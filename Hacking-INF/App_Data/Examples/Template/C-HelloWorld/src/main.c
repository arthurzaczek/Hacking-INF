#include <stdio.h>
#include <stdlib.h>

int main()
{
    char name[128];
    printf("Name: ");
    scanf("%127s", name);
    printf("Hello %s!\n", name);

    return 0;
}
