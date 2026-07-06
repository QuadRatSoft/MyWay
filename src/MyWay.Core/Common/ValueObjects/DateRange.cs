using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Common.ValueObjects;

public sealed record DateRange
{
    public DateRange(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (endAt <= startAt)
        {
            throw new DomainException("Date range end must be later than start.");
        }

        StartAt = startAt;
        EndAt = endAt;
    }

    public DateTimeOffset StartAt { get; }

    public DateTimeOffset EndAt { get; }

    public bool Overlaps(DateRange other)
    {
        return StartAt < other.EndAt && other.StartAt < EndAt;
    }
}
