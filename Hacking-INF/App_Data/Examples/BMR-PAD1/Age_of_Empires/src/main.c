#include <stdio.h>
#include <stdlib.h>


int main()
{
    int age;
    printf("Your age: ");
    scanf("%d", &age);

    printf("%%d:   ~%d~\n", age);
    printf("%%+d:  ~%+d~\n", age);
    printf("%% d:  ~% d~\n", age);
    printf("%%5d:  ~%5d~\n", age);
    printf("%%05d: ~%05d~\n", age);
    printf("%%-5d: ~%-5d~\n", age);
    printf("%%#x:  ~%#x~\n", age);

    return 0;
}
