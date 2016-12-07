#include <stdio.h>
#include <stdlib.h>
/*

    THIS FILE MIGHT CONTAIN ERRORS FOR TESTING PURPOSES

*/

static double bmi_calc(double height, double weight){
    if (height<=0.) return -1;
    if (height<1.60) return -1;
    if (weight<=0.) return -1;
    if (weight<40.) return -1;

    return (weight)/(height*height);

}

static int bmi_class(double bmi){
    if (bmi<0) return -1;

    if(bmi<=16. ){
        return 10;
    }else if( bmi <17.){
        return 11;
    }else if (bmi <18.5){
        return 12;
    }else if (bmi<25){
        return 20;
    }else if(bmi<30){
        return 30;
    }else if (bmi<35){
        return 40;
    }else if (bmi<40){
        return 41;
    }else if(bmi>=40){
        return 42;
    }

    return -1;

}
static void bmi_message(int bmi_class){
    switch(bmi_class){
        case -1: printf("ungueltig"); break;
        case 10: printf("stark untergewichtig");break;
        case 11: printf("maessig untergewichtig");break;
        case 12: printf("leicht untergewichtig");break;

        case 20: printf("normalgewichtig");break;

        case 30: printf("praeadipoes");break;

        case 40: printf("adipoes Grad I");break;
        case 41: printf("adipoes Grad II");break;
        case 42: printf("adipoes Grad III");break;

        default: printf("Fehler");break;

    }

}


#include "use_this_main.c"
