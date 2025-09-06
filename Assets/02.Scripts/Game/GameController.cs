using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        InitializeData();
    }

    public HunterDatabase hunters;
    public ItemDatabase items;
    public EnemyDatabase enemies;
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerPrefs.DeleteAll();
            Debug.LogError("data deleted");
        }
    }


    private void InitializeData()
    {
        if (!PlayerPrefs.HasKey(GameData.PP_USER_DATA))
        {
            UserData.UserDeepData = new UserDeepData();
            HunterData newHunter = HunterData.CreateFromSO(hunters.hunters[0]);

            UserData.UserDeepData.HuntersData = new List<HunterData>
            {
                newHunter
            };

            //initialize user datas
            UserData.UserDeepData.Coins = 0;
            UserData.UserDeepData.Gems = 0;
            UserData.UserDeepData.Soul = 0;
            UserData.UserDeepData.Fool = 0;
            UserData.UserDeepData.Shard = 0;

            UserData.UserDeepData.HuntersNum = 0;

            UserData.UserDeepData.InventorySize = 15;
            UserData.SaveData();
        }
        else
        {
            Debug.Log("Load data");
            UserData.LoadData();
        }

    }

}
