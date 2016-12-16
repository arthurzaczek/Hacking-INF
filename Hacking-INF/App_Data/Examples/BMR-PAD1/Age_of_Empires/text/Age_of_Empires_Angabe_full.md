Age_of_Empires
=======================

Schreiben Sie ein C-Programm, das nach der Eingabe Ihres Alters Ihr Alter wieder ausgibt.

## Aufgabe 

Hierfür soll das Alter unterschiedlich formatiert werden:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
%d
%+d
% d
%5d
%05d
%-5d
%#x
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Beispiele: 
(Text in rot = Benutzereingabe)

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Your age: \stdin{37}
%d:   ~37~
%+d:  ~+37~
% d:  ~ 37~
%5d:  ~   37~
%05d: ~00037~
%-5d: ~37   ~
%#x:  ~0x25~
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Hinweise
* Vergessen Sie bitte nicht das `\n` am Ende von `printf()` Anweisungen! 
* Die `~` dient der "Begrenzung" der Ausgabe.



Method name |  Complexity |  Category |  SLoC
:-|:-:|:-|:-
bmi_class |  10 |  simple, without much risk |  23
bmi_message |  10 |  simple, without much risk |  19
bmi_calc |  5 |  simple, without much risk |  8
main |  1 |  simple, without much risk |  11

Table: Age_of_Empires Komplexität d. Lösung


Table: Age_of_Empires Übersicht Testfälle

Testfall|Beschreibung
---------|------------
001| positives Alter (Beispiel)
002| negatives Alter
003| Eingabe 0
