using Lab9.Repositories;

namespace Lab9.Services;

public class ClientsService : IClientsService
{
    private readonly IClientsRepository _clientsRepository;

    public ClientsService(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public async Task<bool> DeleteClient(int idClient)
    {
        var result = await _clientsRepository.DeleteClient(idClient);
        return result;
    }
}