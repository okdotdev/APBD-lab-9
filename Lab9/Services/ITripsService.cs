using Lab9.Models;

namespace Lab9.Services;

public interface ITripsService
{
    Task<List<Trip>> GetTrips(int page, int pageSize);
    Task<bool> AddClientToTrip(int idTrip, int idClient);

}