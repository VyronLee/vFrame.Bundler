@echo off

SET CURDIR=%~dp0

echo Building for platform: Editor
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /p:DefineConstants="TRACE DEBUG UNITY_EDITOR"
if not exist "%CURDIR%\Output\vFrame.Bundler\Runtime" mkdir %CURDIR%\Output\vFrame.Bundler\Runtime
copy /Y %CURDIR%\Build\vFrame.Bundler\Debug\vFrame.Bundler.* %CURDIR%\Output\vFrame.Bundler\Runtime\
if not exist "%CURDIR%\Output\vFrame.Bundler\Editor" mkdir %CURDIR%\Output\vFrame.Bundler\Editor
copy /Y %CURDIR%\Build\vFrame.Bundler.Editor\Debug\vFrame.Bundler.Editor.* %CURDIR%\Output\vFrame.Bundler\Editor\

echo Building for platform: Standalone
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_STANDALONE"
if not exist "%CURDIR%\Output\vFrame.Bundler\Runtime\Standalone" mkdir %CURDIR%\Output\vFrame.Bundler\Runtime\Standalone
copy /Y %CURDIR%\Build\vFrame.Bundler\Release\vFrame.Bundler.* %CURDIR%\Output\vFrame.Bundler\Runtime\Standalone\

echo Building for platform: Android
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_ANDROID"
if not exist "%CURDIR%\Output\vFrame.Bundler\Runtime\Android" mkdir %CURDIR%\Output\vFrame.Bundler\Runtime\Android
copy /Y %CURDIR%\Build\vFrame.Bundler\Release\vFrame.Bundler.* %CURDIR%\Output\vFrame.Bundler\Runtime\Android\

echo Building for platform: iOS
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE UNITY_IOS"
if not exist "%CURDIR%\Output\vFrame.Bundler\Runtime\iOS" mkdir %CURDIR%\Output\vFrame.Bundler\Runtime\iOS
copy /Y %CURDIR%\Build\vFrame.Bundler\Release\vFrame.Bundler.* %CURDIR%\Output\vFrame.Bundler\Runtime\iOS\

Pause
