using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BootstrapScreen : MonoBehaviour
{
    private UIDocument _uIDocument;
    private TextField _nameTextField;
    private Button _connectBtn;

    public const string PlayerNameKey = "PlayerName";

    private void Awake()
    {
        _uIDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        var root = _uIDocument.rootVisualElement;
        _nameTextField = root.Q<TextField>("name-text-field");
        _nameTextField.RegisterValueChangedCallback<string>(OnNameChangeHandle);

        _connectBtn = root.Q<Button>("btn-connect");
        _connectBtn.SetEnabled(false);
        _connectBtn.RegisterCallback<ClickEvent>(OnConnectHandle);

        string name = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        ValidateUserName(name);
        _nameTextField.SetValueWithoutNotify(name);
    }

    private void OnNameChangeHandle(ChangeEvent<string> evt)
    {
        ValidateUserName(evt.newValue);
    }

    private void OnConnectHandle(ClickEvent evt)
    {
        // 플레이어 Prefs에다가 지금 입력한 이름을 다시 저장해주고
        PlayerPrefs.SetString(PlayerNameKey, _nameTextField.text);
        // NetBootstrapScene으로 넘어가준다
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ValidateUserName(string name)
    {
        // 이름을 알파벳 소문자 대문자 숫자만 사용해서 2글자 이상의 8글자 이하로 검증

        Regex regex = new Regex(@"^[a-zA-Z0-9]{2,8}$");
        _connectBtn.SetEnabled(regex.IsMatch(name));
    }
}
