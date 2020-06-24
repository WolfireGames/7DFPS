del /f /Q /S Receiver_win64.zip
del /f /Q /S "Build\win64"

git rev-parse --verify HEAD > ref.txt

"%UNITY_2019_4_1%" -quit -batchmode -projectPath "%~dp0" -executeMethod ReceiverBuildScript.BuildWindows64 -logFile unitylog.txt

if errorlevel 1 (
    echo Failure To Run
    exit /b %errorlevel%
)

IF EXIST Build\win64 (
    cd Build/win64
    7z a -tzip ../../Receiver_win64.zip ./
    cd ../..
) ELSE ( 
    echo No Build Folder Generated
    exit /b 1
)
