#! /bin/sh

BASE_URL=http://netstorage.unity3d.com/unity
version=2018.1.0f2

download() {
  file=$1
  url="$BASE_URL/${hash}/$package"

  echo "Downloading from $url: "
  curl -o `basename "$package"` "$url"
}

install() {
  package=$1
  download "$package"

  echo "Installing "`basename "$package"`
  sudo installer -dumplog -package `basename "$package"` -target /
}

install "MacEditorInstaller/Unity-$version.pkg"
install "MacEditorTargetInstaller/UnitySetup-Windows-Mono-Support-for-Editor-$version.pkg"



