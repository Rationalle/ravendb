using System;

namespace Raven.Database.Storage
{
    public interface ITransactionalStorage : IDisposable
	{
        /// <summary>
        /// This is used mostly for replication
        /// </summary>
		Guid Id { get; }
		void Batch(Action<IStorageActionsAccessor> action);
		void ExecuteImmediatelyOrRegisterForSyncronization(Action action);
		bool Initialize();
		void StartBackupOperation(DocumentDatabase database, string backupDestinationDirectory);
		void Restore(string backupLocation, string databaseLocation);

	    Type TypeForRunningQueriesInRemoteAppDomain { get;}
        object StateForRunningQueriesInRemoteAppDomain { get; }
	}
}
