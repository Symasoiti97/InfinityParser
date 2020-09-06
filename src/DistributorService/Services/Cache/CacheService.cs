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
        private readonly Dictionary<Guid, HashSet<ItemDto>> _dictionaryItem;
        private readonly HashSet<DataSiteDto> _sites;
        private bool _isEnable = true;

        public CacheService(Timer timer)
        {
            _dictionaryItem = new Dictionary<Guid, HashSet<ItemDto>>();
            _sites = new HashSet<DataSiteDto>();

            _timer = timer ?? throw new ArgumentNullException(nameof(timer));

            InitTimerClear();
        }

        public IEnumerable<T> SaveAndGetUncachedItem<T>(Guid siteId, IEnumerable<T> items) where T : ItemDto
        {
            if (!_isEnable)
                return items;

            var itemsHash = new HashSet<ItemDto>(items);

            if (_dictionaryItem.ContainsKey(siteId))
            {
                itemsHash.ExceptWith(_dictionaryItem[siteId]);
                _dictionaryItem[siteId].UnionWith(itemsHash);
            }
            else
            {
                _dictionaryItem.Add(siteId, itemsHash);
            }

            return itemsHash.Cast<T>().ToList();
        }

        public DataSiteDto GetCachedSiteOrNull(Guid siteId)
        {
            return !_isEnable ? null : _sites.SingleOrDefault(i => i.Id == siteId);
        }

        public void SetCachedSite(DataSiteDto dataSite)
        {
            if (_isEnable) _sites.UnionWith(new[] {dataSite});
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
            _dictionaryItem.Clear();
            _isEnable = true;
        }

        private void TimerOnDisposed(object sender, EventArgs e)
        {
            _timer.Stop();
        }
    }
}