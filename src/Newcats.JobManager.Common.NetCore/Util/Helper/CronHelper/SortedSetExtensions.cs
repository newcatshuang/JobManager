using System.Collections.Generic;

namespace Newcats.JobManager.Api.Infrastructure.Helper.CronHelper
{
    public static class SortedSetExtensions
    {
        public static SortedSet<int> TailSet(this SortedSet<int> set, int value)
        {
            return set.GetViewBetween(value, 9999999);
        }
    }
}
