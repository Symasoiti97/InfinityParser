using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Db.Interfaces;
using Helper.Exceptions;
using Npgsql;

namespace Domain.Provider
{
    public class DataProvider : IDataProvider
    {
        private readonly InfinityParserDbContext _db;

        public DataProvider(InfinityParserDbContext context)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TransactionScope Transaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var ambientLevel = System.Transactions.Transaction.Current?.IsolationLevel;
            var txOptions = new TransactionOptions
            {
                IsolationLevel = ambientLevel == null ? isolationLevel : (IsolationLevel) Math.Min((int) ambientLevel, (int) isolationLevel)
            };
            return new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);
        }

        public async Task Insert<T>(T model) where T : class
        {
            await ExecuteCommand(async () =>
            {
                if (model is ICreatable m)
                {
                    m.CreateDate = DateTime.UtcNow;
                }

                await _db.Set<T>().AddAsync(model);
                return _db.SaveChanges();
            });
        }

        public async Task InsertRange<T>(IEnumerable<T> models) where T : class
        {
            await ExecuteCommand(async () =>
            {
                var entities = models as T[] ?? models.ToArray();

                if (entities.Any())
                {
                    foreach (var entity in entities)
                    {
                        if (entity is ICreatable m)
                        {
                            m.CreateDate = DateTime.UtcNow;
                        }
                    }

                    await _db.Set<T>().AddRangeAsync(entities);
                }

                return _db.SaveChanges();
            });
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
            var entities = models as T[] ?? models.ToArray();

            if (entities.Any())
            {
                _db.Set<T>().RemoveRange(entities);
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

        private static async Task ExecuteCommand(Func<Task<int>> func)
        {
            try
            {
                await func();
            }
            catch (Exception exception) when (exception.InnerException is PostgresException ex)
            {
                var message = ex.Message + ex.Detail;

                if (ex.SqlState == "23505")
                {
                    throw new ObjectAlreadyExistsException(message, ex);
                }

                // if (ex.SqlState == "40001")
                // {
                //     throw new ConcurrentModifyException(message, ex);
                // }

                throw new PostgreSqlException(ex.SqlState, message, ex);
            }
        }
    }
}