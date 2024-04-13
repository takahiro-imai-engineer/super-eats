public class GameConstant
{
  public const string APP_ID = "";
  public const string AUTH_SIGNATURE_METHOD = "";
  public const string AUTH_VERSION = "";
  public const string AUTH_SECRET_KEY = "";
#if UNITY_ANDROID
  public const string DEV_TENJIN_API_KEY = "XXXXXXXXXXXXXX";
  public const string RELEASE_TENJIN_API_KEY = "XXXXXXXXXXXXXX";
#elif UNITY_IOS
  public const string DEV_TENJIN_API_KEY = "XXXXXXXXXXXXXX";
  public const string RELEASE_TENJIN_API_KEY = "XXXXXXXXXXXXXX";
#endif
  public const string PRIVACY_POLICY_URL = "https://www.facebook.com/permalink.php?story_fbid=112043181745382&id=100088191024390";

  /// <summary>
  /// シーン名.
  /// </summary>
  public enum Scene
  {
    Main,
    Title,
    InGame,
    WarpNextStage
  }
}