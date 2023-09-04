using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private const string MenuSceneName = "Menu";

    public async Task<bool> InitAsync()
    {
        // �÷��̾�(����) ���� �κ�
        await UnityServices.InitializeAsync(); // ����Ƽ ���� �ʱ�ȭ

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            return true;
        }
        return false;
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}
