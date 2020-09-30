using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo.Models.Configuration
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
    }
}
