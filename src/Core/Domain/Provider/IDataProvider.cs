using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.Provider
{
    public interface IDataProvider
    {
        Task<IDbContextTransaction> Transaction();
        Task Insert<T>(T model) where T : class;
        Task InsertRange<T>(IEnumerable<T> models) where T : class;
        IQueryable<T> Get<T>() where T : class;
        IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task Delete<T>(T model) where T : class;
        Task DeleteRange<T>(IEnumerable<T> models) where T : class;
        Task Update<T>(T model) where T : class;
        Task UpdateRange<T>(IEnumerable<T> models) where T : class;
    }
}