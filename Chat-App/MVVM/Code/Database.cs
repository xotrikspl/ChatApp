using MySql.Data.MySqlClient;
using System;

namespace Chat_App.MVVM.Code
{
    class Database
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        // Konstruktor
        public Database()
        {
            server = "localhost";
            database = "chat";
            uid = "root";
            password = "mleczko1";
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";
            connection = new MySqlConnection(connectionString);
        }

        // Otwórz połączenie z bazą danych
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                // Obsługa błędów połączenia
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Nie można połączyć z serwerem.");
                        break;

                    case 1045:
                        Console.WriteLine("Nieprawidłowa nazwa użytkownika lub hasło.");
                        break;
                }
                return false;
            }
        }

        // Zamknij połączenie z bazą danych
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Wykonaj zapytanie SQL
        public MySqlDataReader ExecuteQuery(string query)
        {
            if (OpenConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = null;
                try
                {
                    dataReader = command.ExecuteReader();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return dataReader;
            }
            else
            {
                return null;
            }
        }

        // Getter dla pola connection
        public MySqlConnection GetConnection()
        {
            return connection;
        }
    }
}
