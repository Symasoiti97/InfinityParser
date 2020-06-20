using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.Provider
{
    public class DataProvider : IDataProvider
    {
        private readonly InfinityParserDbContext _db;

        public DataProvider(InfinityParserDbContext context)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IDbContextTransaction> Transaction()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public async Task Insert<T>(T model) where T : class
        {
            await _db.Set<T>().AddAsync(model);
            await _db.SaveChangesAsync();
        }

        public async Task InsertRange<T>(IEnumerable<T> models) where T : class
        {
            if (models != null && models.Any())
            {
                await _db.Set<T>().AddRangeAsync(models);
                await _db.SaveChangesAsync();
            }
        }

        public IQueryable<T> Get<T>() where T : class
        {
            return _db.Set<T>();
        }

        public IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _db.Set<T>().Where(predicate);
        }

        public async Task Delete<T>(T model) where T : class
        {
            _db.Set<T>().Remove(model);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteRange<T>(IEnumerable<T> models) where T : class
        {
            if (models != null && models.Any())
            {
                _db.Set<T>().RemoveRange(models);
                await _db.SaveChangesAsync();
            }
        }

        public async Task Update<T>(T model) where T : class
        {
            _db.Set<T>().Update(model);
            await _db.SaveChangesAsync();
        }
        
        public async Task UpdateRange<T>(IEnumerable<T> models) where T : class
        {
            _db.Set<T>().UpdateRange(models);
            await _db.SaveChangesAsync();
        }
    }
}