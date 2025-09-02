using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Thirdweb;
using Thirdweb.Unity;
using UnityEngine;

/// <summary>
/// Fixed version of ContractManager that addresses the OwnableUnauthorizedAccount issue
/// </summary>
public class ContractManager : MonoBehaviour
{
    public static ContractManager Instance { get; private set; }

    private string _contractAbiJson;
    private ulong ActiveChainId = 1328;

    private string _contractAddress = "0x3f28033374ecf16A90fDbc22ea8e2Bb6194671F7";

    [Header("Wallet Configuration")]
    [Tooltip("Use this if you have the contract owner's private key")]
    public string ownerPrivateKey = "710899143d0e93ddc3a35c6fa325a510424946ef69efb5b1ca59776302cd8981";

    [Tooltip("Use this if you have a different authorized wallet's private key")]
    public string authorizedPrivateKey = "710899143d0e93ddc3a35c6fa325a510424946ef69efb5b1ca59776302cd8981";

    [Tooltip("If true, will attempt to set the current wallet as game manager (requires owner permissions)")]
    public bool autoSetGameManager = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance across scenes
        }
        else
        {
            Destroy(this.gameObject); // Ensure only one instance exists
        }
        
        try
        {
            TextAsset characterAbiAsset = Resources.Load<TextAsset>("Contract");
            if (characterAbiAsset == null)
            {
                Debug.LogError("Failed to load DevilGambleGame ABI from Resources!");
                return;
            }
            _contractAbiJson = characterAbiAsset.text;
            
            // Log platform info for debugging
            Debug.Log($"Running on platform: {Application.platform}");
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Debug.Log("WebGL platform detected - using WebGL-optimized settings");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in ContractManager.Awake: {e.Message}");
        }
    }

    private bool IsWebGLPlatform()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }

    private async Task<IThirdwebWallet> CreateAuthorizedWallet()
    {
        try
        {
            var client = ThirdwebClient.Create(secretKey: "X37tt2x5W_o5cNx218uItn0ZfR3mhRF2mTNmAWYkTuRaa6hqwo23y0ZqPVvoYg6vI1_1Ve1LxlvqbVKjRcAAdQ");
            
            if (client == null)
            {
                Debug.LogError("Failed to create Thirdweb client");
                return null;
            }
            
            // Try to use owner private key first
            if (!string.IsNullOrEmpty(ownerPrivateKey))
            {
                Debug.Log("Using owner private key for wallet creation");
                return await PrivateKeyWallet.Create(client, ownerPrivateKey);
            }
            
            // Try authorized private key
            if (!string.IsNullOrEmpty(authorizedPrivateKey))
            {
                Debug.Log("Using authorized private key for wallet creation");
                return await PrivateKeyWallet.Create(client, authorizedPrivateKey);
            }
            
            // Fall back to original private key
            Debug.LogWarning("Using original private key - this may not have sufficient permissions");
            return await PrivateKeyWallet.Create(client, "389c5d956af04b61895d35af9fcd33b0f93140e71725906848a989d523542d26");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create authorized wallet: {e.Message}");
            throw; // Re-throw for caller to handle
        }
    }

    // public async Task<bool> SetGameManager(string gameManagerAddress)
    // {
    //     try
    //     {
    //         var ownerWallet = await CreateAuthorizedWallet();
    //         var contract = await ThirdwebManager.Instance.GetContract(
    //             address: _contractAddress,
    //             chainId: ActiveChainId,
    //             abi: _contractAbiJson);

    //         Debug.Log($"Setting game manager to: {gameManagerAddress}");

    //         var transactionReceipt = await contract.Write(
    //             ownerWallet,
    //             "setGameManager",
    //             BigInteger.Zero,
    //             new object[] { gameManagerAddress }
    //         );

    //         Debug.Log($"Game manager set successfully. Tx Hash: {transactionReceipt.TransactionHash}");
    //         return true;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Failed to set game manager: {e.Message}");
    //         return false;
    //     }
    // }

    public async Task<bool> DiagnosePermissions()
    {
        try
        {
            var wallet = await CreateAuthorizedWallet();
            string walletAddress = await wallet.GetAddress();
            
            var contract = await ThirdwebManager.Instance.GetContract(
                address: _contractAddress,
                chainId: ActiveChainId,
                abi: _contractAbiJson);

            Debug.Log("=== PERMISSION DIAGNOSIS ===");
            Debug.Log($"Current Wallet: {walletAddress}");

            // Check owner
            var owner = await contract.Read<string>("owner");
            bool isOwner = string.Equals(walletAddress, owner, StringComparison.OrdinalIgnoreCase);
            Debug.Log($"Contract Owner: {owner}");
            Debug.Log($"Is Owner: {isOwner}");

            // Check game manager
            // var gameManager = await contract.Read<string>("gameManager");
            // bool isGameManager = string.Equals(walletAddress, gameManager, StringComparison.OrdinalIgnoreCase);
            // Debug.Log($"Game Manager: {gameManager}");
            // Debug.Log($"Is Game Manager: {isGameManager}");

            // Check paused state
            var isPaused = await contract.Read<bool>("paused");
            Debug.Log($"Contract Paused: {isPaused}");

            if (isOwner) //|| isGameManager)
            {
                Debug.Log("✅ Wallet has sufficient permissions!");
                return true;
            }
            else
            {
                Debug.LogError("❌ Wallet lacks permissions!");
                Debug.LogError("SOLUTIONS:");
                Debug.LogError("1. Use the contract owner's private key");
                Debug.LogError("2. Have the owner set this wallet as game manager");
                Debug.LogError("3. Use a wallet that is already set as game manager");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Permission diagnosis failed: {e.Message}");
            return false;
        }
    }

    public void Check()
    {
        //_ = Mint(1);

        _ = GetAllCharacterNFT();
    }
    public void LoadGame(){
        SceneAddressableManager.Instance.LoadScene("MainGameScene");
    }
    public async Task<Dictionary<int, int>> GetAllCharacterNFT()
    {
        try
        {
            var activeWallet = ThirdwebManager.Instance.GetActiveWallet();
            if (activeWallet == null)
            {
                Debug.LogError("No active wallet found");
                return new Dictionary<int, int>();
            }

            string activeWalletAddress = await activeWallet.GetAddress();
            Debug.Log($"Active Wallet: {activeWalletAddress}");

            object result = await ReadContract(_contractAddress, _contractAbiJson, "getUserNFTs", new object[] { activeWalletAddress });

            Debug.Log($"Raw result type: {result?.GetType()}  value: {result}");

            object[] arr = null;
            if (result is object[] tmp) arr = tmp;
            else if (result is IEnumerable en && !(result is string))
            {
                arr = en.Cast<object>().ToArray();
            }

            if (arr == null || arr.Length < 2)
            {
                Debug.LogWarning("Unexpected contract result format — expected two arrays: [ids[], amounts[]]");
                return new Dictionary<int, int>();
            }

            var ids = ToIntList(arr[0]);
            var amounts = ToIntList(arr[1]);

            Debug.Log("Parsed IDs: " + string.Join(",", ids));
            Debug.Log("Parsed Amounts: " + string.Join(",", amounts));

            var dict = new Dictionary<int, int>();
            int n = Math.Min(ids.Count, amounts.Count);
            for (int i = 0; i < n; i++)
            {
                dict[ids[i]] = amounts[i];
            }

            Debug.Log($"GetAllCharacterNFT -> parsed {dict.Count} entries");
            return dict;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in GetAllCharacterNFT: {e}");
            return new Dictionary<int, int>();
        }
    }


    public async Task<string> Mint(int numMint)
    {
        try
        {
            // Get active wallet first to avoid potential issues
            var activeWallet = ThirdwebManager.Instance.GetActiveWallet();
            if (activeWallet == null)
            {
                Debug.LogError("No active wallet found");
                return null;
            }

            string activeWalletAddress = await activeWallet.GetAddress();
            Debug.Log($"Active Wallet: {activeWalletAddress}");

            // First diagnose permissions
            // bool hasPermissions = await DiagnosePermissions();
            // if (!hasPermissions)
            // {
            //     Debug.LogError("Cannot create character: insufficient permissions");
            //     return null;
            // }

            var wallet = await CreateAuthorizedWallet();
            string walletAddress = await wallet.GetAddress();
            Debug.Log($"Using Wallet Address: {walletAddress}");

            // Auto-set game manager if requested and we have owner permissions
            // if (autoSetGameManager)
            // {
            //     bool setGameManagerResult = await SetGameManager(walletAddress);
            //     if (!setGameManagerResult)
            //     {
            //         Debug.LogWarning("Failed to set game manager, but continuing...");
            //     }
            // }

            var contract = await ThirdwebManager.Instance.GetContract(
                address: _contractAddress,
                chainId: ActiveChainId,
                abi: _contractAbiJson);

            if (contract == null)
            {
                Debug.LogError("Failed to get contract");
                return null;
            }

            BigInteger weiValue = BigInteger.Zero;

            var transactionReceipt = await contract.Write(
                wallet,
                "gachaMint",
                weiValue,
                new object[]
                {
                    activeWalletAddress,
                    numMint
                }
            );

            if (transactionReceipt != null && !string.IsNullOrEmpty(transactionReceipt.TransactionHash))
            {
                Debug.Log("✅ Character mint successfully! Tx Hash: " + transactionReceipt.TransactionHash);
                return transactionReceipt.TransactionHash;
            }
            else
            {
                Debug.LogError("Transaction receipt is null or invalid");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error creating character: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");

            if (e.Message.Contains("OwnableUnauthorizedAccount"))
            {
                Debug.LogError("This error confirms the wallet lacks owner/game manager permissions!");
                Debug.LogError("Please use DiagnosePermissions() to get specific guidance.");
            }
            
            // Return null instead of throwing for WebGL compatibility
            return null;
        }
    }



    public async Task<object[]> ReadContract(string contractAddress, string abiJson, string functionName, object[] parameters = null)
    {
        try
        {
            var contract = await ThirdwebManager.Instance.GetContract(
                address: contractAddress,
                chainId: ActiveChainId,
                abi: abiJson);

            print(contract);

            var result = await contract.Read<object[]>(
                functionName,
                parameters
            );
            print(result.ToString());
            
            var jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.Log("Full Result:\n" + jsonResult);

            return result;

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading contract: {e.Message}");
            return null;
        }
    }


    // WebGL-safe method to initialize without auto-minting
    public void InitializeForWebGL()
    {
        if (IsWebGLPlatform())
        {
            Debug.Log("Initializing ContractManager for WebGL platform");
            // Don't auto-mint on WebGL, wait for explicit user action
        }
    }


    private List<int> ToIntList(object input)
    {
        var list = new List<int>();
        if (input == null) return list;

        // If it's already object[]
        if (input is object[] objArr)
        {
            foreach (var o in objArr) if (TryConvertToInt(o, out var v)) list.Add(v);
            return list;
        }

        // If it's any IEnumerable (List<object>, BigInteger[], JArray, etc.), but not a string
        if (input is IEnumerable enumerable && !(input is string))
        {
            foreach (var o in enumerable)
                if (TryConvertToInt(o, out var v))
                    list.Add(v);
            return list;
        }

        // Single scalar value
        if (TryConvertToInt(input, out var single)) list.Add(single);
        return list;
    }

    private bool TryConvertToInt(object o, out int value)
    {
        value = 0;
        if (o == null) return false;
        try
        {
            switch (o)
            {
                case int i: value = i; return true;
                case long l: value = Convert.ToInt32(l); return true;
                case uint ui: value = Convert.ToInt32(ui); return true;
                case ulong ul: value = Convert.ToInt32(ul); return true;
                case short s: value = s; return true;
                case byte b: value = b; return true;
                case BigInteger bi: value = (int)bi; return true; // watch overflow
                case string s when int.TryParse(s, out var si): value = si; return true;
            }

            // handle JSON-like objects by parsing ToString()
            var tname = o.GetType().Name;
            if (tname.StartsWith("J") || tname.Contains("Json"))
            {
                if (int.TryParse(o.ToString(), out var parsed)) { value = parsed; return true; }
            }

            // fallback
            value = Convert.ToInt32(o);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"TryConvertToInt failed for '{o}' ({o?.GetType()}): {ex.Message}");
            return false;
        }
    }
}


