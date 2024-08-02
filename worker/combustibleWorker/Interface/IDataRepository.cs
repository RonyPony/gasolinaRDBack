using combustibleWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace combustibleWorker.Interface
{
    public interface IDataRepository
    {
        Task<IEnumerable<Combustible>> GetAllAsync();
        Task<Combustible> GetByIdAsync(int id);
        Task<int> AddAsync(Combustible model);
        Task<int> UpdateAsync(Combustible model);
        Task<int> DeleteAsync(int id);

        // Otros métodos según sea necesario
    }
}
