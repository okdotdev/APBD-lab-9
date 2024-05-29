using Lab9.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab9.Repositories;

public class ClientsRepository : IClientsRepository
{
    private readonly AppDbContext _appDbContext;

    public ClientsRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public async Task<bool> DeleteClient(int idClient)
    {
        Client? client = await (from clients in _appDbContext.Clients
            where clients.IdClient == idClient
            select clients).FirstOrDefaultAsync();
        if (client == null)
        {
            return false;
        }

        bool hasTrips = await (from clientTrips in _appDbContext.ClientTrips
            where clientTrips.IdClient == idClient
            select clientTrips).AnyAsync();
        if (hasTrips)
        {
            return false;
        }

        _appDbContext.Clients.Remove(client);
        await _appDbContext.SaveChangesAsync();
        return true;
    }
}