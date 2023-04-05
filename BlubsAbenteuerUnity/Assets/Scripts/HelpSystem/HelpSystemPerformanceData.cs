using UnityEngine.EventSystems;

// event data for performance related events
public class HelpSystemPerformanceData : BaseEventData
{
    public enum Rating
    {
        Good, Normal, Bad
    }

    private Rating rating;
    public Rating StarRating
    {
        get { return rating; }
    }

    public HelpSystemPerformanceData(EventSystem eventSystem, Rating rating) : base(eventSystem)
    {
        this.rating = rating;
    }
}
