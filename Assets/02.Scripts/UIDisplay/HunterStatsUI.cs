using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HunterStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hunterNameText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("EXP")]
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Image expFillImage;

    [Header("Stats Tab")]
    [SerializeField] private TextMeshProUGUI hunterClassText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI dpsParamText;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI critText;
    [SerializeField] private TextMeshProUGUI atkSPDText;
    [SerializeField] private TextMeshProUGUI evasionText;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthFillImage;

    [SerializeField] private float currentHealth;
    [SerializeField] private float expCap;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetData(HunterData hunterData)
    {
        hunterNameText.text = hunterData.Name;
        goldText.text = UserData.UserDeepData.Coins.ToString();

        expText.text = string.Format("EXP {0)/{1}",hunterData.Exp,expCap);
        //health fill
         
        hunterClassText.text = hunterData.HunterType.ToString();
        levelText.text = hunterData.Level.ToString();
        dpsParamText.text = hunterData.DPSParam.ToString();
        atkText.text = hunterData.ATK.ToString();
        defenseText.text = hunterData.Defense.ToString();
        critText.text = hunterData.Crit.ToString();
        atkSPDText.text = hunterData.ATKSPD.ToString();
        evasionText.text = hunterData.Evasion.ToString();

        healthText.text = string.Format("{0}/{1}",currentHealth,hunterData.Health);

    }
}
