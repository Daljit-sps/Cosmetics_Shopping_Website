using Cosmetics_Shopping_Website.GenericPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IGenericRepository
    {
        
        //Task<IQueryable<T>> GetByReverseForeignKey<T, TProperty, TResult>(Expression<Func<T, TProperty>> foreignKeyProperty, int foreignKeyId, Expression<Func<T, TResult>> selector) where T : class;
        
        Task<T> GetByIdFromMultipleTable<T>(int id, params Expression<Func<T, object>>[] includes) where T : class;
        Task<IQueryable<T>> GetFromMutlipleTable<T>(params Expression<Func<T, object>>[] includes) where T : class;

        Task<IEnumerable<T>> GetFromMultipleTableBasedOnConditions<T>(Expression<Func<T, bool>> filters, params Expression<Func<T, object>>[] includes) where T : class;

        Task<IEnumerable<T>> GetTable<T>() where T : class;
        Task<T> Get<T>(Expression<Func<T, bool>> filters) where T : class;
        Task<T> GetById<T>(int id) where T : class;
        Task<IEnumerable<T>> GetAll<T>() where T : class;
        Task<T> Post<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<T> Put<T>(T entity) where T : class;

        int Save();
    }
}
