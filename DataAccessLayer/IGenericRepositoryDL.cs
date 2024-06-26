﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IGenericRepositoryDL<T> where T : class
    {
        Task<T> Get(int id);
        Task<T> GetByGen<T2>( T2 val1);
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
