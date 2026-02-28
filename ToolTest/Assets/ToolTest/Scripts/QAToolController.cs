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
    private ScrollView addItemsScrollView;
    private Button closeItemsPanel;
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
    private Dictionary<string, Button> itemButtonSelectors;
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
        addItemsScrollView = document.rootVisualElement.Q("scroll-items-selector") as ScrollView;
        closeItemsPanel = document.rootVisualElement.Q("bt-close-items-selector") as Button;
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
        itemButtonSelectors = new Dictionary<string, Button>();
    }

    private void Start()
    {
        SubscribeToEvents();
        EnablePlayerModify(false);
        CreatePresetButtons();
        CreateItemsButtonsSelector();
        LoadPlayersList();
    }

    private void SubscribeToEvents()
    {
        modifyPlayer.RegisterCallback<ClickEvent>(OnModifyPlayer);
        deletePlayer.RegisterCallback<ClickEvent>(OnDeletePlayer);
        saveButton.RegisterCallback<ClickEvent>(OnSavePlayer);
        tabView.RegisterCallback<ChangeEvent<int>>(OnCreatePlayerTab);
        addItemButton.RegisterCallback<ClickEvent>(OnAddItem);
        closeItemsPanel.RegisterCallback<ClickEvent>(OnCloseItem);
    }

    private void OnCloseItem(ClickEvent evt)
    {
        addItemsScrollView.parent.visible = false;
    }

    private void OnAddItem(ClickEvent evt)
    {
        addItemsScrollView.parent.visible = true;
    }

    private async Task LoadPlayersList()
    {
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
            for (int i = 0; i < playersStoredData[playerId].Items.Length; i++)
            {
                string item = playersStoredData[playerId].Items[i];
                lblItems.text += item;

                if(i < playersStoredData[playerId].Items.Length - 1)
                {
                    lblItems.text += '-';
                }
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
        foreach (Foldout fd in playersFoldouts.Values)
        {
            listView.Remove(fd);
        }
        listView.Clear();
        playersStoredData.Clear();
        playersFoldouts.Clear();
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
            for (int i = 0; i < profile.Items.Length; i++)
            {
                string item = profile.Items[i];
                itemsPanel.text += (item);

                if (i < profile.Items.Length - 1)
                {
                    itemsPanel.text += '-';
                }
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

    private void CreateItemsButtonsSelector()
    {
        ItemsDataBase db = Resources.Load<ItemsDataBase>("Items/ItemsDataBase");

        foreach(ItemDataBase item in db.dataBase)
        {
            Button btn = new Button();
            btn.text = item.itemName;
            itemButtonSelectors.Add(item.itemName, btn);
            addItemsScrollView.Add(btn);
            btn.RegisterCallback<ClickEvent>((ev) => 
            {
                if (itemsPanel.text != string.Empty)
                {
                    itemsPanel.text += '-';
                }
                itemsPanel.text += btn.text;
                addItemsScrollView.parent.visible = false;
            });
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
            //currentPlayerSelected = null;
        }
    }

    private void OnDeletePlayer(ClickEvent evt)
    {
        if (!string.IsNullOrEmpty(currentPlayerSelected))
        {
            dataManager.DeletePlayer(currentPlayerSelected);
            currentPlayerSelected = null;
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

    private void OnCreatePlayerTab(ChangeEvent<int> evt)
    {
        if(tabView.selectedTabIndex != createTab.tabIndex)
        {
            CleanCreatePanel();
            presetsScrollView.parent.visible = true;
            currentPlayerSelected = null;
        }
    }

    private async void OnSavePlayer(ClickEvent evt)
    {
        bool success = false;
        if (currentPlayerSelected == null)
        {
            PlayerProfile profile = GetDataToSave();

            var data = new Dictionary<string, object> {
                    { "display_name", profile.DisplayName },
                    { "preset_name", profile.PresetName },
                    { "level", profile.Level },
                    { "coins", profile.Coins },
                    { "items", profile.Items },
                    { "ab_group", profile.ABGroup }
                };

            await dataManager.CreatePlayer(data);
            success = true;
        }
        else
        {
             success = await dataManager.SavePlayerData(currentPlayerSelected, GetDataToSave());         
        }

        if (success)
        {
            ReloadPlayersList();
            tabView.selectedTabIndex = listTab.tabIndex;
            currentPlayerSelected = null;
        }
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
        List<string> ret = itemsPanel.text.Split("-", System.StringSplitOptions.RemoveEmptyEntries).ToList();
        return ret.ToArray();
    }

    private void CleanCreatePanel()
    {
        presetContainer.Q<TextField>("field-player-item").value = string.Empty;
        nameContainer.Q<TextField>("field-player-item").value = string.Empty;
        levelContainer.Q<TextField>("field-player-item").value = string.Empty;
        coinsContainer.Q<TextField>("field-player-item").value = string.Empty;
        abgroupContainer.Q<TextField>("field-player-item").value = string.Empty;
        itemsPanel.text = string.Empty;
    }
}
