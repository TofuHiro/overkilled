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
            AuthenticationService.Instance.SignInFailed += AuthenticationService_SignInFailed;

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

    void AuthenticationService_SignInFailed(RequestFailedException obj)
    {
        ReturnToMenu();
    }

    void LoadLobby()
    {
        Loader.Instance.LoadScene(Loader.Scene.SafeHouseScene, Loader.TransitionType.Constant, Loader.TransitionType.FadeIn);

        AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
    }

    void ReturnToMenu()
    {
        Loader.Instance.LoadScene(Loader.Scene.MainMenuScene, Loader.TransitionType.Constant, Loader.TransitionType.FadeIn);

        AuthenticationService.Instance.SignInFailed -= AuthenticationService_SignInFailed;
    }
}
