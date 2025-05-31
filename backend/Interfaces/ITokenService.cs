using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.src.ApplicationUser; // EKLEYİN


namespace backend.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}