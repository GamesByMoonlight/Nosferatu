#! /bin/sh
echo 'Attempting to zip builds'
zip -r $(pwd)/Build/${project_name}-Windows.zip $(pwd)/Build/windows/
zip -r $(pwd)/Build/${project_name}-OSX.zip $(pwd)/Build/osx/
