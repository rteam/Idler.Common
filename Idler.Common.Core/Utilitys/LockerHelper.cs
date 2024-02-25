using System;
using System.Threading;

namespace Idler.Common.Core.Utilitys
{

    public class LockerHelper
    {
        private ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        
        public DateTime LastRequireReadTime { get; private set;}

        public DateTime LastRequireWriteTime{ get; private set;}

        public DateTime LastRequireUpgradeableReadTime{ get; private set; }

        public LockingObject Lock(AccessMode accessMode)
        {
            if (accessMode == AccessMode.Read)
            {
                this.LastRequireReadTime = DateTime.Now;
            }
            else if (accessMode == AccessMode.Updata)
            {
                this.LastRequireWriteTime = DateTime.Now;
            }
            else
            {
                this.LastRequireUpgradeableReadTime = DateTime.Now;
            }

            return new LockingObject(this.readerWriterLock, accessMode);
        }
    }
}