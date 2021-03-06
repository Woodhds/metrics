﻿using metrics.Identity.Client.Abstractions;

namespace metrics.Identity.Client
{
    public class UserTokenKeyProvider : IUserTokenKeyProvider
    {
        public string GetKey(int userId)
        {
            return userId.ToString();
        }
    }
}