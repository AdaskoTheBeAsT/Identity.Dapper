using System;
using System.Data.Common;
using System.Threading;
using Identity.Dapper.Connections;
using Identity.Dapper.UnitOfWork.Contracts;

namespace Identity.Dapper.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly SemaphoreSlim _semaphore;
        private DbConnection? _connection;
        private DbTransaction? _transaction;

        public UnitOfWork(IConnectionProvider connProvider)
        {
            _connectionProvider = connProvider;
            _semaphore = new SemaphoreSlim(1);
        }

        public DbTransaction? Transaction => _transaction;

        public DbConnection? Connection => _connection;

        public void CommitChanges() => _transaction?.Commit();

        public DbConnection CreateOrGetConnection()
        {
            _semaphore.Wait();

            if (_connection == null)
            {
                _connection = _connectionProvider.Create();
                _connection.Open();

                _transaction?.Dispose();
                _transaction = _connection.BeginTransaction();
            }

            _semaphore.Release();

            return _connection;
        }

        public void DiscardChanges() => _transaction?.Rollback();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
                _semaphore.Dispose();
            }
        }
    }
}
