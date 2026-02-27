using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class QAToolController : MonoBehaviour
{
    [SerializeField] PlayersDataManager dataManager;
    
    //UI Tool
    private UIDocument document;

    private TabView tabView;

    //Players List Tab
    private Tab listTab;
    private Button modifyPlayer;
    private Button deletePlayer;
    private ScrollView listView;

    //Create Player Tab
    private Tab createTab;
    private Button saveButton;
    private Button addItemButton;
    private ScrollView presetView;
    private ScrollView presetsScrollView;

    //Player Data
    private VisualElement presetContainer;
    private VisualElement nameContainer;
    private VisualElement levelContainer;
    private VisualElement coinsContainer;
    private VisualElement abgroupContainer;

    //Items Panel
    private Label itemsPanel;

    private Dictionary<string, PlayerProfile> playersStoredData;
    private Dictionary<string, Foldout> playersFoldouts;
    private string currentPlayerSelected = null;

    [Header("UI Templates")]
    [SerializeField] private VisualTreeAsset playerFold;
    [SerializeField] private VisualTreeAsset playerInputField;



    private void Awake()
    {
        document = GetComponent<UIDocument>();
        tabView = document.rootVisualElement.Q("qa-tool") as TabView;

        listTab = document.rootVisualElement.Q("list-tab") as Tab;
        modifyPlayer = document.rootVisualElement.Q("btn-modify") as Button;
        deletePlayer = document.rootVisualElement.Q("btn-delete") as Button;
        listView = document.rootVisualElement.Q("scroll-view-players-list") as ScrollView;

        createTab = document.rootVisualElement.Q("create-tab") as Tab;
        saveButton = document.rootVisualElement.Q("btn-save") as Button;
        addItemButton = document.rootVisualElement.Q("btn-add-items") as Button;
        presetView = document.rootVisualElement.Q("scroll-view-presets") as ScrollView;
        presetsScrollView = document.rootVisualElement.Q("scroll-view-presets") as ScrollView;

        presetContainer = document.rootVisualElement.Q("preset-field");
        nameContainer = document.rootVisualElement.Q("player-field");
        levelContainer = document.rootVisualElement.Q("level-field");
        coinsContainer = document.rootVisualElement.Q("coins-field");
        abgroupContainer = document.rootVisualElement.Q("abgroup-field");
        itemsPanel = document.rootVisualElement.Q("lbl-create-items-field") as Label;

        playersStoredData = new Dictionary<string, PlayerProfile>();
        playersFoldouts = new Dictionary<string, Foldout>();
    }

    private void Start()
    {
        SubscribeToEvents();
        EnablePlayerModify(false);
        CreatePresetButtons();
        LoadPlayersList();
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

    private void SubscribeToEvents()
    {
        modifyPlayer.RegisterCallback<ClickEvent>(OnModifyPlayer);
        deletePlayer.RegisterCallback<ClickEvent>(OnDeletePlayer);
        saveButton.RegisterCallback<ClickEvent>(OnSavePlayer);
        tabView.RegisterCallback<ClickEvent>(OnCreatePlayerTab);
    }

    private async Task LoadPlayersList()
    {
        playersStoredData.Clear();
        playersFoldouts.Clear();

        playersStoredData = await dataManager.GetPlayersProfileData();

        foreach (string playerId in playersStoredData.Keys)
        {
            TemplateContainer container = playerFold.Instantiate();
            Foldout foldout = container.Q<Foldout>("fold-player-data");
            foldout.text = playersStoredData[playerId].DisplayName;
            foldout.Q<Label>("lbl-player-name").text = playersStoredData[playerId].DisplayName;
            foldout.Q<Label>("lbl-player-preset").text = playersStoredData[playerId].PresetName;
            foldout.Q<Label>("lbl-player-level").text = playersStoredData[playerId].Level.ToString();
            foldout.Q<Label>("lbl-player-coins").text = playersStoredData[playerId].Coins.ToString();
            foldout.Q<Label>("lbl-player-group").text = playersStoredData[playerId].ABGroup;

            Label lblItems = foldout.Q<Label>("lbl-player-items") as Label;
            foreach(string item in playersStoredData[playerId].Items)
            {
                lblItems.text += (item + ", ");
            }

            foldout.value = false;
            listView.Insert(0, foldout);
            playersFoldouts[playerId] = foldout;
            foldout.RegisterCallback<ClickEvent>(OnPlayerFromListSelected);
        }
    }

    private void EnablePlayerModify(bool enable)
    {
        modifyPlayer.visible = enable;
        deletePlayer.visible = enable;
    }

    private void ReloadPlayersList()
    {
        listView.Clear();
        LoadPlayersList();
    }

    private void UpdateCreatePanel(PlayerProfile profile)
    {
        if (profile != null)
        {
            presetContainer.Q<TextField>("field-player-item").value = profile.PresetName;
            nameContainer.Q<TextField>("field-player-item").value = profile.DisplayName;
            levelContainer.Q<TextField>("field-player-item").value = profile.Level.ToString();
            coinsContainer.Q<TextField>("field-player-item").value = profile.Coins.ToString();
            abgroupContainer.Q<TextField>("field-player-item").value = profile.ABGroup;

            itemsPanel.text = string.Empty;
            foreach (string item in profile.Items)
            {
                itemsPanel.text += (item + ", ");
            }
        }
    }

    private void CreatePresetButtons()
    {
        PlayerPresets presets = Resources.Load<PlayerPresets>("Presets/Presets");

        for (int i = 0; i < presets.profiles.Length; i++)
        {
            PlayerProfile profile = presets.profiles[i];
            Button btn = new Button();
            btn.text = profile.PresetName;
            btn.RegisterCallback<ClickEvent>((ev) => UpdateCreatePanel(profile));
            presetsScrollView.Add(btn);
        }
    }

    #region Callbacks
    private void OnModifyPlayer(ClickEvent evt)
    {
        if (!string.IsNullOrEmpty(currentPlayerSelected))
        {
            presetsScrollView.parent.visible = false;
            tabView.selectedTabIndex = createTab.tabIndex;
            UpdateCreatePanel(playersStoredData[currentPlayerSelected]);
            currentPlayerSelected = null;
        }
    }

    private void OnDeletePlayer(ClickEvent evt)
    {
        if (!string.IsNullOrEmpty(currentPlayerSelected))
        {
            dataManager.DeletePlayer(currentPlayerSelected);
            ReloadPlayersList();
        }
    }

    private void OnPlayerFromListSelected(ClickEvent evt)
    {
        Foldout clickedFoldout = evt.currentTarget as Foldout;

        if (currentPlayerSelected != null && playersFoldouts[currentPlayerSelected] != clickedFoldout)
        {
            playersFoldouts[currentPlayerSelected].value = false;
        }
        if (clickedFoldout.value)
        {
            currentPlayerSelected = null;
        }
        else
        {
            currentPlayerSelected = playersFoldouts.Where((p) => p.Value == clickedFoldout).First().Key;
        }

        EnablePlayerModify(!clickedFoldout.value);
    }

    private void OnCreatePlayerTab(ClickEvent evt)
    {
        if(tabView.selectedTabIndex != createTab.tabIndex)
        {
            presetsScrollView.parent.visible = true;
        }
    }

    private async void OnSavePlayer(ClickEvent evt)
    {
        await dataManager.SavePlayerData(currentPlayerSelected, GetDataToSave());
        await LoadPlayersList();
        tabView.selectedTabIndex = listTab.tabIndex;
    }


    #endregion    
    
    private PlayerProfile GetDataToSave()
    {
        PlayerProfile profile = new PlayerProfile();
        profile.PresetName = presetContainer.Q<TextField>("field-player-item").value;
        profile.DisplayName = nameContainer.Q<TextField>("field-player-item").value;
        profile.Level = int.Parse(levelContainer.Q<TextField>("field-player-item").value);
        profile.Coins = int.Parse(coinsContainer.Q<TextField>("field-player-item").value);
        profile.ABGroup = abgroupContainer.Q<TextField>("field-player-item").value;
        profile.Items = ConvertRawItemsData(itemsPanel.text);

        return profile;
    }

    private string[] ConvertRawItemsData(string items)
    {
        List<string> ret = itemsPanel.text.Split(", ", System.StringSplitOptions.RemoveEmptyEntries).ToList();
        ret.RemoveAt(ret.Count - 1);
        return ret.ToArray();
    }
}
