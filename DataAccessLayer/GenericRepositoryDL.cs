using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace DataAccessLayer
{
    public abstract class GenericRepositoryDL<T> : IGenericRepositoryDL<T> where T : class 
    {
        protected readonly ApplicationDbContext _context;

        protected GenericRepositoryDL(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<T> Get(int id)
        {

            return await _context.Set<T>().FindAsync(id);

        }
        public async Task<T> GetByGen<T2>(T2 val1)
        {
            return await _context.Set<T>().FindAsync(val1);
        }
        public async Task<T> GetByByte(byte id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
             var cc=await _context.Set<T>().ToListAsync();
            return cc;
        }

        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAsync();
        }
        public async Task<T> AddWithReturn(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task Delete(T entity)
        {
             _context.Set<T>().Remove(entity);
            await SaveAsync();
        }

        public async Task Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
        }
        public async Task<T> UpdateWithReturn(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
