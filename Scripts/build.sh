#! /bin/sh

# ls -LR /Applications/Unity/
echo "Attempting to build for Windows"
/Applications/Unity//Unity.app/Contents/MacOS/Unity \
  -silent-crashes \
  -batchmode \
  -nographics \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd)/../ \
  -executeMethod MyEditorScript.PerformBuild  \
  -quit

  echo "Attempting to build for OSX"
/Applications/Unity//Unity.app/Contents/MacOS/Unity \
  -silent-crashes \
  -batchmode \
  -nographics \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd)/../ \
  -executeMethod MyEditorScript.PerformOSXBuild  \
  -quit

# echo 'Build Log'
 cat $(pwd)/unity.log
# echo 'End Build Log'


