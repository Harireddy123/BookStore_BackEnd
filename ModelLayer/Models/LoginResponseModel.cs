using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class LoginResponseModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
