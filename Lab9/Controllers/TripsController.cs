using Lab9.Models;
using Lab9.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab9.Controllers;

[ApiController]
[Route("/api/trips")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips(int page, int pageSize = 10)
    {
        List<Trip> result = await _tripsService.GetTrips(page, pageSize);
        return Ok(result);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, int idClient)
    {
        bool result = await _tripsService.AddClientToTrip(idTrip, idClient);

        return StatusCode(result ? StatusCodes.Status201Created : StatusCodes.Status204NoContent);
    }
}