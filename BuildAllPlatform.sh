#!/bin/bash

CUR_DIR=$(pwd)

echo "Building for platform: Editor"
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /p:DefineConstants="TRACE DEBUG UNITY_EDITOR"
mkdir -p ${CUR_DIR}/Output/vFrame.Bundler/Runtime
cp -f ${CUR_DIR}/Build/vFrame.Bundler/Debug/vFrame.Bundler.* ${CUR_DIR}/Output/vFrame.Bundler/Runtime/
mkdir -p ${CUR_DIR}/Output/vFrame.Bundler/Editor
cp -f ${CUR_DIR}/Build/vFrame.Bundler.Editor/Debug/vFrame.Bundler.Editor.* ${CUR_DIR}/Output/vFrame.Bundler/Editor/

echo "Building for platform: Standalone"
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_STANDALONE"
mkdir -p ${CUR_DIR}/Output/vFrame.Bundler/Runtime/Standalone
cp -f ${CUR_DIR}/Build/vFrame.Bundler/Release/vFrame.Bundler.* ${CUR_DIR}/Output/vFrame.Bundler/Runtime/Standalone/

echo "Building for platform: Android"
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_ANDROID"
mkdir -p ${CUR_DIR}/Output/vFrame.Bundler/Runtime/Android
cp -f ${CUR_DIR}/Build/vFrame.Bundler/Release/vFrame.Bundler.* ${CUR_DIR}/Output/vFrame.Bundler/Runtime/Android/

echo "Building for platform: iOS"
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_IOS"
mkdir -p ${CUR_DIR}/Output/vFrame.Bundler/Runtime/iOS
cp -f ${CUR_DIR}/Build/vFrame.Bundler/Release/vFrame.Bundler.* ${CUR_DIR}/Output/vFrame.Bundler/Runtime/iOS/

echo "Build finished!"
echo

read -p "Press any key to continue.."
