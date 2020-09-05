using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Dto;

namespace DistributorService.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly Timer _timer;
        private readonly Dictionary<Guid, HashSet<ItemDto>> _dictionary;
        private bool _isEnable = true;

        public CacheService(Timer timer)
        {
            _dictionary = new Dictionary<Guid, HashSet<ItemDto>>();
            _timer = timer ?? throw new ArgumentNullException(nameof(timer));
            InitTimerClear();
        }

        public IEnumerable<T> SaveAndGetUncached<T>(Guid siteId, IEnumerable<T> items) where T : ItemDto
        {
            if (!_isEnable)
                return items;

            var itemsHash = new HashSet<ItemDto>(items);

            if (_dictionary.ContainsKey(siteId))
            {
                itemsHash.ExceptWith(_dictionary[siteId]);
                _dictionary[siteId].UnionWith(itemsHash);
            }
            else
            {
                _dictionary.Add(siteId, itemsHash);
            }

            return itemsHash.Cast<T>().ToList();
        }

        private void InitTimerClear()
        {
            _timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Disposed += TimerOnDisposed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _isEnable = false;
            _dictionary.Clear();
            _isEnable = true;
        }

        private void TimerOnDisposed(object sender, EventArgs e)
        {
            _timer.Stop();
        }
    }
}