#!/bin/bash

CUR_DIR=$(pwd)

echo "Building for platform: Editor"
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /p:DefineConstants="TRACE DEBUG UNITY_EDITOR"
mkdir -p ${CUR_DIR}/Output/vBundler/Runtime
cp -f ${CUR_DIR}/Build/vBundler/Release/vBundler.* ${CUR_DIR}/Output/vBundler/Runtime/
mkdir -p ${CUR_DIR}/Output/vBundler/Editor
cp -f ${CUR_DIR}/Build/vBundler.Editor/Release/vBundler.Editor.* ${CUR_DIR}/Output/vBundler/Editor/

echo "Building for platform: Standalone"
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_STANDALONE"
mkdir -p ${CUR_DIR}/Output/vBundler/Runtime/Standalone
cp -f ${CUR_DIR}/Build/vBundler/Release/vBundler.* ${CUR_DIR}/Output/vBundler/Runtime/Standalone/

echo "Building for platform: Android"
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_ANDROID"
mkdir -p ${CUR_DIR}/Output/vBundler/Runtime/Android
cp -f ${CUR_DIR}/Build/vBundler/Release/vBundler.* ${CUR_DIR}/Output/vBundler/Runtime/Android/

echo "Building for platform: iOS"
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_IOS"
mkdir -p ${CUR_DIR}/Output/vBundler/Runtime/iOS
cp -f ${CUR_DIR}/Build/vBundler/Release/vBundler.* ${CUR_DIR}/Output/vBundler/Runtime/iOS/

echo "Build finished!"
echo

read -p "Press any key to continue.."
