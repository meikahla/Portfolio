using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{

    private void Start()
    {
        // Call your CloudScript function
        CallCloudScriptFunction("YourCloudScriptFunction", new { parameterName = "parameterValue" });
    }

    private void CallCloudScriptFunction(string functionName, object functionParameter)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = functionName,
            FunctionParameter = functionParameter,
            GeneratePlayStreamEvent = true // Set to true if you want to generate PlayStream events
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptSuccess, OnCloudScriptFailure);
    }

    private void OnCloudScriptSuccess(ExecuteCloudScriptResult result)
    {
        // Handle successful CloudScript execution
        Debug.Log("CloudScript executed successfully!");
        // Access result.Results for any data returned by your CloudScript function
    }

    private void OnCloudScriptFailure(PlayFabError error)
    {
        // Handle CloudScript execution failure
        Debug.LogError("CloudScript execution failed: " + error.GenerateErrorReport());
    }
    //    private IAppleAuthManager appleAuthManager;

    //    public static PlayFabManager ins;
    //    public string gameTitle;
    //    string deviceID;

    //    string authCode;

    //    private bool isLoggedIn = false;

    //    private UnityAction<LoginResult> _successLogin;
    //    private UnityAction<string> _successLink, _successUnlink;
    //    private UnityAction<PlayFabError> _errorCallback;
    //    private UnityAction<DateTime> _successGetTime;
    //    private UnityAction<GetUserDataResult> _successGetUserData;
    //    private UnityAction<GetUserInventoryResult> _successGetUserInventory;
    //    private UnityAction<UpdateUserDataResult> _successUpdateUserData;
    //    private UnityAction<UpdateUserTitleDisplayNameResult> _successUpdateUsername;
    //    private Coroutine _timerCoroutine;
    //    public GetPlayerProfileResult PlayerProfile;

    //    public DateTime ServerTime { get; private set; }
    //    public bool IsPlayerLogin => PlayFabClientAPI.IsClientLoggedIn();

    //    public static string PlayerId = string.Empty;
    //    public static readonly Dictionary<string, int> virtualCurrency = new Dictionary<string, int>();
    //    public static readonly List<PlayFab.ClientModels.ItemInstance> playerInventory = new List<PlayFab.ClientModels.ItemInstance>();
    //    public static readonly Dictionary<string, int> playerStats = new Dictionary<string, int>();
    //    public int targetFrameRate = 60;

    //    public void Awake()
    //    {
    //        MakeSingleton();

    //        deviceID = SystemInfo.deviceUniqueIdentifier;

    //        QualitySettings.vSyncCount = 0;
    //        Application.targetFrameRate = targetFrameRate;
    //    }

    //    #region Server Time Related
    //    public void GetServerTime(UnityAction<DateTime> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _successGetTime = onSuccess;
    //        _errorCallback = onError;
    //        PlayFabClientAPI.GetTime(new GetTimeRequest(), OnSuccessGetServerTime, OnError);
    //    }

    //    private void OnSuccessGetServerTime(GetTimeResult result)
    //    {
    //        ServerTime = result.Time.ToLocalTime();
    //        if (_timerCoroutine != null)
    //            StopCoroutine(_timerCoroutine);
    //        _timerCoroutine = StartCoroutine(IE_StartTimer());
    //        _successGetTime?.Invoke(result.Time);
    //    }

    //    private IEnumerator IE_StartTimer()
    //    {
    //        WaitForSecondsRealtime unscaleSecond = new WaitForSecondsRealtime(1);
    //        while (true)
    //        {
    //            ServerTime = ServerTime.AddSeconds(1f);
    //            yield return unscaleSecond;
    //        }
    //    }
    //    #endregion

    //    #region Login Related
    //    public void Login(UnityAction<LoginResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        GuestLogin(onSuccess, onError);
    //    }
    //    #endregion

    //    #region Create Account Related
    //    public void CreateAccount(LoginSuccessResult loginResult, UnityAction<LoginResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _successLogin = onSuccess;
    //        _errorCallback = onError;
    //        switch (loginResult.UserLoginType)
    //        {
    //            case LoginIdentityProvider.GooglePlayGames:
    //                PlayServicesLogin();
    //                break;
    //            case LoginIdentityProvider.Apple:
    //                AppleLogin(loginResult.IdToken);
    //                break;
    //            case LoginIdentityProvider.GooglePlay:
    //                GoogleLogin(loginResult.AuthCode);
    //                break;
    //            case LoginIdentityProvider.Facebook:
    //                FacebookLogin(loginResult.IdToken);
    //                break;
    //            default:
    //                GuestLogin(onSuccess, onError);
    //                break;
    //        }
    //    }

    //    private void FacebookLogin(string token)
    //    {
    //        var request = new LoginWithFacebookRequest
    //        {
    //            CreateAccount = true,
    //            AccessToken = token,
    //            TitleId = PlayFabSettings.TitleId
    //        };

    //        PlayFabClientAPI.LoginWithFacebook(request, GuestAccountLink, OnError);
    //    }

    //    private void GoogleLogin(string token)
    //    {
    //        var request = new LoginWithGoogleAccountRequest
    //        {
    //            CreateAccount = true,
    //            ServerAuthCode = token,
    //            TitleId = PlayFabSettings.TitleId
    //        };

    //        PlayFabClientAPI.LoginWithGoogleAccount(request, GuestAccountLink, OnError);
    //    }

    //    private void PlayServicesLogin()
    //    {
    //#if USE_GOOGLE_PLAY_SERVICE
    //        PlayGamesPlatform.Instance.GetAnotherServerAuthCode(true, OnGetServerAuthCode);

    //        void OnGetServerAuthCode(string token)
    //        {
    //            var request = new LoginWithGooglePlayGamesServicesRequest
    //            {
    //                CreateAccount = true,
    //                ServerAuthCode = token,
    //                TitleId = PlayFabSettings.TitleId
    //            };

    //            PlayFabClientAPI.LoginWithGooglePlayGamesServices(request, GuestAccountLink, OnError);
    //        }
    //#endif
    //    }


    //    private void AppleLogin(string token)
    //    {
    //        var request = new LoginWithAppleRequest
    //        {
    //            CreateAccount = true,
    //            IdentityToken = token,
    //            TitleId = PlayFabSettings.TitleId
    //        };

    //        PlayFabClientAPI.LoginWithApple(request, GuestAccountLink, OnError);
    //    }

    //    private void GuestLogin(UnityAction<LoginResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _successLogin = onSuccess;
    //        _errorCallback = onError;

    //#if UNITY_ANDROID
    //        var request = new LoginWithAndroidDeviceIDRequest
    //        {
    //            TitleId = PlayFabSettings.TitleId,
    //            AndroidDevice = SystemInfo.deviceModel,
    //            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
    //            CreateAccount = true,
    //        };
    //        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnSuccessLogin, OnError);

    //#elif UNITY_IOS
    //        var request = new LoginWithIOSDeviceIDRequest
    //        {
    //            TitleId = PlayFabSettings.TitleId,
    //            DeviceModel = SystemInfo.deviceModel,
    //            DeviceId = SystemInfo.deviceUniqueIdentifier,
    //            CreateAccount = true,
    //        };
    //        PlayFabClientAPI.LoginWithIOSDeviceID(request, OnSuccessLogin, OnError);
    //#endif
    //    }

    //    private void OnSuccessLogin(LinkAndroidDeviceIDResult result)
    //    {
    //        GuestLogin(_successLogin, _errorCallback);
    //    }

    //    private void OnSuccessLogin(LinkIOSDeviceIDResult result)
    //    {
    //        GuestLogin(_successLogin, _errorCallback);
    //    }

    //    private void OnSuccessLogin(LoginResult result)
    //    {
    //        GetPlayerProfile(result.PlayFabId);

    //        GetServerTime(OnSuccessGetServerTime, OnError);

    //        void OnSuccessGetServerTime(DateTime serverTime)
    //        {
    //            _successLogin?.Invoke(result);
    //        }
    //    }
    //    #endregion

    //    #region Get Player Data Related
    //    public void GetPlayerData(UnityAction<GetUserDataResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _successGetUserData = onSuccess;
    //        _errorCallback = onError;
    //        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetPlayerData, OnError);
    //    }

    //    private void OnGetPlayerData(GetUserDataResult result)
    //    {
    //        playerStats.Clear();

    //        if (result.Data != null && result.Data.ContainsKey("HighScore"))
    //        {
    //            playerStats.Add("HighScore", int.Parse(result.Data["HighScore"].Value));
    //            PlayerPrefs.SetInt("HighScore", int.Parse(result.Data["HighScore"].Value));
    //        }
    //        Debug.Log("Playfab data init successful");
    //        _successGetUserData?.Invoke(result);
    //    }

    //    public void GetPlayerInventory(UnityAction<GetUserInventoryResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    { 
    //        _successGetUserInventory = onSuccess;
    //        _errorCallback = onError;
    //        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetPlayerInventory, OnError);
    //    }

    //    private void OnGetPlayerInventory(GetUserInventoryResult result)
    //    {
    //        IAPManager.Ins.RefreshIAPItems();
    //#if UNITY_IOS
    //        StartCoroutine(NotificationManager.GetInstance().WaitForRegisterIOS());
    //#endif
    //        virtualCurrency.Clear();
    //        playerInventory.Clear();

    //        foreach (var pair in result.VirtualCurrency)
    //        {
    //            virtualCurrency.Add(pair.Key, pair.Value);
    //            PlayerPrefs.SetInt("Coins", pair.Value);
    //        }
    //        foreach (var item in result.Inventory)
    //        {
    //            playerInventory.Add(item);
    //            PlayerPrefs.SetString(item.ItemId, "unlocked");

    //            if (item.ItemId.Contains("lunarliondance_no_ads"))
    //            {
    //                AdsManager.Ins.RemoveAds(true);
    //                PlayerPrefs.SetInt("RemoveAds", 1);
    //                Debug.Log("remove ads exist");
    //            }
    //        }
    //        Debug.Log("Playfab inventory init successful");
    //        _successGetUserInventory?.Invoke(result);
    //    }

    //    public void GetPlayerProfile(string playFabId, Action<GetPlayerProfileResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _errorCallback = onError;
    //        GetPlayerProfileRequest request = new GetPlayerProfileRequest()
    //        {
    //            PlayFabId = playFabId,
    //            ProfileConstraints = new PlayerProfileViewConstraints()
    //            {
    //                ShowLinkedAccounts = true,
    //            }
    //        };

    //        PlayFabClientAPI.GetPlayerProfile(request, SuccessGetPlayerProfile, OnError);

    //        void SuccessGetPlayerProfile(GetPlayerProfileResult result)
    //        {
    //            PlayerProfile = result;
    //            onSuccess?.Invoke(PlayerProfile);
    //        }
    //    }
    //#endregion

    //    #region Update Player Data Related
    //    public void UpdateUsername(string username, UnityAction<UpdateUserTitleDisplayNameResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest()
    //        {
    //            DisplayName = username
    //        };

    //        _errorCallback = onError;
    //        _successUpdateUsername = onSuccess;
    //        PlayFabClientAPI.UpdateUserTitleDisplayName(request, UpdateUsernameSuccess, OnError);
    //    }

    //    public void UpdateUsernameSuccess(UpdateUserTitleDisplayNameResult result)
    //    {
    //        _successUpdateUsername.Invoke(result);
    //    }

    //    public void SavePlayerData(UnityAction<UpdateUserDataResult> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _successUpdateUserData = onSuccess;
    //        _errorCallback = onError;

    //        var request = new UpdateUserDataRequest
    //        {
    //            Data = new Dictionary<string, string>
    //            {
    //                { PlayFabConstants.PrefData, JsonConvert.SerializeObject(GameData.Instance.prefData) }
    //            }
    //        };

    //        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatePlayerData, OnError);
    //    }

    //    private void OnSuccessUpdatePlayerData(UpdateUserDataResult result)
    //    {
    //        Prefs.gameData = JsonConvert.SerializeObject(GameData.Instance.prefData);
    //        _successUpdateUserData?.Invoke(result);
    //    }
    //    #endregion

    //    #region Link Related
    //    public void LinkAccount(LoginSuccessResult loginResult = null, UnityAction<string> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _successLink = onSuccess;
    //        _errorCallback = onError;

    //        switch (loginResult.UserLoginType)
    //        {
    //            case LoginIdentityProvider.GooglePlayGames:
    //                PlayServicesLink();
    //                break;
    //            case LoginIdentityProvider.Apple:
    //                AppleLink(loginResult.IdToken);
    //                break;
    //            case LoginIdentityProvider.GooglePlay:
    //                GoogleLink(loginResult.AuthCode);
    //                break;
    //            case LoginIdentityProvider.Facebook:
    //                FacebookLink(loginResult.IdToken);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    private void FacebookLink(string token)
    //    {
    //        var request = new LinkFacebookAccountRequest
    //        {
    //            ForceLink = false,
    //            AccessToken = token
    //        };

    //        PlayFabClientAPI.LinkFacebookAccount(request, FacebookLinkSuccess, OnError);

    //        void FacebookLinkSuccess(LinkFacebookAccountResult result)
    //        => _successLink.Invoke("Facebook");
    //    }

    //    private void GoogleLink(string token)
    //    {
    //        var request = new LinkGoogleAccountRequest
    //        {
    //            ForceLink = false,
    //            ServerAuthCode = token
    //        };

    //        PlayFabClientAPI.LinkGoogleAccount(request, GoogleLinkSuccess, OnError);

    //        void GoogleLinkSuccess(LinkGoogleAccountResult result)
    //        => _successLink.Invoke("Google");
    //    }

    //    private void PlayServicesLink()
    //    {
    //#if USE_GOOGLE_PLAY_SERVICE
    //        PlayGamesPlatform.Instance.GetAnotherServerAuthCode(true, OnGetServerAuthCode); 

    //        void OnGetServerAuthCode(string token)
    //        {
    //            var request = new LinkGooglePlayGamesServicesAccountRequest
    //            {
    //                ForceLink = false,
    //                ServerAuthCode = token,
    //            };

    //            PlayFabClientAPI.LinkGooglePlayGamesServicesAccount(request, GooglePlayServicesLinkSuccess, OnError);

    //            void GooglePlayServicesLinkSuccess(LinkGooglePlayGamesServicesAccountResult result)
    //            => _successLink.Invoke("Play Services"); 
    //        }
    //#endif
    //    }

    //    private void AppleLink(string token)
    //    {
    //        var request = new LinkAppleRequest
    //        {
    //            ForceLink = false,
    //            IdentityToken = token,
    //        };

    //        PlayFabClientAPI.LinkApple(request, AppleLinkSuccess, OnError);

    //        void AppleLinkSuccess(EmptyResult result)
    //        => _successLink.Invoke("Apple");
    //    }

    //    private void GuestAccountLink(LoginResult result)
    //    {
    //#if UNITY_ANDROID
    //        var request = new LinkAndroidDeviceIDRequest
    //        {
    //            AndroidDevice = SystemInfo.deviceModel,
    //            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
    //            ForceLink = true,
    //        };
    //        PlayFabClientAPI.LinkAndroidDeviceID(request, OnSuccessLogin, OnError);
    //#elif UNITY_IOS
    //        var request = new LinkIOSDeviceIDRequest
    //        {
    //            DeviceModel = SystemInfo.deviceModel,
    //            DeviceId = SystemInfo.deviceUniqueIdentifier,
    //            ForceLink = true,
    //        };
    //        PlayFabClientAPI.LinkIOSDeviceID(request, OnSuccessLogin, OnError);
    //#endif
    //    }
    //    #endregion

    //    #region Unlink Related
    //    public void UnlinkAccount(LoginIdentityProvider unlinkService, UnityAction<string> onSuccess = null, UnityAction<PlayFabError> onError = null)
    //    {
    //        _successUnlink = onSuccess;
    //        _errorCallback = onError;

    //        switch (unlinkService)
    //        {
    //            case LoginIdentityProvider.GooglePlayGames:
    //                PlayServicesUnlink();
    //                break;
    //            case LoginIdentityProvider.Apple:
    //                AppleUnlink();
    //                break;
    //            case LoginIdentityProvider.GooglePlay:
    //                GoogleUnlink();
    //                break;
    //            case LoginIdentityProvider.Facebook:
    //                FacebookUnlink();
    //                break;
    //            default:
    //                GuestUnlink();
    //                break;
    //        }
    //    }

    //    private void GuestUnlink()
    //    {
    //#if UNITY_ANDROID
    //        var request = new UnlinkAndroidDeviceIDRequest();

    //        PlayFabClientAPI.UnlinkAndroidDeviceID(request, GuestUnlinkSuccess, OnError);

    //        void GuestUnlinkSuccess(UnlinkAndroidDeviceIDResult result)
    //        => _successUnlink.Invoke("Guest");
    //#elif UNITY_IOS
    //        var request = new UnlinkIOSDeviceIDRequest();

    //        PlayFabClientAPI.UnlinkIOSDeviceID(request, GuestUnlinkSuccess, OnError);

    //        void GuestUnlinkSuccess(UnlinkIOSDeviceIDResult result)
    //        => _successUnlink.Invoke("Guest");
    //#endif
    //    }

    //    private void FacebookUnlink()
    //    {
    //        var request = new UnlinkFacebookAccountRequest();

    //        PlayFabClientAPI.UnlinkFacebookAccount(request, FacebookUnlinkSuccess, OnError);

    //        void FacebookUnlinkSuccess(UnlinkFacebookAccountResult result)
    //        => _successUnlink.Invoke("Facebook");
    //    }

    //    private void GoogleUnlink()
    //    {
    //        var request = new UnlinkGoogleAccountRequest();

    //        PlayFabClientAPI.UnlinkGoogleAccount(request, GoogleUnlinkSuccess, OnError);

    //        void GoogleUnlinkSuccess(UnlinkGoogleAccountResult result)
    //        => _successUnlink.Invoke("Google");
    //    }

    //    private void PlayServicesUnlink()
    //    {
    //#if USE_GOOGLE_PLAY_SERVICE
    //        PlayGamesPlatform.Instance.GetAnotherServerAuthCode(true, OnGetServerAuthCode);

    //        void OnGetServerAuthCode(string token)
    //        {
    //            var request = new UnlinkGooglePlayGamesServicesAccountRequest();

    //            PlayFabClientAPI.UnlinkGooglePlayGamesServicesAccount(request, GooglePlayServicesUnlinkSuccess, OnError);

    //            void GooglePlayServicesUnlinkSuccess(UnlinkGooglePlayGamesServicesAccountResult result)
    //            => _successUnlink.Invoke("Play Services");
    //        }
    //#endif
    //    }

    //    private void AppleUnlink()
    //    {
    //        var request = new UnlinkAppleRequest();

    //        PlayFabClientAPI.UnlinkApple(request, AppleUnlinkSuccess, OnError);

    //        void AppleUnlinkSuccess(EmptyResponse result)
    //        => _successUnlink.Invoke("Apple");
    //    }
    //    #endregion

    //    #region Logout Related
    //    public void LogoutAccount()
    //    {
    //        MenuManager.ins.processingLoad.SetActive(false);
    //        CPlayerPrefs.DeleteAll();
    //        Prefs.IsFirstTime = true;
    //        PlayFabClientAPI.ForgetAllCredentials();
    //        SceneManager.LoadScene("Loading");
    //    }
    //    #endregion

    //    #region Delete Account Related
    //    private void OnDelete()
    //    {
    //        List<LoginIdentityProvider?> providers = PlayFabManager.ins.PlayerProfile.PlayerProfile.LinkedAccounts.Select(account => account.Platform).ToList();

    //#if UNITY_ANDROID
    //        LoginIdentityProvider provider = LoginIdentityProvider.AndroidDevice;
    //#elif UNITY_IOS
    //        LoginIdentityProvider provider = LoginIdentityProvider.IOSDevice;
    //#endif
    //        PlayFabManager.ins.UnlinkAccount(provider, OnSuccessUnlink, OnFailedUnlink);

    //    }

    //    private void OnSuccessUnlink(string provider)
    //    {
    //        DeleteAccount();
    //    }

    //    private void OnFailedUnlink(PlayFabError error)
    //    {
    //        Debug.Log(error.ErrorMessage);
    //    }

    //    public void DeleteAccount()
    //    {
    //        // Call the CloudScript function for account deletion
    //        var request = new ExecuteCloudScriptRequest
    //        {
    //            FunctionName = "DeleteAccount",
    //            // Any additional parameters your CloudScript function expects
    //            FunctionParameter = new { },
    //        };

    //        MenuManager.ins.processingLoad.SetActive(true);

    //        PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptSuccess, OnCloudScriptFailure);
    //    }
    //    void OnCloudScriptSuccess(ExecuteCloudScriptResult result)
    //    {
    //        // Handle success
    //        LogoutAccount();
    //        Debug.Log("$$$ Account deletion successful");
    //    }

    //    void OnCloudScriptFailure(PlayFabError error)
    //    {
    //        // Handle failure
    //        MenuManager.ins.processingLoad.SetActive(false);
    //        Debug.LogError("$$$ Account deletion failed: " + error.ErrorMessage);
    //    }
    //#endregion

    //    #region Error Related
    //    private void OnError(PlayFabError error)
    //    {
    //        Debug.Log("%%%%% Playfab error");
    //        Debug.Log(error.HttpCode);
    //        Debug.Log(JsonConvert.SerializeObject(error.ErrorDetails));
    //        Debug.Log(error.ErrorMessage);
    //        Debug.Log(error.Error.ToString());
    //        Debug.Log(error.GenerateErrorReport());
    //        _errorCallback?.Invoke(error);
    //    }
    //    #endregion

    //    public void updatePlayerHighScore(int newScore)
    //    {
    //        PlayFabClientAPI.UpdateUserData(new PlayFab.ClientModels.UpdateUserDataRequest()
    //        {
    //            Data = new Dictionary<string, string>
    //            {
    //                {"HighScore", newScore.ToString()}
    //            }
    //        }, result =>
    //        {            
    //            Debug.Log("highscore updated");
    //        }, error =>
    //        {

    //        });
    //    }

    //    public void addCurrency(int newCoins)
    //    {   
    //        PlayFabClientAPI.AddUserVirtualCurrency(new PlayFab.ClientModels.AddUserVirtualCurrencyRequest()
    //        {
    //            VirtualCurrency = "GC",
    //            Amount = newCoins
    //        }, result =>
    //        {            
    //            Debug.Log("currency updated");
    //        }, error =>
    //        {
    //            Debug.Log(error.ErrorMessage);

    //        });
    //    }

    //    public void deductCurrency(int newCoins)
    //    {
    //        PlayFabClientAPI.SubtractUserVirtualCurrency(new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest()
    //        {
    //            VirtualCurrency = "GC",
    //            Amount = newCoins
    //        }, result =>
    //        {
    //            Debug.Log("currency updated");
    //        }, error =>
    //        {
    //            Debug.Log(error.ErrorMessage);

    //        });
    //    }

    //    public void GrantItem(LionSkins lionSkins)
    //    {
    //        var items = new List<string> { lionSkins.itemID};
    //        PlayFabServerAPI.GrantItemsToUser(new PlayFab.ServerModels.GrantItemsToUserRequest()
    //        {
    //            ItemIds = items
    //        }, result =>
    //        {
    //            Debug.Log($"skin {lionSkins.itemID} granted");
    //        }, error =>
    //        {
    //            Debug.Log(error.ErrorMessage);
    //        });
    //    }

    //    public void BuyItem(int itemPrice, string itemID)
    //    {
    //        PlayFabClientAPI.PurchaseItem(new PlayFab.ClientModels.PurchaseItemRequest()
    //        {
    //            VirtualCurrency = "GC",
    //            Price = itemPrice,
    //            ItemId = itemID.ToString()
    //        }, result =>
    //        {
    //            Debug.Log($"skin {itemID} purchased");

    //        }, error =>
    //        {
    //            Debug.Log(error.ErrorMessage);
    //        });
    //    }





    //    public void MakeSingleton()
    //    {
    //        if (ins == null)
    //        {
    //            ins = this;
    //            DontDestroyOnLoad(this.gameObject);
    //        }
    //        else
    //        {
    //            Destroy(this.gameObject);
    //        }
    //    }
}
