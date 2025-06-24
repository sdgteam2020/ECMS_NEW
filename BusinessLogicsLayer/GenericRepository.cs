using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace BusinessLogicsLayer
{
    public abstract class GenericRepository<T> : IGenericRepositoryDL<T> where T : class
    {
       
        public Task Add(T entity)
        {
            // Validate the entity (optional)
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return Add(entity);
        }

        public Task<T> AddWithReturn(T entity)
        {
            // Validate the entity (optional)
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return AddWithReturn(entity);
        }

        public Task Delete(T entity)
        {
            // Validate the entity (optional)
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return Delete(entity);
        }

        public Task<T> Delete(int id)
        {
          if(id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Delete(id);
        }

        public Task<T> Get(int id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Get(id);
        }
        public Task<T> GetByGen<T2>(T2 val1)
        {
            // Validate the entity (optional)
            if (val1 == null)
            {
                throw new ArgumentNullException(nameof(val1));
            }
            return GetByGen(val1);
        }
        public Task<T> GetByByte(byte id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return GetByByte(id);
        }

        public Task<IEnumerable<T>> GetAll()
        {
            return GetAll();
        }

        public Task Update(T entity)
        {
            return Update(entity);  
        }

        public Task<T> UpdateWithReturn(T entity)
        {
            // Validate the entity (optional)
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return UpdateWithReturn(entity);
        }

        public Task<DTODataTablesResponse<T>> GetDataTableResponse(DTODataTablesRequest request)
        {
            return GetDataTableResponse(request);
        }
    }
}
