using Lab9.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab9.Controllers;

[ApiController]
[Route("/api/clients")]
public class ClientsController : ControllerBase
{
    private IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        bool result = await _clientsService.DeleteClient(idClient);
        if (result)
        {
            return NoContent();
        }

        return NotFound();
    }
}