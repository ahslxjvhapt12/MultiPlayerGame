using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class TestScreen : MonoBehaviour
{
    private UIDocument _uiDocument;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _uiDocument.rootVisualElement.Q<Button>("HostBtn").RegisterCallback<ClickEvent>(HandleClientClick);
        _uiDocument.rootVisualElement.Q<Button>("ClientBtn").RegisterCallback<ClickEvent>(HandleHostClick);
    }

    private void HandleClientClick(ClickEvent evt)
    {
        NetworkManager.Singleton.StartHost();
    }

    private void HandleHostClick(ClickEvent evt)
    {
        NetworkManager.Singleton.StartClient();
    }
}
