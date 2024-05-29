using Lab9.Models;
using Lab9.Repositories;

namespace Lab9.Services;

public class TripsService : ITripsService
{
    private ITripsRepository _tripsRepository;

    public TripsService(ITripsRepository tripsRepository)
    {
        _tripsRepository = tripsRepository;
    }

    public async Task<List<Trip>> GetTrips(int page, int pageSize = 10)
    {
        var result = await _tripsRepository.GetTrips(page, pageSize);
        return result;
    }
    
    public async Task<bool> AddClientToTrip(int idTrip, int idClient)
    {
        var result = await _tripsRepository.AddClientToTrip(idTrip, idClient);
        return result;
    }
}