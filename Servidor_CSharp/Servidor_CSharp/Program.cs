using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Servidor_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            conexion();
        }

        public static void conexion() 
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            /*Se crea el socket indicando:
             -AddressFamily.InterNetwork: El esquema de dirección que utilizará la instancia de socket que en este caso será IP
             versión 4, es decir, el socket esperará una conexión IPV4 de un host remoto.
             -SocketType.Stream: Este tipo de socket comunica con un compañero y necesita una conexión con un host remoto antes 
              de que la comunicación empiece.
             -ProtocolType.Tcp: Indica que el socket soporta el protocolo TCP*/
            IPEndPoint direccion = new IPEndPoint(IPAddress.Parse("192.168.1.104"), 1234); 
            /*Se asigna la dirección IP y el puerto para realizar la conexión por donde el servidor escuchará*/

            string texto;
            byte[] texto_env;
            byte[] texto_rec;
            byte[] dir_ip;
            byte[] finish;

            try
            {
                socket.Bind(direccion);//Asocia la dirección con el socket
                socket.Listen(1);//Escucha los intentos de conexiones entrantes. En este caso el máximo número de conexiones es 1

                Console.WriteLine("Esperando conexion...");
                Socket servidor = socket.Accept();//Acepta un cliente
                Console.WriteLine("Conectado!");
                Console.WriteLine();
                Console.WriteLine();

                dir_ip = new byte[255];
                int rec_ip = servidor.Receive(dir_ip, 0, dir_ip.Length, 0);
                /*Recibe dirección IP del cliente, la cual se guarda en un vector de bytes de logitud constante, la ubicación en
                 dir_ip para almacenar los datos, el número de bytes que va a recibir.*/
                Array.Resize(ref dir_ip, rec_ip);//Asigna el tamaño de "rec_ip" a "dir_ip"
                Console.WriteLine("IP Cliente: " + Encoding.UTF8.GetString(dir_ip));
                //Muestra la dirección IP del cliente, convirtiéndola de bytes a string

                texto_rec = new byte[255];
                int rec = servidor.Receive(texto_rec, 0, texto_rec.Length, 0);
                /*Recibe nombre del cliente, el cual se guarda en un vector de bytes de logitud constante, la ubicación en
                 "texto_rec" para almacenar los datos, el número de bytes que va a recibir.                 */
                Array.Resize(ref texto_rec, rec);//Asigna el tamaño de "rec" a "texto_rec" 
                Console.WriteLine("Nombre Cliente: " + Encoding.UTF8.GetString(texto_rec));
                /*Muestra el nombre del host cliente, convirtiéndolo de bytes a string*/

                Console.WriteLine();
                Console.Write("Servidor dice al cliente: ");
                texto = Console.ReadLine();
                texto_env = Encoding.UTF8.GetBytes(texto);//Se covierte "texto" de string a bytes
                servidor.Send(texto_env, 0, texto_env.Length, 0);//Envia texto al cliente
                Console.WriteLine("\nEnviado exitosamente!");

                finish = new byte[255];
                int end = servidor.Receive(finish, 0, finish.Length, 0);//Recibe la señal del cliente para terminar la conexión
                Array.Resize(ref finish, end);

                if (String.Compare(Encoding.UTF8.GetString(finish), "over")==0)
                {
                    socket.Close();//Cierra la conexión
                    Console.WriteLine();
                    Console.WriteLine("Conexion servidor cerrada");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
            }
            Console.ReadLine();
        }
    }
}
