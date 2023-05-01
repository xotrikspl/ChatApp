using Chat_App.MVVM.Code;
using MySql.Data.MySqlClient;
using System.Windows;

namespace Chat_App.MVVM.AdditionalWindow
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Pobranie adresu IP użytkownika
            User user = new User(userName);
            user.GetAddressIP();
            string ip_address = user.IPAddress;

            // Sprawdzenie, czy wszystkie pola zostały wypełnione
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Proszę wypełnić wszystkie pola.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Sprawdzenie, czy hasło i jego potwierdzenie są takie same
            if (password != confirmPassword)
            {
                MessageBox.Show("Podane hasła nie są identyczne.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Sprawdzenie, czy użytkownik zaakceptował warunki korzystania z serwisu
            if (!AgreeToTermsCheckBox.IsChecked.Value)
            {
                MessageBox.Show("Proszę zaakceptować warunki korzystania z serwisu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Sprawdzenie, czy adres email zawiera końcówkę @gmail.com lub @wp.pl
            if (!email.EndsWith("@gmail.com")
                && !email.EndsWith("@wp.pl")
                && !email.EndsWith("@o2.pl")
                && !email.EndsWith("@onet.pl")
                && !email.EndsWith("@op.pl")
                && !email.EndsWith("@vp.pl")
                && !email.EndsWith("@tlen.pl")
                && !email.EndsWith("@spoko.pl")
                && !email.EndsWith("@onet.eu")
                && !email.EndsWith("@hotmail.com")
                && !email.EndsWith("@go2.pl")
                && !email.EndsWith("@prokonto.pl")
                && !email.EndsWith("@poczta.onet.pl")
                && !email.EndsWith("@outlook.com")
                && !email.EndsWith("@icloud.com")
                && !email.EndsWith("@autograf.pl")
                && !email.EndsWith("@vip.onet.pl")
                && !email.EndsWith("@onet.com.pl"))
            {
                MessageBox.Show("Wprowadź poprawny adres email. Dopuszczalne adresy email - @gmail.com, @wp.pl, @o2.pl, @onet.pl, @op.pl, @vp.pl, @tlen.pl, @spoko.pl, @onet.eu, @hotmail.com, @go2.pl, @prokonto.pl, @poczta.onet.pl, @outlook.com@, icloud.com, @autograf.pl, @vip.onet.pl, @onet.com.pl", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Dodanie nowego użytkownika do bazy danych
            // Sprawdzenie, czy użytkownik już istnieje w bazie danych
            try
            {
                Database db = new Database();
                if (db.OpenConnection() == true)
                {
                    string check_user_query = $"SELECT * FROM register_users WHERE username='{userName}' OR email='{email}' OR ip='{ip_address}'";
                    MySqlCommand check_user_cmd = new MySqlCommand(check_user_query, db.GetConnection());
                    MySqlDataReader check_user_reader = check_user_cmd.ExecuteReader();

                    if (check_user_reader.HasRows)
                    {
                        MessageBox.Show("Użytkownik o podanej nazwie użytkownika, adresie e-mail lub adresie IP już istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        check_user_reader.Close();
                        return;
                    }
                    check_user_reader.Close();

                    string register_query = $"INSERT INTO register_users (username, email, password, password_confirm, ip) VALUES ('{userName}', '{email}', '{password}', '{confirmPassword}', '{ip_address}')";
                    string user_query = $"INSERT INTO users (username, email, password, ip) VALUES ('{userName}', '{email}', '{password}', '{ip_address}')";
                    MySqlCommand register_cmd = new MySqlCommand(register_query, db.GetConnection());
                    MySqlCommand user_cmd = new MySqlCommand(user_query, db.GetConnection());

                    register_cmd.ExecuteNonQuery();
                    user_cmd.ExecuteNonQuery();
                    db.CloseConnection();
                    MessageBox.Show("Rejestracja przebiegła pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Nie udało się nawiązać połączenia z bazą danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Błąd podczas dodawania użytkownika do bazy danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
