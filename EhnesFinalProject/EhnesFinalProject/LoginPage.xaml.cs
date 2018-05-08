using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EhnesFinalProject
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {

        private List<User> userList;

        public LoginPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            userList = new List<User>();

            string path = Directory.GetCurrentDirectory();

            //  Should only return one directory
            string[] userDirectory = Directory.GetDirectories(path, "Users");

            //  Getting files to loop through and load in
            string[] users = Directory.GetFiles(userDirectory[0]);

            //  Going through each file and loading them into a list
            foreach(string user in users)
            {

                FileStream filestream = new FileStream(user, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                User loadedUser = new User();
                loadedUser = (User)bf.Deserialize(filestream);
                userList.Add(loadedUser);
                filestream.Close();

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SelectionPage());
        }

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {

            //  Checking to make sure that fields were filled
            if(string.IsNullOrWhiteSpace(UsernameInput.Text) || string.IsNullOrWhiteSpace(PasswordInput.Text))
            {
                WarningLabel.Content = "Please fill of the fields appropriately";
                WarningLabel.Visibility = Visibility.Visible;
                return;
            }

            string username = UsernameInput.Text;
            string password = PasswordInput.Text;

            string path = Directory.GetCurrentDirectory();

            //  Should only return one directory
            string[] userDirectory = Directory.GetDirectories(path, "Users");
            string userPath = userDirectory[0];

            User newUser = new User(username, password);

            // Saving
            FileStream filestream = new FileStream(userPath + @"\\" + username, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(filestream, newUser);
            filestream.Close();

            this.NavigationService.Navigate(new SelectionPage());

        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            //  Checking to make sure that fields were filled
            if (string.IsNullOrWhiteSpace(UsernameInput.Text) || string.IsNullOrWhiteSpace(PasswordInput.Text))
            {
                WarningLabel.Content = "Please fill of the fields appropriately";
                WarningLabel.Visibility = Visibility.Visible;
                return;
            }

            string username = UsernameInput.Text;
            string password = PasswordInput.Text;
            bool found = false;

            //  Checking to make sure that there is a user with these credentials
            foreach(User user in userList)
            {

                if(username.Equals(user.Username) && password.Equals(user.Password))
                {
                    found = true;
                }

            }

            if (found)
            {
                this.NavigationService.Navigate(new SelectionPage());
            }
            else
            {
                WarningLabel.Content = "No Such User";
                WarningLabel.Visibility = Visibility.Visible;
            }
        }
    }
}
