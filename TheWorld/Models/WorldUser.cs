using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TheWorld.Models
{
    public class WorldUser : IdentityUser
    {
        public int FirstTrip { get; set; }
    }  
}
