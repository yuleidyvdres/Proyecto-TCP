using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Cliente_CSharp
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
            IPEndPoint direccion = new IPEndPoint(IPAddress.Parse("192.168.1.101"), 1234);

            byte[] texto_rec;
            byte[] texto_env;
            byte[] d_ip;
            byte[] mssg;
            string texto;

            try
            {
                socket.Connect(direccion);

                Console.WriteLine("Conectado con exito!");
                string ip_dir = "";
                var host = Dns.GetHostEntry(Dns.GetHostName());//Obtiene el nombre del host y apartir de alli su lista de direcciones
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip_dir = ip.ToString();//Dirección IP del local host
                    }
                }
                texto_env = Encoding.UTF8.GetBytes(ip_dir);
                socket.Send(texto_env, 0, texto_env.Length, 0);//Envio dirección IP al servidor

                string nombre = Dns.GetHostName();//Obtengo el nombre del local host
                d_ip = Encoding.UTF8.GetBytes(nombre);
                socket.Send(d_ip, 0, d_ip.Length, 0);//Envio nombre del local host al servidor

                while (true)
                {
                    texto_rec = new byte[255];
                    int recibir = socket.Receive(texto_rec, 0, texto_rec.Length, 0);//Recibo el "texto" del servidor
                    Array.Resize(ref texto_rec, recibir);
                    Console.WriteLine("Servidor dice: " + Encoding.UTF8.GetString(texto_rec));//Muestro texto que envió el servidor

                    Console.Write("Cliente dice: ");
                    texto = Console.ReadLine();
                    mssg = Encoding.UTF8.GetBytes(texto);//Se covierte "texto" de string a bytes
                    socket.Send(mssg, 0, mssg.Length, 0);//Envia texto al servidor
                    //Console.WriteLine("\n");

                    if (String.Compare(texto, "over") == 0)
                    {
                        socket.Close();//Cierra la conexión
                        Console.WriteLine();
                        Console.WriteLine("Conexion cliente cerrada");
                        break;
                    }
                }
                /*string finish = "over";
                over = Encoding.UTF8.GetBytes(finish);
                socket.Send(over, 0, over.Length, 0);//Envio fin de conexión

                socket.Close();
                Console.WriteLine();
                Console.WriteLine("Conexion cliente cerrada");*/
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
            }
            Console.ReadLine();
        }
    }
}
