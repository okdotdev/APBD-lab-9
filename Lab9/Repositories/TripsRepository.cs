using Lab9.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab9.Repositories;

public class TripsRepository : ITripsRepository
{
    private readonly AppDbContext _appDbContext;

    public TripsRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<Trip>> GetTrips(int page, int pageSize)
    {
        List<Trip> result = await (from trip in _appDbContext.Trips
                orderby trip.DateFrom descending
                select trip)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return result;
    }
    
    public async Task<bool> AddClientToTrip(int idTrip, int idClient)
    {
        string? pesel = await (from clients in _appDbContext.Clients
            where clients.IdClient == idClient
            select clients.Pesel).FirstOrDefaultAsync();

        Client? clientExists = await (from clients in _appDbContext.Clients
            where clients.Pesel == pesel
            select clients).FirstOrDefaultAsync();
        if (clientExists == null)
        {
            return false;
        }
        
        bool clientIsRegistered = await (from clientTrips in _appDbContext.ClientTrips
            join clients in _appDbContext.Clients on clientTrips.IdClient equals clients.IdClient
            where clients.Pesel == pesel && clientTrips.IdTrip == idTrip
            select clientTrips).AnyAsync();
        if (clientIsRegistered)
        {
            return false;
        }

        Trip? tripExists = await (from trips in _appDbContext.Trips
            where trips.IdTrip == idTrip
            select trips).FirstOrDefaultAsync();

        if (tripExists == null)
        {
            return false;
        }

        ClientTrip clientTripN = new ClientTrip
        {
            IdClient = idClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = null
        };

        await _appDbContext.ClientTrips.AddAsync(clientTripN);
        await _appDbContext.SaveChangesAsync();

        return true;
    }
}