﻿using Auth.API.Domain.Models;

namespace Auth.API.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(CoffeeShopUser user, IEnumerable<string> roles);
        //string GenerateEmailConfirmationToken(string email);
    }
}
