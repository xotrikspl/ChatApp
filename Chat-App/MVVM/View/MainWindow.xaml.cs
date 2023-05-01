using System;
using System.Windows;
using Chat_App.MVVM.AdditionalWindow;
using MySql.Data.MySqlClient;
using Chat_App.MVVM.Code;

namespace Chat_App
{
    public partial class MainWindow : Window
    {
        // Zmienna bazy danych
        private Database database;

        // Zmienna zapisania loginu użytkownika
        private string loggedInUser;

        public MainWindow()
        {
            InitializeComponent();
            database = new Database();

            // Wyłącznie pól oraz przycisków przed zalogowaniem
            messageButton.IsEnabled = false;
            messageTextBox.IsEnabled = false;
            logoutButton.IsEnabled = false;
            productsButton.IsEnabled = false;
            registerButton.IsEnabled = true;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            database.OpenConnection();
            string query = $"SELECT COUNT(*) FROM users WHERE username = '{username}' AND password = '{password}'";
            MySqlCommand command = new MySqlCommand(query, database.GetConnection());

            int count = Convert.ToInt32(command.ExecuteScalar() ?? 0);

            if (count == 1)
            {
                loggedInUser = username;

                // Włącznie przycisków
                loginButton.IsEnabled = false;
                messageButton.IsEnabled = true;
                messageTextBox.IsEnabled = true;
                logoutButton.IsEnabled = true;
                productsButton.IsEnabled = true;
                registerButton.IsEnabled = false;

                // Pobieranie danych o użytkowniku
                User user = new User(loggedInUser);
                user.Pv();

                // Wyświetlanie wszystkich wiadomości
                query = $"SELECT sender, message FROM messages";
                command = new MySqlCommand(query, database.GetConnection());
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string s = reader.GetString(0);
                    string message = reader.GetString(1);
                    // Wyświetlenie wiadomości w oknie aplikacji
                }

                reader.Close();
            }
            else
            {
                MessageBox.Show("Nieprawidłowa nazwa użytkownika lub hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            database.CloseConnection();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Włączenie przycisków po wylogowaniu.
            loginButton.IsEnabled = true;
            logoutButton.IsEnabled = false;
            messageButton.IsEnabled = false;
            messageTextBox.IsEnabled = false;
            productsButton.IsEnabled = false;
            registerButton.IsEnabled = true;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = messageTextBox.Text;
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            // Zapisywanie wiadomości do bazy danych
            database.OpenConnection();

            string query = $"INSERT INTO messages (sender, message) VALUES ('{loggedInUser}', '{msg}')";
            MySqlCommand command = new MySqlCommand(query, database.GetConnection());
            command.ExecuteNonQuery();
            database.CloseConnection();
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productsWindow = new ProductsWindow();
            productsWindow.Show();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
        }
    }
}
