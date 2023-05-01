using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Chat_App.MVVM.Code
{
    class User
    {
        private string username;
        private Database database;
        private string ip_address;
        private string get_ip;

        // Konstruktor
        public User(string username)
        {
            this.username = username;
            database = new Database();
        }

        public string IPAddress
        {
            get { return get_ip; }
        }

        // Metoda pobierająca dane o adresie IP użytkownika i zapisująca je w bazie danych
        public void Pv()
        {
            database.OpenConnection();
            WebClient client = new WebClient();
            string responseString = client.DownloadString("https://ipinfo.io/");
            JObject data = JObject.Parse(responseString);

            ip_address = data["ip"].ToString();
            string hostname = data["hostname"].ToString();
            string city = data["city"].ToString();
            string region = data["region"].ToString();
            string country = data["country"].ToString();
            string loc = data["loc"].ToString();
            string org = data["org"].ToString();
            string postal = data["postal"].ToString();
            string timezone = data["timezone"].ToString();

            try
            {
                // Sprawdź, czy rekord już istnieje w bazie danych
                string query = "SELECT * FROM ip_info WHERE username = @username AND ip_address = @ip_address";
                MySqlCommand command = new MySqlCommand(query, database.GetConnection());
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@ip_address", ip_address);
                MySqlDataReader reader = command.ExecuteReader();

                // Jeśli rekord nie istnieje, dodaj go do bazy danych
                if (!reader.HasRows)
                {
                    reader.Close();
                    query = "INSERT INTO ip_info (username, ip_address, hostname, city, region, country, loc, org, postal, timezone) VALUES (@username, @ip_address, @hostname, @city, @region, @country, @loc, @org, @postal, @timezone)";
                    command = new MySqlCommand(query, database.GetConnection());
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@ip_address", ip_address);
                    command.Parameters.AddWithValue("@hostname", hostname);
                    command.Parameters.AddWithValue("@city", city);
                    command.Parameters.AddWithValue("@region", region);
                    command.Parameters.AddWithValue("@country", country);
                    command.Parameters.AddWithValue("@loc", loc);
                    command.Parameters.AddWithValue("@org", org);
                    command.Parameters.AddWithValue("@postal", postal);
                    command.Parameters.AddWithValue("@timezone", timezone);
                    command.ExecuteNonQuery();

                    System.Console.WriteLine("Rekord został dodany do bazy danych.");
                }
            }
            catch (MySqlException e)
            {
                // Obsługa błędów połączenia z bazą danych
                System.Console.WriteLine("Wystąpił błąd połączenia z bazą danych: " + e.Message);
            }
            finally
            {
                database.CloseConnection();
            }
        }

        // Metoda pobierająca adres IP użytkownika
        public void GetAddressIP()
        {
            WebClient client = new WebClient();
            string responseString = client.DownloadString("https://ipinfo.io/");
            JObject data = JObject.Parse(responseString);
            get_ip = data["ip"].ToString();
        }
    }
}
