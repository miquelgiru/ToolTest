using UnityEngine;

[CreateAssetMenu(fileName = "ToolTest", menuName = "ToolTest/Items DataBase")]
public class ItemsDataBase : ScriptableObject
{
   public ItemDataBase[] dataBase;
}

[System.Serializable]
public class ItemDataBase
{
    public string itemName;
    public string itemDescription;
}
