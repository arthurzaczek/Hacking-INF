Age_of_Empires
=======================

Schreiben Sie ein C-Programm, das nach der Eingabe Ihres Alters Ihr Alter wieder ausgibt.

##Aufgabe 

Hierfür soll das Alter unterschiedlich formatiert werden:

--------------------------------------------
%d
%+d
% d
%5d
%05d
%-5d
%#x
--------------------------------------------

##Beispiele: 
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

##Hinweise
* Vergessen Sie bitte nicht das `\n` am Ende von `printf()` Anweisungen! 
* Die `~` dient der "Begrenzung" der Ausgabe.

