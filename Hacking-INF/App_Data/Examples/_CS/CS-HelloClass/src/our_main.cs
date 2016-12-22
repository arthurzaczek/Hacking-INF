
public class Programm {
    public static void Main() {
		// read name
		Console.WriteLine("Name: ");
		var name = Console.ReadLine();
		
		// say hello 
		var g = new Greetings();
		Console.WriteLine(g.SayHello(name));
    }
}