namespace Lab9.Services;

public interface IClientsService
{
    Task<bool> DeleteClient(int idClient);
}