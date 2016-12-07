int main()
{
    double weight=0., height=0., bmi=0.;
    printf("Koerpergewicht [kg]: ");
    scanf("%lf", &weight);
    printf("Koerpergroesse [m]: ");
    scanf("%lf", &height);
    printf("m=%.2fkg l=%.2fm -> BMI=%.2f (", weight, height, bmi=bmi_calc(height, weight));
    bmi_message(bmi_class(bmi));
    printf(")\n");
    return 0;

}
