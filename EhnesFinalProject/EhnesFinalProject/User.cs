using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhnesFinalProject
{
    [Serializable]
    class User
    {

        string password, username;

        public User()
        {
        }

        public User(string username, string password)
        {

            this.username = username;
            this.password = password;

        }

        public string Username
        {

            get
            {
                return this.username;
            }

            set
            {
                this.username = value;
            }

        }

        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
            }
        }

    }
}
