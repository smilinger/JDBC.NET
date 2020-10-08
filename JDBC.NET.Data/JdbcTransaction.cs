﻿using System;
using System.Data;
using System.Data.Common;
using JDBC.NET.Proto;

namespace JDBC.NET.Data
{
    public class JdbcTransaction : DbTransaction
    {
        #region Fields
        private readonly JdbcConnection _connection;
        #endregion

        #region Properties
        protected override DbConnection DbConnection => _connection;

        public override IsolationLevel IsolationLevel { get; }

        public bool IsDisposeed { get; private set; }
        #endregion

        #region Constructor
        internal JdbcTransaction(JdbcConnection connection, IsolationLevel IsolationLevel)
        {
            _connection = connection;
            this.IsolationLevel = IsolationLevel;
        }
        #endregion

        #region IDbTransaction
        public override void Commit()
        {
            if (IsDisposeed)
                throw new ObjectDisposedException(ToString());

            _connection.Bridge.Database.commit(new TransactionRequest
            {
                ConnectionId = _connection.ConnectionId
            });
        }

        public override void Rollback()
        {
            if (IsDisposeed)
                throw new ObjectDisposedException(ToString());

            _connection.Bridge.Database.rollback(new TransactionRequest
            {
                ConnectionId = _connection.ConnectionId
            });
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (IsDisposeed)
                return;

            _connection.Bridge.Database.setAutoCommit(new SetAutoCommitRequest
            {
                ConnectionId = _connection.ConnectionId,
                UseAutoCommit = true
            });

            base.Dispose(disposing);
            IsDisposeed = true;
        }
        #endregion
    }
}