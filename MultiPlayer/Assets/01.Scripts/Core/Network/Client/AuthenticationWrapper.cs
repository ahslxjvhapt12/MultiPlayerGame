using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}

public class AuthenticationWrapper
{
    public static AuthState State { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (State == AuthState.Authenticated)
        {
            return State;
        }

        State = AuthState.Authenticating;

        int tries = 0;
        while (State == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // 지금 현재 로그인도 되었고, 승인도 끝난 상태인거
            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                State = AuthState.Authenticated;
                break;
            }
            tries++;
            await Task.Delay(1000); // 1초에 한번씩 인증시도
        }

        return State;
    }
}
