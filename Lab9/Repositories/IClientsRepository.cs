namespace Lab9.Repositories;

public interface IClientsRepository
{
    Task<bool> DeleteClient(int idClient);
}