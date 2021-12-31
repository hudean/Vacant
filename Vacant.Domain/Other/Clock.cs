using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain
{
    public class Clock : IClock
    {
        protected ClockOptions Options { get; }

        public Clock(IOptions<ClockOptions> options)
        {
            Options = options.Value;
        }

        public virtual DateTime Now => Options.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

        public virtual DateTimeKind Kind => Options.Kind;

        public virtual bool SupportsMultipleTimezone => Options.Kind == DateTimeKind.Utc;

        public virtual DateTime Normalize(DateTime dateTime)
        {
            if (Kind == DateTimeKind.Unspecified || Kind == dateTime.Kind)
            {
                return dateTime;
            }

            if (Kind == DateTimeKind.Local && dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime();
            }

            if (Kind == DateTimeKind.Utc && dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }

            return DateTime.SpecifyKind(dateTime, Kind);
        }
    }

    public class ClockOptions
    {
        /// <summary>
        /// Default: <see cref="DateTimeKind.Unspecified"/>
        /// </summary>
        public DateTimeKind Kind { get; set; }

        public ClockOptions()
        {
            Kind = DateTimeKind.Unspecified;
        }
    }
}
