using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Model;

namespace TG.Client.ViewModel.MoreAccLogin
{
    public class LoginViewModel : BaseViewModel
    {

        #region 字段属性

        private string account;
        public string Account
        {
            get { return account; }
            set { account = value; this.OnPropertyChanged(); }
        }


        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; this.OnPropertyChanged(); }
        }

        private string phonePrefix;

        public string PhonePrefix
        {
            get { return phonePrefix; }
            set
            {
                phonePrefix = value;

                this.OnPropertyChanged();
            }
        }

        private string phone;

        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;

                this.OnPropertyChanged();
            }
        }

        private string verifyCode;

        public string VerifyCode
        {
            get { return verifyCode; }
            set
            {
                verifyCode = value;

                this.OnPropertyChanged();
            }
        }

        #endregion

    }
}
