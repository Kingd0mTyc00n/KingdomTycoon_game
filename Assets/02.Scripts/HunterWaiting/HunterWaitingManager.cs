using System.Collections.Generic;
using UnityEngine;

public class HunterWaitingManager : MonoBehaviour
{
    public GameObject hunterWaitingUI; // Assign in inspector
    public Transform hunterPanel; // Assign in inspector

    public List<HunterData> hunters; // List of hunters to manage
    private void Start()
    {
        hunters = GameController.instance.hunters.hunters;
        hunterPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (hunters.Count / 3) * 400);
        foreach (var hunter in hunters)
        {
            GameObject hunterUI = Instantiate(hunterWaitingUI, hunterPanel);
            HunterUIWaiting uiComponent = hunterUI.GetComponent<HunterUIWaiting>();
            if (uiComponent != null)
            {
                uiComponent.SetHunterData(hunter);
            }
        }
    }
}
