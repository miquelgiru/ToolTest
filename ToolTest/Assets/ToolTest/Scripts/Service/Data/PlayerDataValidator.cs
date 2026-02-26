using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlayerDataValidator
{
    private static List<ItemDataBase> itemsValidator;

    public static bool IsValidName(string name, bool allowUppercase = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }
        if (name.Length > 20 && name.Length < 4)
        {
            return false;
        }

        foreach (char c in name)
        {
            bool isValidChar = char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_';

            if (!isValidChar)
            {
                return false;
            }
            if (!allowUppercase && char.IsUpper(c))
            {
                return false;
            }
        }

            return true;
    }

    public static bool IsValidCoins(object value)
    {
        if (value is int coins)
        {
            return coins >= 0 && coins <= 999999;
        }

        return false;
    }

    public static bool IsValidLevel(object value)
    {
        if (value is int level)
        {
            return level > 0 && level <= 100;
        }

        return false;
    }

    public static bool AreValidItems(object value, out string itemError)
    {

        if (itemsValidator == null)
        {
            var dataBase = Resources.Load<ItemsDataBase>("Items/ItemsDataBase");

            if (dataBase != null)
            {
                itemsValidator = dataBase.dataBase.ToList();
            }
            else
            {
                itemError = "Items dataBase not found";
                return false;
            }
        }

        itemError = "Invalid items: ";
        string[] items = value as string[];

        if(items != null)
        {
            foreach (string item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (!IsValidItem(item))
                    {
                        itemError += $"({item})";
                        return false;
                    }
                }
            }

            return true;
        }
        else
        {
            itemError = "Invalid items format";
        }

        return false;
    }

    public static bool IsValidItem(object value)
    {

        if (value is string item)
        {
            return itemsValidator.Any(validator => validator.itemName.Equals(value));
        }

        return false;
    }

    public static bool ValidateDictionary(Dictionary<string, object> playerData, out string error)
    {
        foreach (var item in playerData)
        {
            switch (item.Key)
            {
                case "preset_name":
                    if (!IsValidName(item.Value.ToString()))
                    {
                        error = "Invalid preset name";
                        return false;
                    }
                    break;

                case "display_name":
                    if (!IsValidName(item.Value.ToString()))
                    {
                        error = "Invalid player name";
                        return false;
                    }
                    break;

                case "coins":
                    if (!IsValidCoins(item.Value))
                    {
                        error = "Invalid coins value";
                        return false;
                    }
                    break;

                case "level":
                    if (!IsValidLevel(item.Value))
                    {
                        error = "Invalid level value";
                        return false;
                    }
                    break;

                case "ab_group":
                    if (!IsValidName(item.Value.ToString()))
                    {
                        error = "Invalid abgroup value";
                        return false;
                    }
                    break;

                case "items":
                    if (!AreValidItems(item.Value, out error))
                    {
                        return false;
                    }
                    break;
            }
        }

        error = null;
        return true;
    }
}
