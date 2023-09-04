using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostSingletone : MonoBehaviour
{
    private static HostSingletone _instance;

    public static HostSingletone Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType<HostSingletone>();

            if (_instance == null)
            {
                Debug.LogError("No Host Singleton");
            }

            return _instance;
        }
    }

    public HostGameManager GameManager { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
}
