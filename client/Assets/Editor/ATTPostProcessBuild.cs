#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class ATTPostProcessBuild
{
    private const string ATT_FRAMEWORK = "AppTrackingTransparency.framework";
    private const string ATT_TRACKING_DESCRIPTION = "Your data will be used to deliver ads tailored for you.";
    [PostProcessBuild]
    private static void OnPostProcessBuild
    (
        BuildTarget buildTarget,
        string      pathToBuiltProject
    )
    {
        if ( buildTarget != BuildTarget.iOS ) return;

        var path          = pathToBuiltProject + "/Info.plist";
        var plistDocument = new PlistDocument();

        plistDocument.ReadFromFile( path );
        plistDocument.root.SetString
        (
            key: "NSUserTrackingUsageDescription",
            val: ATT_TRACKING_DESCRIPTION
        );
        plistDocument.WriteToFile( path );
    }
}

#endif