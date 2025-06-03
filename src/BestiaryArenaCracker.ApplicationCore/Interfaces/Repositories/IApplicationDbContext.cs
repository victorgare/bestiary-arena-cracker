﻿namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
