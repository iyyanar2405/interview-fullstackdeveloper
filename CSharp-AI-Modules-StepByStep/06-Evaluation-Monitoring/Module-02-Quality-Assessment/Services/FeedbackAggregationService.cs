using System.Collections.Concurrent;
using Module_02_Quality_Assessment.Models;

namespace Module_02_Quality_Assessment.Services;

/// <summary>
/// Stores human feedback for evaluated responses and provides quick aggregates for review flows.
/// </summary>
public sealed class FeedbackAggregationService
{
    private readonly ConcurrentDictionary<string, FeedbackBucket> _feedback = new(StringComparer.OrdinalIgnoreCase);
    private const int MaxCommentsPerBucket = 25;

    public FeedbackAggregate Submit(FeedbackSubmission submission)
    {
        if (string.IsNullOrWhiteSpace(submission.ResponseId))
        {
            throw new ArgumentException("ResponseId is required", nameof(submission));
        }

        var bucket = _feedback.GetOrAdd(submission.ResponseId, _ => new FeedbackBucket());
        var aggregate = bucket.Add(submission);
        return aggregate with { ResponseId = submission.ResponseId };
    }

    public FeedbackAggregate? GetAggregate(string responseId)
    {
        return _feedback.TryGetValue(responseId, out var bucket)
            ? bucket.ToAggregate(responseId)
            : null;
    }

    private sealed record FeedbackBucket
    {
        private readonly object _sync = new();
        private double _ratingSum;
        private int _ratingCount;
        private readonly Queue<string> _recentComments = new();

        public FeedbackAggregate Add(FeedbackSubmission submission)
        {
            lock (_sync)
            {
                _ratingSum += submission.Rating;
                _ratingCount++;

                if (!string.IsNullOrWhiteSpace(submission.Comments))
                {
                    _recentComments.Enqueue(submission.Comments!);
                    while (_recentComments.Count > MaxCommentsPerBucket)
                    {
                        _recentComments.Dequeue();
                    }
                }

                return ToAggregate(string.Empty);
            }
        }

        public FeedbackAggregate ToAggregate(string responseId)
        {
            lock (_sync)
            {
                var average = _ratingCount == 0 ? 0 : _ratingSum / _ratingCount;
                return new FeedbackAggregate
                {
                    ResponseId = responseId,
                    AverageRating = Math.Round(average, 2),
                    RatingCount = _ratingCount,
                    RecentComments = _recentComments.Reverse().ToArray()
                };
            }
        }
    }
}
