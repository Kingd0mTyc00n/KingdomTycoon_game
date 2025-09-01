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
    }

    public HunterDatabase hunters;
    public ItemDatabase items;
    public EnemyDatabase enemies;
    private void Start()
    {
        if (!PlayerPrefs.HasKey(GameData.PP_USER_DATA))
        {
            InitializeData();
            Debug.Log("Created new data !!!");
        }
        else
        {
            Debug.Log("Load data");
            UserData.LoadData();
        }
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
        UserData.UserDeepData = new UserDeepData();
        //Test display Hunter 
        UserData.UserDeepData.HuntersData = new List<HunterData>();
        HunterData hunter = hunters.hunters[0];
        UserData.UserDeepData.HuntersData.Add(hunter);

        UserData.UserDeepData.InventorySize = 15;
        UserData.SaveData();
    }


}
