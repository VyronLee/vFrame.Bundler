@echo off

SET CURDIR=%~dp0

echo Building for platform: Editor
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /p:DefineConstants="TRACE DEBUG UNITY_EDITOR"
if not exist "%CURDIR%\Output\vBundler\Runtime" mkdir %CURDIR%\Output\vBundler\Runtime
copy /Y %CURDIR%\Build\vBundler\Release\vBundler.* %CURDIR%\Output\vBundler\Runtime\
if not exist "%CURDIR%\Output\vBundler\Editor" mkdir %CURDIR%\Output\vBundler\Editor
copy /Y %CURDIR%\Build\vBundler.Editor\Release\vBundler.Editor.* %CURDIR%\Output\vBundler\Editor\

echo Building for platform: Standalone
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_STANDALONE"
if not exist "%CURDIR%\Output\vBundler\Runtime\Standalone" mkdir %CURDIR%\Output\vBundler\Runtime\Standalone
copy /Y %CURDIR%\Build\vBundler\Release\vBundler.* %CURDIR%\Output\vBundler\Runtime\Standalone\

echo Building for platform: Android
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_ANDROID"
if not exist "%CURDIR%\Output\vBundler\Runtime\Android" mkdir %CURDIR%\Output\vBundler\Runtime\Android
copy /Y %CURDIR%\Build\vBundler\Release\vBundler.* %CURDIR%\Output\vBundler\Runtime\Android\

echo Building for platform: iOS
msbuild vBundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_IOS"
if not exist "%CURDIR%\Output\vBundler\Runtime\iOS" mkdir %CURDIR%\Output\vBundler\Runtime\iOS
copy /Y %CURDIR%\Build\vBundler\Release\vBundler.* %CURDIR%\Output\vBundler\Runtime\iOS\

Pause
