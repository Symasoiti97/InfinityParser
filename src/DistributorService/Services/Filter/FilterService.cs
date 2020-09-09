using System.Collections.Generic;
using System.Linq;
using Dto;

namespace DistributorService.Services.Filter
{
    public class FilterService : IFilterService
    {
        public IEnumerable<T> Filter<T>(IEnumerable<T> items, Dictionary<string, string[]> includeFilter, Dictionary<string, string[]> excludeFilter) where T : ItemDto
        {
            var result = IncludeFilter(items, includeFilter);
            return ExcludeFilter(result, excludeFilter);
        }

        private static IEnumerable<T> IncludeFilter<T>(IEnumerable<T> items, Dictionary<string, string[]> filter) where T : ItemDto
        {
            return items.Where(i => IsIncludeValid(i, filter)).ToArray();
        }

        private static IEnumerable<T> ExcludeFilter<T>(IEnumerable<T> items, Dictionary<string, string[]> filter) where T : ItemDto
        {
            return items.Where(i => IsExcludeValid(i, filter)).ToArray();
        }

        private static bool IsIncludeValid<T>(T item, Dictionary<string, string[]> filter) where T : ItemDto
        {
            var properties = GetProperties(item);

            foreach (var (key, value) in properties)
            {
                if (filter.TryGetValue(key, out var values))
                {
                    if (!values.All(i => value.ToLower().Contains(i.ToLower())))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsExcludeValid<T>(T item, Dictionary<string, string[]> filter) where T : ItemDto
        {
            var properties = GetProperties(item);

            foreach (var (key, value) in properties)
            {
                if (filter.TryGetValue(key, out var values))
                {
                    if (values.Any(i => value.ToLower().Contains(i.ToLower())))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static Dictionary<string, string> GetProperties<T>(T item) where T : ItemDto
        {
            return item.GetType().GetProperties().ToDictionary(k => k.Name, k => k.GetValue(item).ToString());
        }
    }
}