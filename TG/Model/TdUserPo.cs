﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Model
{
    public class TdUserPo
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        private string name;
        public string Name { get { return FirstName + " " + LastName; } set { name = value; } }

        public string Flag { get; set; }
        
    }
}
