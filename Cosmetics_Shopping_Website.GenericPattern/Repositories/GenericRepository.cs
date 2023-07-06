using Azure;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Repositories
{
    public class EntityBase
    {
        public int Id { get; set; }
    }

    public class GenericRepository: IGenericRepository
    {
        private readonly CosmeticsShoppingDbContext _context;


        public GenericRepository(CosmeticsShoppingDbContext context)
        {
            _context = context;

        }
       
        public async Task<IQueryable<T>> GetFromMutlipleTable<T>(params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            query = includes.Aggregate(query, (current, includes) => current.Include(includes));
            return query;
        }

        public async Task<IEnumerable<T>> GetFromMultipleTableBasedOnConditions<T>(Expression<Func<T, bool>> filters, params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            query = includes.Aggregate(query, (current, includes) => current.Include(includes)).Where(filters);
            return query;
        }


        //-----------------------------------------------------------------------

       

        //-------------------------------------------------------------------
        public async Task<T> GetByIdFromMultipleTable<T>(int id, params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            query = includes.Aggregate(query, (current, includes) => current.Include(includes));

            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var idExpression = Expression.Property(parameter, idProperty);
                var equalExpression = Expression.Equal(idExpression, Expression.Constant(id));
                var lambdaExpression = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);

                query = query.Where(lambdaExpression);
            }
            else
            {
                throw new InvalidOperationException("Entity does not have an 'Id' property.");
            }

            return query.SingleOrDefault();


        }
        public async Task<IEnumerable<T>> Search<T>(Expression<Func<T, bool>> filters) where T : class
        {
            return await _context.Set<T>().Where(filters).ToListAsync();
        }

        public async Task<IEnumerable<T>> SearchFormMultipleTable<T>(Expression<Func<T, bool>> filters, params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            query = includes.Aggregate(query, (current, includes) => current.Include(includes)).Where(filters);
            return query;
        }

        public async Task<IEnumerable<T>> GetTable<T>() where T: class
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T> Get<T>(Expression<Func<T, bool>> filters) where T : class
        {
            return await _context.Set<T>().Where(filters).FirstOrDefaultAsync();
        }

        public async Task<T> GetById<T>(int id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> Post<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();

        }

        public async Task<T> Put<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges(true);
            return entity;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }

   
}
