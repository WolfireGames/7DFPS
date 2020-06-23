del /f /Q /S Receiver_linux32.zip
del /f /Q /S "Build\linux32"

"%UNITY_2019_4_1%" -quit -batchmode -projectPath "%~dp0" -executeMethod ReceiverBuildScript.BuildLinux32 -logFile unitylog.txt

if errorlevel 1 (
    echo Failure To Run
    exit /b %errorlevel%
)

cd Build/linux32
7z a -tzip ../../Receiver_linux32.zip ./
cd ../..
