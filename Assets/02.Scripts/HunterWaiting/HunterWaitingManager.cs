using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HunterWaitingManager : ScreenUI
{
    public GameObject hunterWaitingUI; // Assign in inspector
    public Transform hunterPanel; // Assign in inspector


    public TextMeshProUGUI textHunterCount; // Text to display the count of hunters

    public List<HunterData> hunterDatas; // List of hunters to manage

    public List<HunterUIWaiting> hunters;
    private void Start()
    {
        hunterDatas = GameController.instance.hunters.hunters;
        textHunterCount.text = $"({hunters.Count}/100)";
    }

    public async void SingleGacha()
    {
        int roll = UnityEngine.Random.Range(1, 2); // 1 → 25
        HunterData hunter = GetHunterByNumber(roll);

        AddHunterToUI(hunter);


        HunterSpawner.Instance.AddHunterFromGacha(hunter);
        await ContractManager.Instance.Mint(1);
        await LoadFromContract();
    }
    public async void MultiGacha()
    {
        for (int i = 0; i < 10; i++)
        {
            int roll = UnityEngine.Random.Range(1, 26);
            HunterData hunter = GetHunterByNumber(roll);

            AddHunterToUI(hunter);

            HunterSpawner.Instance.AddHunterFromGacha(hunter);
        }
        await ContractManager.Instance.Mint(10);
        await LoadFromContract();
    }

    private void AddHunterToUI(HunterData hunterData)
    {
        HunterUIWaiting hunterUI = null;

        var inactive = hunters.FirstOrDefault(h => !h.gameObject.activeSelf);
        if (inactive != null)
        {
            hunterUI = inactive;
            hunterUI.gameObject.SetActive(true);
        }
        else
        {
            var prefab = Instantiate(hunterWaitingUI, hunterPanel);
            hunterUI = prefab.GetComponent<HunterUIWaiting>();
            hunters.Add(hunterUI);
        }

        hunterUI.SetHunterData(hunterData);
        ResizePanel();
    }

    private void ResizePanel()
    {
        var rt = hunterPanel.GetComponent<RectTransform>();
        textHunterCount.text = $"({hunters.Count}/100)";
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (hunters.Count / 3) * 400);
    }

    private async Task LoadFromContract()
    {
        // Get NFT ids with amounts
        var nftDict = await ContractManager.Instance.GetAllCharacterNFT();

        // Hide all existing hunters first
        foreach (var hunter in hunters)
        {
            hunter.gameObject.SetActive(false);
        }

        // Rebuild UI based on dictionary
        foreach (var kvp in nftDict) // kvp.Key = id, kvp.Value = amount
        {
            int id = kvp.Key;
            int amount = kvp.Value;

            var hunterData = hunterDatas.Find(h => h.Id == id);
            if (hunterData == null)
            {
                Debug.LogWarning($"Hunter ID {id} not found in hunterDatas");
                continue;
            }

            // Spawn 'amount' times
            for (int i = 0; i < amount; i++)
            {
                HunterUIWaiting hunterUI = null;

                // Try reuse an inactive slot if available
                var inactive = hunters.FirstOrDefault(h => !h.gameObject.activeSelf);
                if (inactive != null)
                {
                    hunterUI = inactive;
                    hunterUI.gameObject.SetActive(true);
                }
                else
                {
                    // Instantiate new one
                    var prefab = Instantiate(hunterWaitingUI, hunterPanel);
                    hunterUI = prefab.GetComponent<HunterUIWaiting>();
                    hunters.Add(hunterUI);
                }

                hunterUI.SetHunterData(hunterData);
            }
        }

        ResizePanel();
    }

    public override void Open()
    {
        base.Open();
        LoadFromContract();
    }

    public override void Close()
    {
        base.Close();
    }

    /// <summary>
    /// Support for gacha system
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public HunterData GetHunterByNumber(int number)
    {
        switch (number)
        {
            case <= 16:
                return GetHunterByType(HunterType.Common);
            case <= 21:
                return GetHunterByType(HunterType.Rare);
            case <= 24:
                return GetHunterByType(HunterType.Epic);
            case 25:
                return GetHunterByType(HunterType.Legendary);
            default:
                return GetHunterByType(HunterType.Common);
        }
    }

    public HunterData GetHunterByType(HunterType type)
    {
        List<HunterData> matchedHunters = hunterDatas.FindAll(h => h.HunterType == type);
        int randomIndex = UnityEngine.Random.Range(0, 1);//matchedHunters.Count
        return matchedHunters[randomIndex];
    }
}
