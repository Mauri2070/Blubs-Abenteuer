using UnityEngine.EventSystems;

// Help system interface using Unity events
public interface IHelpSystem : IEventSystemHandler
{
    void WrongInteraction();
    void RightInteraction();
    void NeutralInteraction();
    void DecreaseHelpBorder();
    void MiniGameStarted(HelpSystemEventData eventData);
    void MiniGameCompleted();
    void RegisterPerformance(HelpSystemPerformanceData eventData);
}
