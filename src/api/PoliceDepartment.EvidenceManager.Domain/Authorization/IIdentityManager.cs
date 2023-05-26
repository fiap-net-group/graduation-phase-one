﻿namespace PoliceDepartment.EvidenceManager.Domain.Authorization
{
    public interface IIdentityManager
    {
        Task<AccessTokenModel> AuthenticateAsync(string email, string password);
    }
}