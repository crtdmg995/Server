using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Data.SqlClient;

namespace Console_Server
{
    public class MyServer
    {
        private TcpListener listener;
        private String IPadress;
        private int portNumber;

        public MyServer(String IPadr, int PortN)
        {
            setIPport(IPadr, PortN);
        }

        ~MyServer()
        {
            closeConnect();
        }

        public void StartServer()
        {
            DataTable dtRequestResoult;

            dtRequestResoult = new DataTable();

            try
            {
                listener = new TcpListener(IPAddress.Parse(IPadress), portNumber);
                listener.Start();
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    var client = listener.AcceptTcpClient();
                    var binaryFormatter = new BinaryFormatter();
                    var strRequestText = binaryFormatter.Deserialize(client.GetStream()).ToString();

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////

                    dtRequestResoult = dtExequteSQL(strRequestText);
                    binaryFormatter.Serialize(client.GetStream(), dtRequestResoult);
                    Console.WriteLine(strRequestText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private DataTable dtExequteSQL(string strRequest)
        {
            string strConnectionParametr = @"Data Source=DESKTOP-IM7TKKB\SQLEXPRESS;Initial Catalog=Weather;Integrated Security=True";

            DataTable dtResoultTable;
            SqlConnection sqlConnection1;
            SqlDataReader drDataReader;

            SqlCommand sqlCommand1;

            dtResoultTable = new DataTable();
            sqlCommand1 = new SqlCommand();
            sqlConnection1 = new SqlConnection(strConnectionParametr);

            sqlCommand1.CommandText = strRequest;
            sqlCommand1.Connection = sqlConnection1;

            try
            {
                sqlConnection1.Open();
                drDataReader = sqlCommand1.ExecuteReader();
                dtResoultTable.Load(drDataReader);
                sqlConnection1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return dtResoultTable;
        }

        public void closeConnect()
        {
            listener.Stop();
        }

        public void setIPport(string IPadrr, int portN)
        {
            IPadress = IPadrr;
            portNumber = portN;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MyServer DataBaseServer;
            DataBaseServer = new MyServer("127.0.0.1", 2200);
            DataBaseServer.StartServer();
        }
    }
}




