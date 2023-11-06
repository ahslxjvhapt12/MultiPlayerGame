using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingletone _clientPrefab;
    [SerializeField] private HostSingletone _hostPrefab;
    [SerializeField] private NetworkObject _playerPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            HostSingletone hostSingletone = Instantiate(_hostPrefab);
            hostSingletone.CreateHostNetworkObject(_playerPrefab); // 게임매니저 만들고 준비

            ClientSingletone clientSingletone = Instantiate(_clientPrefab);
            bool authenticated = await clientSingletone.CreateClient();

            // 여기까지 성공하면 메뉴씬으로 이동한다.

            if (authenticated)
            {
                ClientSingletone.Instance.GameManager.GotoMenu();
            }

        }
    }
}
