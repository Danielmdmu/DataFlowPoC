using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlowPoC
{
    public enum SyncActionType
    {
        Added = 0,
        Modiefied = 1,
        Deleted = 2
    }

    public enum SyncTarget
    {
        Collection = 0,
        Filter = 1
    }

    public class SyncAction
    {
        public SyncAction(SyncActionType syncActionType, SyncTarget syncTarget)
        {
            this.SyncActionType = syncActionType;
            this.SyncTarget = syncTarget;
            Guid = Guid.NewGuid();
        }

        public SyncAction(SyncTarget syncTarget)
        {
            this.SyncTarget = syncTarget;
            var rnd = new Random();
            this.SyncActionType = (SyncActionType)rnd.Next(0, 2);
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; }
        public SyncActionType SyncActionType { get; }
        public SyncTarget SyncTarget { get; }

        public static SyncAction CreateRandomSyncAction()
        {
            var rnd = new Random();

            return new SyncAction((SyncActionType)rnd.Next(0, 2), (SyncTarget)rnd.Next(0, 1));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            switch (SyncActionType)
            {
                case SyncActionType.Added:
                    sb.Append("Add");
                    break;

                case SyncActionType.Modiefied:
                    sb.Append("Modify");
                    break;

                case SyncActionType.Deleted:
                    sb.Append("Delete");
                    break;

                default:
                    break;
            }

            sb.Append(" - ");

            switch (SyncTarget)
            {
                case SyncTarget.Collection:
                    sb.Append("Collection");
                    break;

                case SyncTarget.Filter:
                    sb.Append("Filter");
                    break;

                default:
                    break;
            }

            sb.Append(" - ");
            sb.Append(Guid);

            return sb.ToString();
        }
    }
}