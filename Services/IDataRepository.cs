

using CombustiblesrdBack.Models;

namespace CombustiblesrdBack.Interface
{
    public interface IDataRepository
    {
        Task<IEnumerable<Combustible>> GetAllAsync();
        Task<IEnumerable<List<Combustible>>> GetHistory();
    }
}
