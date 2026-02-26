using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class QAToolController : MonoBehaviour
{
    [SerializeField] PlayersDataManager dataManager;
    [SerializeField] VisualTreeAsset playerPanel;
    private UIDocument document;

    private ScrollView playersListView;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        playersListView = document.rootVisualElement.Q("fld-players-list") as ScrollView;
    }

    private void Start()
    {
        UpdatePlayersList();
    }

    private async void UpdatePlayersList()
    {
        PlayersListInfo players = await dataManager.ListPlayers();

        if (players != null)
        {
            // Update existing children
            for (int i = 0; i < playersListView.contentContainer.childCount; i++)
            {
                var child = playersListView.contentContainer[i];

                if (i < players.PlayersList.Length)
                {
                    SetPlayerData(child, players.PlayersList[i]);
                    child.style.display = DisplayStyle.Flex;
                }
                else
                {
                    child.style.display = DisplayStyle.None;
                }
            }

            // Add new children if needed
            for (int i = playersListView.contentContainer.childCount; i < players.PlayersList.Length; i++)
            {
                var newItem = playerPanel.Instantiate();
                SetPlayerData(newItem, players.PlayersList[i]);
                playersListView.contentContainer.Add(newItem);
            }
        }
    }

    private void SetPlayerData(VisualElement element, PlayerProfile data)
    {
        element.Q<Label>("preset_name").text = data.PresetName;
        element.Q<Label>("display_name").text = data.DisplayName;
        element.Q<Label>("level").text = data.Level.ToString();
        element.Q<Label>("coins").text = data.Coins.ToString();
        element.Q<Label>("items").text = data.Items.ToString();
        element.Q<Label>("ab_group").text = data.ABGroup;
    }
}
