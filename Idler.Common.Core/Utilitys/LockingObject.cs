using System;
using System.Threading;

namespace Idler.Common.Core.Utilitys
{

    public class LockingObject : IDisposable
    {
        private readonly ReaderWriterLockSlim _readerWriterLock;
        private readonly AccessMode _accessMode = AccessMode.Read;

        public LockingObject(ReaderWriterLockSlim @lock, AccessMode lockMode)
        {
            this._readerWriterLock = @lock;
            this._accessMode = lockMode;

            if (this._accessMode == AccessMode.Read)
            {
                this._readerWriterLock.EnterReadLock();
            }
            else if (this._accessMode == AccessMode.Write)
            {
                this._readerWriterLock.EnterWriteLock();
            }
            else
            {
                this._readerWriterLock.EnterUpgradeableReadLock();
            }
        }

        public void Dispose()
        {
            if (this._accessMode == AccessMode.Read)
            {
                this._readerWriterLock.ExitReadLock();
            }
            else if (this._accessMode == AccessMode.Write)
            {
                this._readerWriterLock.ExitWriteLock();
            }
            else
            {
                this._readerWriterLock.ExitUpgradeableReadLock();
            }
        }
    }
}