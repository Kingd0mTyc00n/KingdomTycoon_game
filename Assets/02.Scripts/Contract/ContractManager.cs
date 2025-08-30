using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

    public async Task<bool> SetGameManager(string gameManagerAddress)
    {
        try
        {
            var ownerWallet = await CreateAuthorizedWallet();
            var contract = await ThirdwebManager.Instance.GetContract(
                address: _contractAddress,
                chainId: ActiveChainId,
                abi: _contractAbiJson);

            Debug.Log($"Setting game manager to: {gameManagerAddress}");

            var transactionReceipt = await contract.Write(
                ownerWallet,
                "setGameManager",
                BigInteger.Zero,
                new object[] { gameManagerAddress }
            );

            Debug.Log($"Game manager set successfully. Tx Hash: {transactionReceipt.TransactionHash}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to set game manager: {e.Message}");
            return false;
        }
    }

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
        _ = GetAllCharacterNFT();
    }

    public async Task GetAllCharacterNFT()
    {
        try
        {
            var activeWallet = ThirdwebManager.Instance.GetActiveWallet();
            if (activeWallet == null)
            {
                Debug.LogError("No active wallet found");
                return;
            }

            string activeWalletAddress = await activeWallet.GetAddress();
            Debug.Log($"Active Wallet: {activeWalletAddress}");

            // foreach (var character in GameManager.Instance.CharacterCardData.CharacterCardsList)
            // {
            //     if(character.id == "26")
            //     {
            //         await CreateCharacter(character);
            //     }
            // }

            var result = await ReadContract(_contractAddress, _contractAbiJson, "getUserNFTIds", new object[] { activeWalletAddress });
            //var result = await ReadContract(_contractAddress, _contractAbiJson, "getCharacter", new object[] { 2 });

            //var result = await Mint();

            //print(result.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in GetAllCharacterNFT: {e.Message}");
        }
    }


    public async Task<string> Mint()
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
            bool hasPermissions = await DiagnosePermissions();
            if (!hasPermissions)
            {
                Debug.LogError("Cannot create character: insufficient permissions");
                return null;
            }

            var wallet = await CreateAuthorizedWallet();
            string walletAddress = await wallet.GetAddress();
            Debug.Log($"Using Wallet Address: {walletAddress}");

            // Auto-set game manager if requested and we have owner permissions
            if (autoSetGameManager)
            {
                bool setGameManagerResult = await SetGameManager(walletAddress);
                if (!setGameManagerResult)
                {
                    Debug.LogWarning("Failed to set game manager, but continuing...");
                }
            }

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
                "adminMintCharacter",
                weiValue,
                new object[]
                {
                    activeWalletAddress,
                    2
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

    // Add timeout for WebGL operations
    public async Task<string> MintWithTimeout(int timeoutSeconds = 30)
    {
        if (IsWebGLPlatform())
        {
            Debug.Log("Using WebGL-optimized mint process");
        }

        var mintTask = Mint();
        var timeoutTask = Task.Delay(timeoutSeconds * 1000);

        var completedTask = await Task.WhenAny(mintTask, timeoutTask);

        if (completedTask == timeoutTask)
        {
            Debug.LogError("Mint operation timed out");
            return null;
        }

        return await mintTask;
    }
}
