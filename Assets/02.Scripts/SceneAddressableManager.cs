using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneAddressableManager : MonoBehaviour
{
    public static SceneAddressableManager Instance;

    // Store loaded scenes
    private Dictionary<string, SceneInstance> loadedScenes = new Dictionary<string, SceneInstance>();

    private void Awake()
    {
        // Make it singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Load scene by addressable name
    /// </summary>
    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (loadedScenes.ContainsKey(sceneName))
        {
            Debug.LogWarning("Scene already loaded: " + sceneName);
            return;
        }

        Addressables.LoadSceneAsync(sceneName, mode).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedScenes[sceneName] = handle.Result;
                Debug.Log("Scene loaded: " + sceneName);
            }
            else
            {
                Debug.LogError("Failed to load scene: " + sceneName);
            }
        };
    }

    /// <summary>
    /// Unload scene by name
    /// </summary>
    public void UnloadScene(string sceneName)
    {
        if (loadedScenes.TryGetValue(sceneName, out SceneInstance instance))
        {
            Addressables.UnloadSceneAsync(instance).Completed += _ =>
            {
                loadedScenes.Remove(sceneName);
                Debug.Log("Scene unloaded: " + sceneName);
            };
        }
        else
        {
            Debug.LogWarning("Scene not loaded: " + sceneName);
        }
    }
}
