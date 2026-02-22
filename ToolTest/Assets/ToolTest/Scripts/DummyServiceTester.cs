using UnityEngine;

public class DummyServiceTester : MonoBehaviour
{
    public PlayersDataManager dataManager;
    public string PlayerIDToDelete;

    [ContextMenu("List Players")]
    void ListPlayers() => dataManager.ListPlayers();

    [ContextMenu("Create Player")]
    void CreatePlayer() => dataManager.CreatePlayer();

    [ContextMenu("Delete Player")]
    void DeletePlayer() => dataManager.DeletePlayer(PlayerIDToDelete);
}
