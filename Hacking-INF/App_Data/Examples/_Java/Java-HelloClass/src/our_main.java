
public class Main {
    public static void main(String[] args) throws Exception {
		// read name
		System.out.print("Name: ");
		BufferedReader in = new BufferedReader(
		    new InputStreamReader(System.in));
		String name = in.readLine();
		
		// say hello 
		Greetings g = new Greetings();
		System.out.println(g.sayHello(name));
    }
}