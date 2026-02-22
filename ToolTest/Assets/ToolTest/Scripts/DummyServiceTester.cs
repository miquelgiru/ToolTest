using UnityEngine;

public class DummyServiceTester : MonoBehaviour
{
    public PlayersDataManager dataManager;

    [ContextMenu("List Players")]
    void ListPlayers() => dataManager.ListPlayers();

    [ContextMenu("Create Player")]
    void CreatePlayer() => dataManager.CreatePlayer();

    [ContextMenu("Delete Player")]
    void DeletePlayer() => dataManager.DeletePlayer("bNqxYgvLEJit7p13wPX5O7l9pAIH");
}
