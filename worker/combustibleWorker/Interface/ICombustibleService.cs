using combustibleWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace combustibleWorker.Interface
{
    public interface ICombustibleService
    {
        List<Combustible> GetCombustible();
    }
}
