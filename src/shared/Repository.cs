namespace BryanPorter.IntervalTrainer.Shared
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Data.Common;

    using BryanPorter.IntervalTrainer.Shared.Interfaces;
    using BryanPorter.IntervalTrainer.Shared.Models;


    public class Repository 
        : IRepository
    {
        readonly IStorage _storage;

        readonly Dictionary<Guid, RoutineCacheEntry> _instanceCache = new Dictionary<Guid, RoutineCacheEntry>();
        readonly Dictionary<Guid, WeakReference<Routine>> _detachedInstances = new Dictionary<Guid, WeakReference<Routine>>();

        readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public Repository(IStorage storageSystem)
        {
            _storage = storageSystem;
        }

        public Routine[] GetRoutines()
        { 
            var retVal = new List<Routine>();

            foreach (var id in _storage.GetStorageIdentifiers())
            {
                retVal.Add(GetRoutine(id));
            }

            return retVal.ToArray();
        }

        public Routine GetRoutine(Guid routineId)
        {
            Routine retVal = null;

            if (_instanceCache.ContainsKey(routineId))
            {
                if (_storage.CheckStorageIdentifierExists(routineId))
                {
                    if (_storage.GetLastWriteDateTimeUtc(routineId) > _instanceCache[routineId].LoadTimeUtc)
                    {
                        // invalidate our entry and reload
                        retVal = LoadRoutine(routineId);
                        var oldRoutine = _instanceCache[routineId].Routine;

                        // add old routine to our detached reference list
                        _detachedInstances.Add(routineId, new WeakReference<Routine>(oldRoutine));

                        _instanceCache[routineId].LoadTimeUtc = DateTime.UtcNow;
                        _instanceCache[routineId].Routine = retVal;
                    }
                    else
                    {
                        retVal = _instanceCache[routineId].Routine;
                    }
                }
            }
            else
            {
                // Hmm, requested instance is not in our cache. Does it exist?

                if (_storage.CheckStorageIdentifierExists(routineId))
                {
                    // it does exist
                    retVal = LoadRoutine(routineId);

                    _instanceCache.Add(retVal.RoutineId, new RoutineCacheEntry() { Routine = retVal, LoadTimeUtc = DateTime.UtcNow });
                }
            }

            return retVal;
        }

        public void SaveRoutine(Routine routine)
        {
            ThrowOnConcurrencyViolation(routine);

            using (var stream = _storage.GetWriteableStream(routine.RoutineId))
            {
                _binaryFormatter.Serialize(stream, routine);
            }

            if (!_instanceCache.ContainsKey(routine.RoutineId))
                _instanceCache.Add(routine.RoutineId, new RoutineCacheEntry() { Routine = routine, LoadTimeUtc = DateTime.UtcNow });
        }

        public void DeleteRoutine(Routine routine)
        {
            ThrowOnConcurrencyViolation(routine);

            _instanceCache.Remove(routine.RoutineId);
            _detachedInstances.Add(routine.RoutineId, new WeakReference<Routine>(routine));

            _storage.RemoveStorage(routine.RoutineId);
        }

        public void Refresh()
        {
            // really, this just means empty our cache
            _instanceCache.Clear();
        }

        private void ThrowOnConcurrencyViolation(Routine routine)
        {
            if (_detachedInstances.ContainsKey(routine.RoutineId))
            {
                var wre = _detachedInstances[routine.RoutineId];
                var detachedRoutine = default(Routine);

                if (wre.TryGetTarget(out detachedRoutine))
                {
                    if (detachedRoutine == routine)
                    {
                        // passed routine is in our detached list, which means a different load
                        // invalidated that routine entry and someone was holding a reference.
                        throw new InvalidOperationException(); // TODO: pick a more appropriate exception?
                    }
                }
                else
                {
                    // we failed to retrieve the target of the weak reference because it was cleaned up.
                    _detachedInstances.Remove(routine.RoutineId);
                }
            }
        }

        private Routine LoadRoutine(Guid routineId)
        {
            var retVal = default(Routine);

            using (var stream = _storage.GetReadableStream(routineId))
            {
                retVal = _binaryFormatter.Deserialize(stream) as Routine;
            }

            return retVal;
        }

        private class RoutineCacheEntry
        {
            public DateTime LoadTimeUtc { get; set; }
            public Routine Routine { get; set; }
        }
    }
}
