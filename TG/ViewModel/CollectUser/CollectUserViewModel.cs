using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Model;

namespace TG.ViewModel.CollectUser
{
    public class CollectUserViewModel : BaseViewModel
    {

        #region

        private List<DataGridData> userList = new List<DataGridData>();

        private bool inner24Hour = false;
        public bool Inner24Hour
        {
            get { return inner24Hour; }
            set
            {
                inner24Hour = value;

                this.OnPropertyChanged();
            }
        }

        private bool inner7Day = false;
        public bool Inner7Day
        {
            get { return inner7Day; }
            set
            {
                inner7Day = value;

                this.OnPropertyChanged();
            }
        }

        #endregion

        #region 命令

        #endregion

    }


    public class DataGridData : BaseViewModel
    {
        private string name;
        public string Name { get { return name; } set { name = value; this.OnPropertyChanged(); } }
    }
}
