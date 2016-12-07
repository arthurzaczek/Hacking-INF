#include <stdio.h>
#include <stdlib.h>
#define NVAL 8

int main()
{
    int A[5][10] = {{0}};
    int i, j, x, y;
    int hist[16]={0};
    int input;

    while (1) {
        /* Ausgabe */
        printf("+----------+\n");
        for (i = 0; i < 5; i++) {
            printf("|");
            for (j = 0; j < 10; j++) {
                if (A[i][j] == 0)
                    printf(".");
                else
                    printf("%d", A[i][j]);
            }
            printf("|\n");
        }
        printf("+----------+\n");

        /* Eingabe */
        printf("x: ");
        scanf("%d", &x);
        if (x < 0) break;
        printf("y: ");
        scanf("%d", &y);
        if (y < 0) break;

        if (x == 0 || x > 10 || y == 0 || y > 5)
            printf("Ungueltige Eingabe!\n");
        else if(A[y - 1][x - 1]==NVAL-1)
            printf("Gesaettigt!\n");
        else
            A[y - 1][x - 1]++;
    };

   /* for (i = 0; i < 5; i++)
        for (j = 0; j < 10; j++)
            sum += A[i][j];

    printf("%d Treffer.\n", sum);
*/
 printf("Histogramm anzeigen?: ");
 scanf("%d",&input);
 if (input==1){
 printf("Histogramm:\n");
    for (i = 0; i < 5; i++)
        for (j = 0; j < 10; j++)
            hist[A[i][j]]++;

    for (i = 0; i < NVAL; i++){
        printf("%2d| ", i);
        for(j=0; j<hist[i]; j++)
            printf("*");
        printf(" (%d)\n",hist[i]);
    }
}
    return 0;
}
