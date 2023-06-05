using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Model.Login
{
    public class LoginPo
    {

        public string Phone { get; set; }
        public string Email { get; set; }
        public string EmailAuthenticationCode { get; set; }
        public string AuthenticationCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return "Phone:" + Phone + ", Email:" + Email + ", EmailAuthenticationCode:" + EmailAuthenticationCode
                + "AuthenticationCode:" + AuthenticationCode + ", FirstName:" + FirstName + ", LastName:" + LastName
                + "Password:" + Password;
        }
    }
}
