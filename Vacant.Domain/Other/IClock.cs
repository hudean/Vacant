using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain
{
    public interface IClock
    {
        /// <summary>
        /// Gets Now.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets kind.
        /// </summary>
        DateTimeKind Kind { get; }

        /// <summary>
        /// Is that provider supports multiple time zone.
        /// </summary>
        bool SupportsMultipleTimezone { get; }

        /// <summary>
        /// Normalizes given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime">DateTime to be normalized.</param>
        /// <returns>Normalized DateTime</returns>
        DateTime Normalize(DateTime dateTime);
    }
}
