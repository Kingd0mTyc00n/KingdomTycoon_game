using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;


public class HunterUIWaiting : MonoBehaviour
{
    private HunterData data;

    public Image iconHunter;
    public TextMeshProUGUI textNameHunter;
    public TextMeshProUGUI textInfoHunter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetHunterData(HunterData hunterData)
    {
        data = hunterData;
        LoadHunterSprite(data.SpritePath, sprite =>
        {
            iconHunter.sprite = sprite;
        });
        textNameHunter.text = data.Name;
        textInfoHunter.text = data.HunterType.ToString();
        SwitchHunterType();
    }

    public void LoadHunterSprite(string address, Action<Sprite> onLoaded)
    {
        Addressables.LoadAssetAsync<Sprite>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                onLoaded?.Invoke(handle.Result);
        };
    }

    public void SwitchHunterType()
    {
        switch (data.HunterType)
        {
            case HunterType.Common:
                textInfoHunter.color = Color.white;
                break;
            case HunterType.Rare:
                textInfoHunter.color = Color.green;
                break;
            case HunterType.Epic:
                textInfoHunter.color = Color.yellow;
                break;
            case HunterType.Legendary:
                textInfoHunter.color = Color.red;
                break;
            default:
                textInfoHunter.color = Color.white;
                break;
        }
    }
}
