using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MyEditorScript
{

	 static string[] scenes = { "Assets/Scenes/Lobby (offline).unity", 
		"Assets/Scenes/LobbyNetwork",
		"Assets/Scenes/Online",
		"Assets/Scenes/Maze (online)"  };

	public static void PerformBuild ()
	{
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "Build/windows/Windows.exe";
		buildPlayerOptions.target = BuildTarget.StandaloneWindows;
		buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}

	public static void PerformOSXBuild ()
	{
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = scenes;
		buildPlayerOptions.locationPathName = "Build/osx/OSX.app";
		buildPlayerOptions.target = BuildTarget.StandaloneOSX;
		buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}
}