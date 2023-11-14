using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _txtUsername;

    public void ConnectedToServer()
    {
        string name = _txtUsername.text;
        UserData userData = new UserData
        {
            username = name,

        };

        ClientSingleton.Instance.StartClient(userData);
    }
}
