using Unity.RemoteConfig;
using UnityEngine;

public class RemoteConfigManager : MonoBehaviour
{

    private struct userAttributes { }

    private struct appAttributes { }

    void Awake()
    {
        // Add a listener to apply settings when successfully retrieved: 
        ConfigManager.FetchCompleted += ApplyRemoteSettings;

        // Set the user’s unique ID:
        //ConfigManager.SetCustomUserID("Test1");

        // Fetch configuration setting from the remote service: 
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    // Start is called before the first frame update
    void Start() { }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {

        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                break;
            case ConfigOrigin.Cached:
                break;
            case ConfigOrigin.Remote:
                // Debug.Log ("New settings loaded this session; update values accordingly.");
                // var0 = ConfigManager.appConfig.GetInt ("var0");
                // var1 = ConfigManager.appConfig.GetBool ("var1");

                Debug.Log("RemoteConfig Fetch Complete");

                break;
        }
    }
}