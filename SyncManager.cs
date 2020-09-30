using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowPoC
{
    public class SyncManager
    {
        private BatchBlock<SyncAction> batch;

        private BroadcastBlock<SyncAction> broadcastBlock;
        private BufferBlock<SyncAction> buffer;
        private ActionBlock<SyncAction[]> syncAction;
        private TimeSpan timeout;
        private IObservable<Timestamped<long>> timer;
        private ActionBlock<SyncAction> timerBlock;
        private bool timerIsRunning;
        private IDisposable timerSubscription;

        public SyncManager(TimeSpan timeout, int batchSize)
        {
            this.timeout = timeout;
            timerIsRunning = false;

            buffer = new BufferBlock<SyncAction>();
            batch = new BatchBlock<SyncAction>(batchSize, new GroupingDataflowBlockOptions() { Greedy = true });
            syncAction = new ActionBlock<SyncAction[]>(Sync);
            broadcastBlock = new BroadcastBlock<SyncAction>(x => x);
            timerBlock = new ActionBlock<SyncAction>(timerBlockAction);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            buffer.LinkTo(broadcastBlock, linkOptions);
            broadcastBlock.LinkTo(batch, linkOptions);
            broadcastBlock.LinkTo(timerBlock, linkOptions);
            batch.LinkTo(syncAction, linkOptions);
        }

        public Task AddSyncAction(SyncAction syncAction)
        {
            Console.WriteLine($"Add SyncAction to Buffer at {DateTime.Now}");
            Console.WriteLine($"SyncAction = {syncAction}");

            return buffer.SendAsync(syncAction);
        }

        private async Task Sync(SyncAction[] obj)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("####################################################");
            Console.WriteLine("Start Synchronization");

            foreach (var item in obj)
            {
                Console.WriteLine($"Syncing of {item}");
                await Task.Delay(2000);
            }

            Console.WriteLine("Synchronization Complete");
            Console.WriteLine("####################################################");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void timerBlockAction(SyncAction obj)
        {
            if (!timerIsRunning)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Start Timer at {DateTime.Now} with timeout = {timeout}");
                Console.ResetColor();

                timer = Observable.Interval(timeout).Timestamp();
                timerSubscription = timer.Subscribe(timerTriggered);
                timerIsRunning = true;
            }
        }

        private void timerTriggered(Timestamped<long> obj)
        {
            var time = TimeZone.CurrentTimeZone.ToLocalTime(obj.Timestamp.DateTime);

            Console.WriteLine($"Timer triggered at {time}");

            if (timerIsRunning)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Reset Timer");
                Console.ResetColor();

                timer = null;
                timerSubscription.Dispose();
                timerSubscription = null;
                timerIsRunning = false;
            }

            batch.TriggerBatch();
        }
    }
}