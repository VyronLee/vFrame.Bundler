@echo off

SET CURDIR=%~dp0

echo Building for platform: Editor
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /p:DefineConstants="TRACE DEBUG UNITY_EDITOR"
if not exist "%CURDIR%\Output\vFrame.Bundler\Runtime\Debug" mkdir %CURDIR%\Output\vFrame.Bundler\Runtime\Debug
copy /Y %CURDIR%\Build\vFrame.Bundler\Debug\vFrame.Bundler.* %CURDIR%\Output\vFrame.Bundler\Runtime\Debug
if not exist "%CURDIR%\Output\vFrame.Bundler\Editor" mkdir %CURDIR%\Output\vFrame.Bundler\Editor
copy /Y %CURDIR%\Build\vFrame.Bundler.Editor\Debug\vFrame.Bundler.Editor.* %CURDIR%\Output\vFrame.Bundler\Editor\

echo Building for platform: Runtime
msbuild vFrame.Bundler.sln /t:Clean,Rebuild /p:Configuration=Release /p:Platform="Any CPU" /p:DefineConstants="TRACE"
if not exist "%CURDIR%\Output\vFrame.Bundler\Runtime\Release" mkdir %CURDIR%\Output\vFrame.Bundler\Runtime\Release
copy /Y %CURDIR%\Build\vFrame.Bundler\Release\vFrame.Bundler.* %CURDIR%\Output\vFrame.Bundler\Runtime\Release

Pause
