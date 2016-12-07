CMOSHistogram
=======================

Schreiben Sie ein C-Programm, mit dem man den Beschuss eines CMOS-Sensors mit Photonen simulieren kann. Der Sensor hat eine Auflösung von 5 x 10 Zellen. Jede Zelle registriert das Auftreffen eines Photons und zählt die Gesamtzahl der Treffer. Nach jedem Treffer wird der aktuelle Status ausgegeben. Anschließend frägt das Programm ob ein Histogramm ausgegeben werden soll (`Histogramm anzeigen?: `).



Das Programm frägt solange nach x- und y-Koordinaten (x: von 1 bis 10, y: von 1 bis 5), bis eine der beiden Koordinaten negativ erfasst wird. Koordinaten außerhalb des Sensorbereichs (0 bzw. größer als der rechte/untere Rand) führen zu einer Fehlermeldung (`Ungueltige Eingabe!`).

Nach 7 Treffern geht eine Zelle des Sensors in Sättigung (`Gesaettigt!`), d.h. der Wert wird nicht weiter erhöht.

Die Darstellung des Histogramms erfolgt zeilenweise. In einer Zeile steht zunächst der Wert am CMOS. Der Count wird in Form eines Balken von `*` dargestellt (z.B. Count 5 wird durch `*****` dargestellt). Am Ende des Balkens wird der numerische Wert in Klammern ausgegeben (als Beschriftung).

##Hinweise

- Stellen sie den CMOS mit einem 2-dimensionalen Array dar
- Speichern Sie das Histogramm in einem Array (mit 8 Elementen - für jeden möglichen Wert eines)
- anschließend iterieren Sie über das 2-dimensionale Array das den CMOS darstellt und erhöhen den entsprechenden Wert im Histogramm

##Beispiele: 
(Text in rot = Benutzereingabe)


> ~~~~{}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{1}
> y: \stdin{1}
> +----------+
> |1.........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{2}
> y: \stdin{2}
> +----------+
> |1.........|
> |.1........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{3}
> y: \stdin{5}
> +----------+
> |1.........|
> |.1........|
> |..........|
> |..........|
> |..1.......|
> +----------+
> x: \stdin{4}
> y: \stdin{5}
> +----------+
> |1.........|
> |.1........|
> |..........|
> |..........|
> |..11......|
> +----------+
> x: \stdin{1}
> y: \stdin{2}
> +----------+
> |1.........|
> |11........|
> |..........|
> |..........|
> |..11......|
> +----------+
> x: \stdin{1}
> y: \stdin{2}
> +----------+
> |1.........|
> |21........|
> |..........|
> |..........|
> |..11......|
> +----------+
> x: \stdin{-1}
> Histogramm anzeigen?: \stdin{1}
> Histogramm:
>  0| ********************************************* (45)
>  1| **** (4)
>  2| * (1)
>  3|  (0)
>  4|  (0)
>  5|  (0)
>  6|  (0)
>  7|  (0)
> ~~~~~



> ~~~~{}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{1}
> y: \stdin{1}
> +----------+
> |1.........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{2}
> y: \stdin{2}
> +----------+
> |1.........|
> |.1........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{3}
> y: \stdin{5}
> +----------+
> |1.........|
> |.1........|
> |..........|
> |..........|
> |..1.......|
> +----------+
> x: \stdin{4}
> y: \stdin{5}
> +----------+
> |1.........|
> |.1........|
> |..........|
> |..........|
> |..11......|
> +----------+
> x: \stdin{1}
> y: \stdin{2}
> +----------+
> |1.........|
> |11........|
> |..........|
> |..........|
> |..11......|
> +----------+
> x: \stdin{1}
> y: \stdin{2}
> +----------+
> |1.........|
> |21........|
> |..........|
> |..........|
> |..11......|
> +----------+
> x: \stdin{-1}
> Histogramm anzeigen?: \stdin{0}
> ~~~~~


> ~~~
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{0}
> y: \stdin{0}
> Ungueltige Eingabe!
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{12}
> y: \stdin{15}
> Ungueltige Eingabe!
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{-1}
> Histogramm anzeigen?: \stdin{1}
> Histogramm:
>  0| ************************************************** (50)
>  1|  (0)
>  2|  (0)
>  3|  (0)
>  4|  (0)
>  5|  (0)
>  6|  (0)
>  7|  (0)
> ~~~


> ~~~
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |..........|
> +----------+
> x: \stdin{2}
> 
> y: \stdin{5}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.1........|
> +----------+
> x: \stdin{2}
> y: \stdin{5}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.2........|
> +----------+
> x: \stdin{2}
> y: \stdin{5}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.3........|
> +----------+
> x: \stdin{2}
> y: \stdin{5}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.4........|
> +----------+
> x: \stdin{2}
> y: \stdin{5}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.5........|
> +----------+
> x: \stdin{2}
> y: \stdin{5}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.6........|
> +----------+
> x: \stdin{2}
> y: \stdin{5}
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.7........|
> +----------+
> x: \stdin{2}
> y: \stdin{5}
> Gesaettigt!
> +----------+
> |..........|
> |..........|
> |..........|
> |..........|
> |.7........|
> +----------+
> x: \stdin{-1}
> Histogramm anzeigen?: \stdin{1}
> Histogramm:
>  0| ************************************************* (49)
>  1|  (0)
>  2|  (0)
>  3|  (0)
>  4|  (0)
>  5|  (0)
>  6|  (0)
>  7| * (1)
> ~~~


##Abgabe:

Laden Sie in moodle hoch und benennen Sie die Datei `main.c` *in moodle* um auf `CMOSHistogram_id.c`, wobei `id` ihre Technikum-Kennung ist.

####Beispiel:
`id` lautet `mr31b090`. filename lautet: `CMOSHistogram_mr31b090.c`

