using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMSClientApp.Models.Google
{
    public class GoogleApiTokenInfo
    {
        public string email { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string locale { get; set; }
        public string name { get; set; }
        public string sub { get; set; }
    }
}
