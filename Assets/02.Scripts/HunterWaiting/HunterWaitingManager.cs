using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HunterWaitingManager : MonoBehaviour
{
    public GameObject hunterWaitingUI; // Assign in inspector
    public Transform hunterPanel; // Assign in inspector


    public TextMeshProUGUI textHunterCount; // Text to display the count of hunters

    public List<HunterData> hunterDatas; // List of hunters to manage
    
    public List<HunterData> hunters;
    private void Start()
    {
        hunterDatas = GameController.instance.hunters.hunters;
        textHunterCount.text = $"({hunters.Count}/100)";
    }

    public void TestSingleGacha()
    {
        int index = Random.RandomRange(0, hunterDatas.Count);
        var hunter = Instantiate(hunterWaitingUI, hunterPanel);
        hunter.GetComponent<HunterUIWaiting>().SetHunterData(hunterDatas[index]);
        hunters.Add(hunterDatas[index]);

        ResizePanel();
    }
    public void TestMultiGacha()
    {
        for (int i = 0; i < 10; i++)
        {
            int index = Random.RandomRange(0, hunterDatas.Count);
            var hunter = Instantiate(hunterWaitingUI, hunterPanel);
            hunter.GetComponent<HunterUIWaiting>().SetHunterData(hunterDatas[index]);
            hunters.Add(hunterDatas[index]);
        }

        ResizePanel();
    }
    private void ResizePanel()
    {
        var rt = hunterPanel.GetComponent<RectTransform>();
        textHunterCount.text = $"({hunters.Count}/100)";
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (hunters.Count/3) * 400);
    }
}
