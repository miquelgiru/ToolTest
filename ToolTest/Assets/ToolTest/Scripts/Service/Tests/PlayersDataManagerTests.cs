using NUnit.Framework;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayersDataManagerTests
{
    private PlayersDataManager dataManager;
    private MockToolTestService mockService;
    private GameObject go;

    [SetUp]
    public void Setup()
    {
        go = new GameObject();
        dataManager = go.AddComponent<PlayersDataManager>();

        mockService = new MockToolTestService();
        dataManager.Initialize(mockService);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ===========================
    // CreatePlayer Tests
    // ===========================

    [Test]
    public async Task CreatePlayer_ReturnsTrue_WhenServiceReturnsId()
    {
        mockService.CreateResult = "player123";

        bool result = await dataManager.CreatePlayer(new Dictionary<string, object>());

        Assert.IsTrue(result);
    }

    [Test]
    public async Task CreatePlayer_ReturnsFalse_WhenServiceReturnsNull()
    {
        mockService.CreateResult = null;

        bool result = await dataManager.CreatePlayer(new Dictionary<string, object>());

        Assert.IsFalse(result);
    }

    // ===========================
    // GetPlayer Tests
    // ===========================

    [Test]
    public async Task GetPlayer_CachesResult()
    {
        mockService.GetPlayerResult = JObject.Parse(@"{
            'results': [
                { 'key':'display_name','value':'Tester' },
                { 'key':'level','value':'10' },
                { 'key':'coins','value':'100' }
            ]
        }");

        var profile = await dataManager.GetPlayer("1");

        Assert.AreEqual("Tester", profile.DisplayName);
        Assert.AreEqual(10, profile.Level);
        Assert.AreEqual(100, profile.Coins);
        Assert.IsTrue(dataManager.IsCached("1"));
    }

    [Test]
    public void GetPlayer_ThrowsException_WhenServiceThrows()
    {
        mockService.ThrowOnGetPlayer = true;

        Assert.ThrowsAsync<System.Exception>(async () => await dataManager.GetPlayer("1"));
    }

    [Test]
    public async Task GetPlayer_ReturnsCachedValue_OnSecondCall()
    {
        mockService.GetPlayerResult = JObject.Parse(@"{
            'results': [
                { 'key':'display_name','value':'Tester' }
            ]
        }");

        var firstCall = await dataManager.GetPlayer("1");
        var secondCall = await dataManager.GetPlayer("1");

        // Service result is not null, cache hit occurs
        Assert.AreSame(firstCall, secondCall);
    }

    // ===========================
    // DeletePlayer Tests
    // ===========================

    [Test]
    public async Task DeletePlayer_RemovesFromCache_WhenSuccessful()
    {
        mockService.GetPlayerResult = JObject.Parse(@"{
            'results': [
                { 'key':'display_name','value':'Tester' }
            ]
        }");

        await dataManager.GetPlayer("1");

        bool result = await dataManager.DeletePlayer("1");

        Assert.IsTrue(result);
        Assert.IsFalse(dataManager.IsCached("1"));
    }

    [Test]
    public async Task DeletePlayer_DoesNotThrow_WhenServiceFails()
    {
        mockService.DeleteResult = false;

        bool result = await dataManager.DeletePlayer("1");

        Assert.IsFalse(result);
    }

    // ===========================
    // SavePlayerData Tests
    // ===========================

    [Test]
    public async Task SavePlayerData_ReturnsTrue_WhenValidationPassesAndServiceSucceeds()
    {
        var profile = new PlayerProfile
        {
            DisplayName = "ValidName",
            PresetName = "ValidPreset",
            Level = 10,
            Coins = 500,
            ABGroup = "Control",
            Items = new string[] { "sling_power_1", "sling_power_2" }
        };

        mockService.SaveResult = true;

        bool result = await dataManager.SavePlayerData("1", profile);

        Assert.IsTrue(result);
    }

    [Test]
    public async Task SavePlayerData_ReturnsFalse_WhenValidationFails()
    {
        var profile = new PlayerProfile
        {
            // invalid
            DisplayName = "", 
            PresetName = "",
            Level = -1,
            Coins = -10,
            ABGroup = "",
            Items = new string[] { }
        };

        bool result = await dataManager.SavePlayerData("1", profile);

        Assert.IsFalse(result);
    }

    [Test]
    public void SavePlayerData_ThrowsException_WhenServiceThrows()
    {
        mockService.ThrowOnSavePlayerData = true;

        var profile = new PlayerProfile
        {
            DisplayName = "ValidName",
            PresetName = "ValidPreset",
            Level = 1,
            Coins = 1,
            ABGroup = "Control",
            Items = new string[] { "sling_power_2" }
        };

        Assert.ThrowsAsync<System.Exception>(async () => await dataManager.SavePlayerData("1", profile));
    }

    // ===========================
    // GetPlayersInfo Tests
    // ===========================

    [Test]
    public async Task GetPlayersInfo_ReturnsCloudSaveList()
    {
        mockService.CloudSaveResult = new[] { "p1", "p2" };

        var result = await dataManager.GetPlayersInfo();

        Assert.AreEqual(2, result.Length);
        Assert.Contains("p1", result);
        Assert.Contains("p2", result);
    }

    [Test]
    public async Task GetPlayersInfo_ReturnsEmpty_WhenNoPlayers()
    {
        mockService.CloudSaveResult = new string[0];

        var result = await dataManager.GetPlayersInfo();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }
}