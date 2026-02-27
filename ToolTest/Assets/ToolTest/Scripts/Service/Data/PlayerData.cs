using System;
using System.Collections.Generic;



#region Get Player
[System.Serializable]
public class PlayerDataContent
{
    public List<PlayerDataItem> results;
    public PlayerDataLinks links;
    public int sizeLimit;
    public int totalSize;
}

[System.Serializable]
public class PlayerDataItem
{
    public string key;
    public object value;
    public PlayerDataTimestamp modified;
    public PlayerDataTimestamp created;
    public int storedSize;
}

[System.Serializable]
public class PlayerDataTimestamp
{
    public DateTime date;
}

[System.Serializable]
public class PlayerDataLinks
{
    public string next;
}
#endregion


#region List Players
[Serializable]
public class PlayersListContent
{
    public List<PlayerContent> results;
    public PlayerLink links;
}

[Serializable]
public class PlayerContent
{
    public string id;
    public Dictionary<string, object> accessClasses;
}

[Serializable]
public class PlayerLink
{
    public string next;
}

#endregion
