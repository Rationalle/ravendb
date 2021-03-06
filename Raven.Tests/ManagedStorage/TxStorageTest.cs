using System.IO;
using Raven.Database;
using Raven.Database.Storage;
using Raven.Storage.Managed;

namespace Raven.Storage.Tests
{
	public class TxStorageTest
	{
		public TxStorageTest()
		{
			if(Directory.Exists("test"))
				Directory.Delete("test", true);
		}

        public ITransactionalStorage NewTransactionalStorage()
        {
            var newTransactionalStorage = new TransactionalStorage(new RavenConfiguration
            {
                DataDirectory = "test",
            }, () => { });
            newTransactionalStorage.Initialize();
            return newTransactionalStorage;
        }
	}
}