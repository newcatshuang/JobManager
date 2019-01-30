using System.Collections.Generic;

namespace Newcats.JobManager.Common.NetCore.Util.Helper.CronHelper
{
    public static class SortedSetExtensions
    {
        public static SortedSet<int> TailSet(this SortedSet<int> set, int value)
        {
            return set.GetViewBetween(value, 9999999);
        }
    }
}
