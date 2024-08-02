

using combustibleWorker.Interface;
using combustibleWorker.Models;
using Dapper;
using System.Data;

namespace combustibleWorker.Repository
{
    public class DataRepository : IDataRepository, IDisposable
    {
        private readonly IDbConnection _dbConnection;

        public DataRepository(IDbConnection dbConnection)
        {
            
            this._dbConnection = dbConnection;
        }
        

        public async Task<int> AddAsync(Combustible model)
        {

            try
            {
                var sql = "INSERT INTO combustibleRD (name, price, updateDate) VALUES (@Nombre, @Precio, @UpdateDate)";
                _dbConnection.Close();
                _dbConnection.Open();
                int x = await _dbConnection.ExecuteAsync(sql, model);
                _dbConnection.Close();
                return x;
            }
            catch (Exception ex )
            {

                throw ex;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _dbConnection.Close();
        }

        public async Task<IEnumerable<Combustible>> GetAllAsync()
        {
            var sql = "SELECT * FROM combustibleRD";
            return await _dbConnection.QueryAsync<Combustible>(sql);
        }

        public async Task<Combustible> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM combustibleRD WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Combustible>(sql, new { Id = id });
        }

        public Task<int> UpdateAsync(Combustible model)
        {
            throw new NotImplementedException();
        }
    }
}
