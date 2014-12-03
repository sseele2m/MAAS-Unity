import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class TcpClient {

	/**
	 * @param args
	 * @throws IOException 
	 */
	public static void main(String[] args) throws IOException {
		try
		{
			String host = args[0];
			int portNumber = 0;
			try
			{
				portNumber = Integer.parseInt(args[1]);
			}
			catch(NumberFormatException e)
			{
				System.err.println("Argument " + args[1] + " must be an integer.");
				System.exit(1);
			}
			Socket serverSocket = new Socket(host, portNumber);
			PrintWriter out = new PrintWriter(serverSocket.getOutputStream(), true);
			BufferedReader in = new BufferedReader(new InputStreamReader(serverSocket.getInputStream()));
			
			BufferedReader commandLineReader = new BufferedReader(new InputStreamReader(System.in));
			
			System.out.println("Select project:\n1: Rockin\n2: Kiva");
			System.out.print(">");
			int id = 0;
			while(id < 1 || id > 2)
			{
				id = Integer.parseInt(commandLineReader.readLine());
				if(id == 1)
				{
					out.println("Rockin");
				}
				else if(id == 2)
				{
					out.println("Kiva");
				}
				else
				{
					System.out.println("Type '1' for Rockin or '2' for Kiva");
					System.out.print(">");
				}
			}
			
			/*System.out.println("Connected to server " + host + " on port " + portNumber);
			String msg = in.readLine();
			System.out.println("Received: " + msg);*/
		
			System.out.println("Enter message:");
			System.out.print(">");
			while(true)
			{
				if(commandLineReader.ready())
				{
					String str = commandLineReader.readLine();
					if(str.equals("exit"))
					{
						break;
					}
					System.out.println(">Sending: " + str);
					out.println(str);
					System.out.print(">");
				}
				
				if(in.ready())
				{
					String strIn = in.readLine();
					System.out.println("Received: " + strIn);
					System.out.print(">");
				}
			}
			
			serverSocket.close();
			System.out.println("Shutting down");
			System.exit(0);
		}
		catch(Exception e)
		{
			System.out.println(e.getMessage());
			System.exit(1);
		}
	}

}
