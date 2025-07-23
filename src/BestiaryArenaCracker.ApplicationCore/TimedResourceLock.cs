using System.Collections.Concurrent;

namespace BestiaryArenaCracker.ApplicationCore
{
    public class TimedResourceLock<TKey>(TimeSpan ttl) where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, DateTime> _inUse = new();
        private readonly ConcurrentDictionary<TKey, SemaphoreSlim> _locks = new();
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromSeconds(30);
        private DateTime _lastCleanup = DateTime.UtcNow;

        public IReadOnlySet<TKey> ReservedValues => _inUse.Keys.ToHashSet();
        public async Task<bool> TryAcquireAsync(TKey key)
        {
            MaybeCleanup();

            var now = DateTime.UtcNow;

            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            if (!await semaphore.WaitAsync(0))
                return false;

            try
            {
                if (_inUse.TryGetValue(key, out var expires) && expires > now)
                    return false;

                _inUse[key] = now.Add(ttl);
                return true;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Release(TKey key)
        {
            _inUse.TryRemove(key, out _);
        }

        private void MaybeCleanup()
        {
            var now = DateTime.UtcNow;
            if ((now - _lastCleanup) < _cleanupInterval)
                return;

            _lastCleanup = now;

            foreach (var kvp in _inUse.ToArray())
            {
                if (kvp.Value <= now)
                {
                    _inUse.TryRemove(kvp.Key, out _);
                    _locks.TryRemove(kvp.Key, out _);
                }
            }
        }
    }
}