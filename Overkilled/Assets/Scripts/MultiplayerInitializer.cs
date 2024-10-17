using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class MultiplayerInitializer : MonoBehaviour
{
    void Start()
    {
#if DEVELOPMENT_BUILD
        InitializeUnityAuthenticationBuild();
#else
        InitializeUnityAuthentication();
#endif
    }

    async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;

            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync("Test", "Passw0rd!");
        }
        else
        {
            LoadLobby();
        }
    }

    async void InitializeUnityAuthenticationBuild()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;

            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync("Test2", "Passw0rd!!");
        }
        else
        {
            LoadLobby();
        }
    }

    void AuthenticationService_SignedIn()
    {
        LoadLobby();
    }

    void LoadLobby()
    {
        Loader.LoadScene(Loader.Scene.SafeHouseScene);

        AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
    }
}
