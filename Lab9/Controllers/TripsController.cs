using Lab9.Models;
using Lab9.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TripsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        Client? client = await _context.GetClients()
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            return NotFound(new { Message = "Client not found" });
        }

        if (client.ClientTrips.Count != 0)
        {
            return BadRequest(new { Message = "Client has assigned trips" });
        }

        _context.GetClients().Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        IOrderedQueryable<Trip> tripsQuery = _context.GetTrips()
            .Include(t => t.Countries)
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.Client)
            .OrderByDescending(t => t.DateFrom);

        int totalTrips = await tripsQuery.CountAsync();
        List<Trip> trips = await tripsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = (int)Math.Ceiling(totalTrips / (double)pageSize),
            Trips = trips.Select(t => new
            {
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                Countries = t.Countries.Select(c => new { c.Name }),
                Clients = t.ClientTrips.Select(ct => new { ct.Client.FirstName, ct.Client.LastName })
            })
        };

        return Ok(response);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientDto clientDto)
    {
        Trip? trip = await _context.Trips.FindAsync(idTrip);

        if (trip == null || trip.DateFrom <= DateTime.Now)
        {
            return BadRequest(new { Message = "Trip does not exist or has already started" });
        }

        Client? existingClient = await _context.GetClients()
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.Pesel == clientDto.Pesel);

        if (existingClient != null)
        {
            return BadRequest(existingClient.ClientTrips.Any(t => t.IdTrip == idTrip)
                ? new { Message = "Client is already registered for this trip" }
                : new { Message = "Client with this PESEL already exists" });
        }

        Client client = new Client
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Email = clientDto.Email,
            Telephone = clientDto.Telephone,
            Pesel = clientDto.Pesel
        };

        ClientTrip clientTrip = new ClientTrip
        {
            Client = client,
            Trip = trip,
            RegisteredAt = DateTime.UtcNow,
            PaymentDate = clientDto.PaymentDate
        };

        _context.GetClients().Add(client);
        _context.GetClientTrips().Add(clientTrip);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClient), new { id = client.IdClient }, client);
    }

    [HttpGet("{idClient}")]
    public async Task<IActionResult> GetClient(int idClient)
    {
        Client? client = await _context.GetClients().FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            return NotFound(new { Message = "Client not found" });
        }

        return Ok(client);
    }
}