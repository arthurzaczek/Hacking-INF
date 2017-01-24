using System;

public class Programm {
    public static void Main() {
		// read name
		Console.WriteLine("Name: ");
		var name = Console.ReadLine().Trim();
		
		// say hello 
		Console.WriteLine("Hello " + name + "!");
    }
}