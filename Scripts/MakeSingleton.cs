using UnityEngine;

/// <summary>
/// A generic base class for implementing the Singleton pattern in Unity. Ensures only one instance of a class exists 
/// and persists across scenes. Inherit this class to create singleton instances for any MonoBehaviour-derived class.
/// </summary>

public class MakeSingleton<T> : MonoBehaviour where T : MakeSingleton<T>
{
    // The single instance of the singleton
    public static T Instance { get; private set; }

    // Awake is called when the script is initialized
    protected virtual void Awake()
    {
        // Check if the instance already exists
        if (Instance == null)
        {
            // If not, set this instance as the singleton
            Instance = (T)this;

            // Prevent the instance from being destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // If another instance exists, destroy this one and log a warning
            Debug.LogWarning($"Duplicate singleton instance of {typeof(T)} detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}
