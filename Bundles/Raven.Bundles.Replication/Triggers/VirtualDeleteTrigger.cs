using System.Threading;
using Newtonsoft.Json.Linq;
using Raven.Database;
using Raven.Database.Plugins;

namespace Raven.Bundles.Replication.Triggers
{
    /// <summary>
    /// We can't allow real deletes when using replication, because
    /// then we won't have any way to replicate the delete. Instead
    /// we allow the delete but don't do actual delete, we replace it 
    /// with a delete marker instead
    /// </summary>
    public class VirtualDeleteTrigger : AbstractDeleteTrigger
    {
        ThreadLocal<JToken> deletedSource = new ThreadLocal<JToken>();
        ThreadLocal<JToken> deletedVersion = new ThreadLocal<JToken>();

        public override void OnDelete(string key, TransactionInformation transactionInformation)
        {
            if (ReplicationContext.IsInReplicationContext)
                return;

            var document = Database.Get(key, transactionInformation);
            if (document == null)
                return;
            deletedSource.Value = document.Metadata[ReplicationConstants.RavenReplicationSource];
            deletedVersion.Value = document.Metadata[ReplicationConstants.RavenReplicationVersion];
        }

        public override void AfterDelete(string key, TransactionInformation transactionInformation)
        {
            if (ReplicationContext.IsInReplicationContext)
                return;
            var metadata = new JObject(
                new JProperty("Raven-Delete-Marker", true),
                new JProperty(ReplicationConstants.RavenReplicationParentSource, deletedSource.Value),
                new JProperty(ReplicationConstants.RavenReplicationParentVersion, deletedVersion.Value)
                );
            deletedVersion.Value = null;
            deletedSource.Value = null;
            Database.Put(key, null, new JObject(), metadata,transactionInformation);
        }
    }
}
