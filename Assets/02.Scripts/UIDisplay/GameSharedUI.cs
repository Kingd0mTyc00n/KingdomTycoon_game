using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSharedUI : MonoBehaviour
{
    public static GameSharedUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] public TextMeshProUGUI coinsUIText;
    [SerializeField] public TextMeshProUGUI gemsUIText;
    [SerializeField] public TextMeshProUGUI soulUIText;
    [SerializeField] public TextMeshProUGUI foolUIText;
    [SerializeField] public TextMeshProUGUI shardUIText;
    [SerializeField] public TextMeshProUGUI hunterNum;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerPrefs.HasKey(GameData.PP_USER_DATA));
        UpdateUIText();
    }

    public void UpdateUIText()
    {
        SetUIText(coinsUIText, UserData.UserDeepData.Coins);
        SetUIText(gemsUIText, UserData.UserDeepData.Gems);
        SetUIText(soulUIText, UserData.UserDeepData.Soul);
        SetUIText(foolUIText, UserData.UserDeepData.Fool);
        SetUIText(shardUIText, UserData.UserDeepData.Shard);
        SetHunterNumber(hunterNum, UserData.UserDeepData.HuntersNum);
    }

    void SetUIText(TextMeshProUGUI textMesh, int value)
    {
        if (value >= 1000 && value < 1000000)
            textMesh.text = string.Format("{0}K.{1}", (value / 1000), GetFirstDigitFromNumber(value / 1000));
        else if (value >= 1000000)
            textMesh.text = string.Format("{0}M.{1}", (value / 1000000), GetFirstDigitFromNumber(value / 1000000));
        else
            textMesh.text = value.ToString();
    }

    void SetHunterNumber(TextMeshProUGUI textMesh, int value)
    {
        textMesh.text = $"{value}/6";
    }

    int GetFirstDigitFromNumber(int number)
    {
        return int.Parse(number.ToString()[0].ToString());
    }
}
