using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MyEditorScript
{
	public static void PerformBuild ()
	{
        string[] scenes = { "Assets/_Scenes/Lobby (offline).unity" };
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "Build/windows/Windows.exe";
		buildPlayerOptions.target = BuildTarget.StandaloneWindows;
		buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}

	public static void PerformOSXBuild ()
	{
        string[] scenes = { "Assets/_Scenes/Lobby (offline).unity" };
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "Build/osx/OSX.app";
		buildPlayerOptions.target = BuildTarget.StandaloneOSX;
		buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}
}