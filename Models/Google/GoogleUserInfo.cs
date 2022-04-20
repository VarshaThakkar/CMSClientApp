using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMSClientApp.Models.Google
{
    public class GoogleUserInfo
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Locale { get; set; }
        public string Name { get; set; }
        public string ProviderUserId { get; set; }
    }
}
