using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Get(int id);
        Task<T> GetByByte(byte id);
        Task<IEnumerable<T>> GetAll();
        Task Add(T entity);
        Task<T> AddWithReturn(T entity);
        Task Delete(T entity);
        Task<T> Delete(int id);
        Task Update(T entity);
        Task<T> UpdateWithReturn(T entity);
    }
}
