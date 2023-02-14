using EodHistoricalData.Sdk.Events;
using Import.Infrastructure.Events;
using Shibusa.Extensions;
using System.Text;
using static Import.Infrastructure.Configuration.Constants;

namespace Import.Infrastructure.Domain
{
    public class ImportCycle : IDisposable
    {
        private bool disposedValue;

        private readonly Stream logStream;
        private readonly List<ActionItem> actions;

        public ImportCycle(DirectoryInfo outputDirectory)
        {
            Id = new string(DateTime.Now.Ticks.ToString().ToCharArray()[^12..^0]);

            OutputDirectory = new DirectoryInfo(Path.Combine(outputDirectory.FullName, Id));

            if (!OutputDirectory.Exists) { OutputDirectory.Create(); }

            logStream = File.Create(Path.Combine(OutputDirectory.FullName, "log.txt"));

            ApiEventPublisher.RaiseApiResponseEventHandler += EventPublisher_RaiseApiResponseEventHandler;
            ApiEventPublisher.RaiseMessageEventHandler += EventPublisher_RaiseMessageEventHandler;
            DomainEventPublisher.RaiseMessageEventHandler += EventPublisher_RaiseMessageEventHandler;

            actions = new List<ActionItem>();
        }

        public string Id { get; }

        public DirectoryInfo OutputDirectory { get; }

        private ActionItem[]? actionItemArray = null;

        public ActionItem[] Actions
        {
            get
            {
                actionItemArray ??= SortActionItems(actions).ToArray();

                return actionItemArray;
            }
        }

        public int RemoveActions(Predicate<ActionItem> predicate) => actions.RemoveAll(predicate);

        public bool TryAddAction(ActionItem action, out string? reason)
        {
            reason = null;

            var existing = actions.Where(a => a.ActionName == action.ActionName &&
                a.TargetDataType == action.TargetDataType &&
                a.TargetName == action.TargetName &&
                a.TargetScope == action.TargetScope);

            if (existing.Any())
            {
                reason = "Action already added.";
            }

            if (reason == null)
            {
                if (action.TargetScope == null)
                {
                    actions.Add(action);
                    actionItemArray = null;
                }
                else
                {
                    if (action.ActionName == ActionNames.Import && action.TargetScope == DataTypeScopes.Full)
                    {
                        int removeCount = actions.RemoveAll(a => a.ActionName == ActionNames.Import &&
                            a.TargetDataType == action.TargetDataType &&
                            a.TargetName == action.TargetName);

                        actions.Add(action);
                        actionItemArray = null;
                    }

                    if (action.ActionName == ActionNames.Import &&
                        action.TargetScope is DataTypeScopes.Bulk or DataTypeScopes.TryBulkThenFull)
                    {
                        if (actions.Any(a => a.ActionName == ActionNames.Import && a.TargetScope == DataTypeScopes.Full))
                        {
                            reason = $"Trying to add a {action.TargetScope} item for ${action.TargetDataType} when an item with scope of {DataTypeScopes.Full} already exists. First remove {DataTypeScopes.Full} item.";
                        }
                        else
                        {
                            actions.Add(action);
                            actionItemArray = null;
                        }
                    }
                }
            }

            return reason is null;
        }

        private static IEnumerable<ActionItem> SortActionItems(IEnumerable<ActionItem> actionItems)
        {
            List<ActionItem> results = new();

            var actions = actionItems.ToList();

            // If any of the actions are "Skip", then we're skipping the import actions
            if (actions.Any(a => a.ActionName == ActionNames.Skip))
            {
                actions.RemoveAll(r => r.ActionName == ActionNames.Import);
            }

            if (!actions.Any()) { return Enumerable.Empty<ActionItem>(); }

            actions.Sort(ActionComparison);

            List<(string Action, string Target, string TargetDataType)> rollingList = new();

            foreach (var action in actions)
            {
                if (action.TargetName == null)
                {
                    results.Add(action);
                }
                else
                {
                    if (!rollingList.Contains((action.ActionName, action.TargetName, action.TargetDataType!)))
                    {
                        rollingList.Add((action.ActionName, action.TargetName, action.TargetDataType!));
                        results.Add(action);
                    }
                }
            }

            // If we have overlapping "Full" and "Bulk" imports, remove the bulk items.
            foreach (var fullImport in results.Where(r => r.ActionName == ActionNames.Import &&
                r.TargetScope == DataTypeScopes.Full))
            {
                results.RemoveAll(r => r.ActionName == ActionNames.Import &&
                    r.TargetScope == DataTypeScopes.Bulk &&
                    r.TargetDataType == fullImport.TargetDataType);
            }

            return results;
        }

        private static int ActionComparison(ActionItem item1, ActionItem item2)
        {
            if (ReferenceEquals(item1, item2)) return 0;

            int result = item1.ActionName.CompareTo(item2.ActionName);

            if (result == 0) { result = item1.TargetScopeValue.GetValueOrDefault().CompareTo(item2.TargetScopeValue.GetValueOrDefault()); }
            if (result == 0) { result = item1.TargetDataTypeSortValue.GetValueOrDefault().CompareTo(item2.TargetDataTypeSortValue.GetValueOrDefault()); }

            return result;
        }

        private readonly HashSet<string> usedTimestamps = new();
        
        private readonly object tsObj = new();

        private void EventPublisher_RaiseApiResponseEventHandler(object? sender, ApiResponseEventArgs e)
        {
            const string timestampFormat = "yyyyMMddHHmmss";
            string ts = e.UtcTimestamp.ToString(timestampFormat);

            lock (tsObj)
            {
                int i = 1;
                while (usedTimestamps.Contains(ts))
                {
                    ts = $"{ts[0..timestampFormat.Length]}({++i})";
                }

                usedTimestamps.Add(ts);
            }

            var requestFileName = $"{ts}-request.json";
            var responseFileName = $"{ts}-response.json";
            File.WriteAllText(Path.Combine(OutputDirectory.FullName, requestFileName), e.Request);
            File.WriteAllText(Path.Combine(OutputDirectory.FullName, responseFileName), e.Response);

            if (e.ApiResponseException != null)
            {
                StringBuilder text = new();
                text.AppendLine(e.ApiResponseException.Uri);
                text.AppendLine(e.ApiResponseException.Source);
                text.AppendLine(e.ApiResponseException.Message);
                if (e.ApiResponseException.InnerException != null)
                {
                    text.AppendLine(e.ApiResponseException.InnerException.ToString());
                }

                var exceptionFileName = $"{ts}-exception.txt";
                File.WriteAllText(Path.Combine(OutputDirectory.FullName, exceptionFileName), text.ToString());
            }
        }

        private readonly object logObj = new();

        private void EventPublisher_RaiseMessageEventHandler(object? sender, MessageEventArgs e)
        {
            if (logStream?.CanWrite ?? false)
            {
                lock (logObj)
                {
                    logStream.WriteLine($"{e.UtcTimestamp:yyyy-MM-dd HH:mm:ss}\t{e}");

                    if (e.Exception != null)
                    {
                        logStream.WriteLine(e.Exception.ToString());
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ApiEventPublisher.RaiseApiResponseEventHandler -= EventPublisher_RaiseApiResponseEventHandler;
                    ApiEventPublisher.RaiseMessageEventHandler -= EventPublisher_RaiseMessageEventHandler;
                    DomainEventPublisher.RaiseMessageEventHandler -= EventPublisher_RaiseMessageEventHandler;

                    logStream?.Close();
                    logStream?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
