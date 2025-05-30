using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.User; // EKLEYİN


namespace api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(UserEntity user);
    }
}