using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HunterUI : MonoBehaviour
{
    private HunterData hunterData;

    [SerializeField] private TextMeshProUGUI hunterNameText;
    [SerializeField] private TextMeshProUGUI hunterLevelText;
    [SerializeField] private Image hunterImage;

    [SerializeField] private Button statsBtn;
    [SerializeField] private Button locationBtn;

    private void Start()
    {
        statsBtn.onClick.AddListener(() => OpenStats());
        locationBtn.onClick.AddListener(() => MoveCameraToLocation());
    }

    public void SetData(HunterData newHunterData)
    {
        this.hunterData = newHunterData;

        hunterNameText.text = hunterData.Name;
        //hunterImage.sprite = GameController.instance.hunters.hunters[hunterData.Id].sprite;
        hunterLevelText.text = $"Lv.{hunterData.Level} <color=#00FF00>{hunterData.HunterType}</color>";
    }

    private void OpenStats()
    {
        var menuIns = MenuManager.instance;
        menuIns.OpenScreen("HunterStats");
        menuIns.CloseAllHUDBar();
    }

    private void MoveCameraToLocation()
    {

    }
}
