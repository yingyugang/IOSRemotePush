using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace BlueNoah
{
    public class Build : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(BuildReport report)
        {
			var path = report.summary.outputPath;
			var rootDir = Directory.GetParent(Application.dataPath).ToString();
			var projPath = Path.Combine(path, "Unity-iPhone.xcodeproj/project.pbxproj");
			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));
			// Notification Service ExtensionをXcodeプロジェクトに追加する処理
			// iOSフォルダに必要なものを追加
			if (!Directory.Exists(path + "/Notification"))
			{
				Directory.CreateDirectory(path + "/Notification");
			}
			File.Copy(rootDir + "/Notification/NotificationService.h", path + "/Notification/NotificationService.h");
			File.Copy(rootDir + "/Notification/NotificationService.m", path + "/Notification/NotificationService.m");
			File.Copy(rootDir + "/Notification/Info.plist", path + "/Notification/Info.plist");
			// XcodeプロジェクトNotification Service Extensionのtargetを追加する
			string targetGUID = proj.GetUnityMainTargetGuid();
			var pathToNotificationService = path + "/Notification";
			var notificationServicePlistPath = "Notification/Info.plist";
			PlistDocument notificationServicePlist = new PlistDocument();
			notificationServicePlist.ReadFromFile(notificationServicePlistPath);
			notificationServicePlist.root.SetString("CFBundleShortVersionString", PlayerSettings.bundleVersion);
			notificationServicePlist.root.SetString("CFBundleVersion", PlayerSettings.iOS.buildNumber.ToString());
			var notificationServiceTarget = PBXProjectExtensions.AddAppExtension(proj, targetGUID, "Notification", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS) + ".Notification", notificationServicePlistPath);
			// Notification Service Extensionのtargetで使用するファイルをXcodeプロジェクトに追加する
			proj.AddFile(pathToNotificationService + "/Info.plist", "NotificationService/Info.plist");
			proj.AddFile(pathToNotificationService + "/NotificationService.h", "NotificationService/NotificationService.h");
			proj.AddFileToBuild(notificationServiceTarget, proj.AddFile(pathToNotificationService + "/NotificationService.m", "NotificationService/NotificationService.m"));
			// Notification Service Extensionに必要な設定を追加する
			proj.SetBuildProperty(notificationServiceTarget, "ARCHS", "$(ARCHS_STANDARD)");
			proj.SetBuildProperty(notificationServiceTarget, "DEVELOPMENT_TEAM", PlayerSettings.iOS.appleDeveloperTeamID);
			proj.SetBuildProperty(notificationServiceTarget, "TARGETED_DEVICE_FAMILY", "1,2");
			proj.SetBuildProperty(notificationServiceTarget, "GCC_C_LANGUAGE_STANDARD", "gnu11");
			proj.SetBuildProperty(notificationServiceTarget, "CLANG_CXX_LANGUAGE_STANDARD", "gnu++14");
			proj.SetBuildProperty(notificationServiceTarget, "CLANG_CXX_LIBRARY", "libc++");
			proj.SetBuildProperty(notificationServiceTarget, "CLANG_ENABLE_MODULES", "YES");
			proj.SetBuildProperty(notificationServiceTarget, "ALWAYS_SEARCH_USER_PATHS", "NO");
			// Xcodeプロジェクトに書き込む
			notificationServicePlist.WriteToFile(notificationServicePlistPath);
			proj.WriteToFile(projPath);
			Debug.Log("Done!");
		}
    }
}

