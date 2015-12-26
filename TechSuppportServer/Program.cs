using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net.Mail; //Note smtp is a pain in the butt, but support may be added in the future

//TODO Make this threaded.
//TODO learn threading

namespace TechSuppportServer
{
	class MainClass
	{
		static int port = 4000; //The port for the server to listen on
		static string ipadress = "0.0.0.0"; //Allow connects from any ip adress.
		public static string input; //A string to hold input in
		public static int ticketid = 0; //The id of the ticket
		static IPAddress adress = IPAddress.Parse (ipadress); //Parse any ip adress that enters the server

		public static void Main (string[] args)
		{
			while (true) { //Run this loop forever
				connect ();//Start the connect method
			}
		}

		public static void connect(){
			int check; //check the varible that gets sent to the server to tell the server weather to send or recive information
			try{
				//Start of network creation
				var tcplister = new TcpListener(adress, port); //A new listern for any adress and on port 4000
				tcplister.Start(); //Start the listener
				Socket socket = tcplister.AcceptSocket(); //Accept the new connection
				//end of network creation

				//Creation for network reading
				var networkStream = new NetworkStream (socket); //Start a new network stream
				var streamreader = new System.IO.StreamReader (networkStream); //I know I should have my stream reader and writer here, but I got lazy. Both of them need reader, so it goes here.
				//End of network reading creation.

				//Start of reading in the number used to determin the serveres action
				input =  streamreader.ReadLine(); //Read the number from the client to see what the server needs to do (send or recive data)
				check = Convert.ToInt32 (input); //Convert the string to a int
				Console.WriteLine(check);//Write the check out to the console (used for debug left atm because server side stuff needs to be update, and this helps with that.)
				//End of server action check.

				if(check == 1){
					newTicket(networkStream);//Call the new ticket method

				}else if (check == 2){ //If the number is 2, then the client is reading data.
					reviewTicket(networkStream); //Call the reviewTicket method
				}
				socket.Close();//Close the socket
				tcplister.Stop(); //Stop reading in data
			} 
			catch(Exception e){ //Error checking
				Console.WriteLine(e); //Show error message, super helpful (thank you for Scott for making me learn to use this AWESOME tool)
			}
		}

		public static void newTicket(NetworkStream networkstream){
			//This is the list of output variables
			var streamreader = new System.IO.StreamReader (networkstream); //I know I should have my stream reader and writer here, but I got lazy. Both of them need reader, so it goes here.
			string name;
			string ticketName;
			string Ticketmesage;
			string TicketImportance;
			string emailaddress;
			string filePath; //The filepath for the server to read data from
			//End of output varibles

			Console.Clear ();//Cleasr the screen
			name = streamreader.ReadLine(); //Read in the name
			Console.WriteLine("Name:" + name);//Write out the name
			ticketName = streamreader.ReadLine();//Repeate
			Console.WriteLine("ticket: " + ticketName);//Repeate
			Ticketmesage = streamreader.ReadLine();//Repeate
			Console.WriteLine("Message: " + Ticketmesage);//Repeate
			TicketImportance = streamreader.ReadLine();//Repeate
			Console.WriteLine("Importance: " +TicketImportance);//Repeate
			emailaddress = streamreader.ReadLine();//Repeate
			filePath = @"/home/(username)/tickets/"+ticketName+".txt";//Set the file path for the ticket to be saved as *Note you must run on linux, and you must have afolder called tickets
			input = System.IO.File.ReadAllText(@"/home/(username)/ticketstotal.txt");//Read in the current ticket id number. (You need a file called ticketstotal and it needs to have a number in it)
			Console.WriteLine(input);//WRite the ticket id out to the console.
			ticketid = Convert.ToInt32 (input);//Convert the input to a int
			string[] data ={name,ticketName,Ticketmesage,TicketImportance, emailaddress, DateTime.Now.ToShortDateString(), input}; //Write out all the data into an array, plus the date the ticket was created.
			System.IO.File.WriteAllLines(filePath,data);//Write the data to disk
			ticketid++;//Increase the ticket id
			System.IO.File.WriteAllText(@"/home/(username)/ticketstotal.txt", Convert.ToString(ticketid));//Write the new ticket out out to file.
		}

		public static void reviewTicket(NetworkStream networkStream){
			//Variable creation
			string filePath; //The filepath for the server to read data from
			var streamwrite = new System.IO.StreamWriter(networkStream);//See, I only use the writer here, and only here, so I want to only make it here.
			var streamreader = new System.IO.StreamReader (networkStream); //I know I should have my stream reader and writer here, but I got lazy. Both of them need reader, so it goes here.
			string filename;
			string[] data;
			int i = 0;//Write out all the data to the screen. Exists for debugging and cus im lazy
			// end of variable creation

			//Read in the data from the file.
			Console.Clear ();//Clear the console.
			filename = streamreader.ReadLine(); //Read in a string from the client (this is the ticket name)
			filePath = @"/home/(username)/tickets/"+filename+".txt"; //Store the file path
			data = System.IO.File.ReadAllLines(filePath); //Read in all the data into an array
			//End of reading in the data

			while(i < 7){//This is used in debugging to make sure all of the variables are passed properly. left in because it can be helpful to see the last call
				Console.WriteLine(data[i]);
				i++;
			}

			//Start writing out the data
			//Now write out all of your data to the network, for the client to read in.
			streamwrite.WriteLine(data[0]);
			streamwrite.Flush();
			streamwrite.WriteLine(data[1]);
			streamwrite.Flush();
			streamwrite.WriteLine(data[2]);
			streamwrite.Flush();
			streamwrite.WriteLine(data[3]);
			streamwrite.Flush();
			streamwrite.WriteLine(data[4]);
			streamwrite.Flush();
			streamwrite.WriteLine(data[5]);
			streamwrite.Flush();
			streamwrite.WriteLine(data[6]);
			streamwrite.Close();
			//stop writing out the data.

		}
	}
}