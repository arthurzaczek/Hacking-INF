using System;

public class Greetings 
{
	public string SayHello(string name) 
	{
		return "Hello " + name + "!";
	}
}

public class Programm {
    public static void Main() {
		// read name
		Console.WriteLine("Name: ");
		var name = Console.ReadLine().Trim();
		
		// say hello 
		var g = new Greetings();
		Console.WriteLine(g.SayHello(name));
    }
}