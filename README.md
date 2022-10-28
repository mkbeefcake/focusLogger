# focusLogger
Detects focused windows app

# build instruction
1. install Microsoft Visual Studio 2019
2. Open the project and Build it

# launch parameters
Usage :
focusLogger.exe --logfilePath %APPDATA%\focuslogger --logfileName focuslogger --rotateInterval 60

-p, --logfilePath : LogFile's path
-f, --logfileName : LogFile's name
-l, --rotateInterval : log rotation interval in minutes

at least have to indicate -l, --rotateInterval option to launch the app correctly
like as
focusLogger.exe --rotateInterval 1


