
using CombustiblesrdBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CombustiblesrdBack.Services
{
    public interface ICombustibleService
    {
        List<Combustible> GetCombustible();
        Task<IEnumerable<Combustible>> GetCombustiblesLocalAsync();
        Task<IEnumerable<List<Combustible>>> GetCombustiblesHistory();
    }
}
