using Lab9.Models;

namespace Lab9.Repositories;

public interface ITripsRepository
{
    Task<List<Trip>> GetTrips(int page, int pageSize);
    Task<bool> AddClientToTrip(int idTrip, int idClient);

}