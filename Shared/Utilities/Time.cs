using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public static class Time
    {
        private static TimeSpan? _adjustBy;
        private static DateTimeOffset? _time;

        public static DateTimeOffset Now
        {
            get
            {
                var baseTime = _time ?? DateTimeOffset.Now;
                if (_adjustBy.HasValue)
                {
                    return baseTime.Add(_adjustBy.Value);
                }
                return baseTime;
            }
        }

        public static DateTimeOffset UtcNow => _time ?? DateTimeOffset.UtcNow;

        public static DateTimeOffset Adjust(TimeSpan by)
        {
            if (_adjustBy.HasValue)
            {
                _adjustBy = _adjustBy.Value.Add(by);
            }
            else
            {
                _adjustBy = by;
            }

            return Now;
        }

        public static void Reset()
        {
            _adjustBy = null;
            _time = null;
        }

        public static void Set(DateTimeOffset time)
        {
            _adjustBy = null;
            _time = time;
        }
    }
}
