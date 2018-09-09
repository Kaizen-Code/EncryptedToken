using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MvcWebApplication.Models
{
    public class Client
    {
        public static string ClientFile = HttpContext.Current.Server.MapPath("~/App_Data/Clients.json");
        public static int Counter;

        static Client()
        {
            var list = GetClients();
            if (list.Count > 0)
                Counter = list.Max(c => c.ID);
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public static List<Client> GetClients()
        {
            List<Client> clients = new List<Client>();
            if (File.Exists(ClientFile))
            {
                // File exists..
                string content = File.ReadAllText(ClientFile);
                // Deserialize the objects 
                clients = JsonConvert.DeserializeObject<List<Client>>(content);

                // Returns the clients, either empty list or containing the Client(s).
                return clients;
            }
            else
            {
                // Create the file 
                File.Create(ClientFile).Close();
                // Write data to it; [] means an array, 
                // List<Client> would throw error if [] is not wrapping text
                File.WriteAllText(ClientFile, "[]");

                // Re run the function
                GetClients();
            }

            return clients;
        }
    }

}