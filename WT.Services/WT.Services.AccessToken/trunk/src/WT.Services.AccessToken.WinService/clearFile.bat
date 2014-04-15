@ECHO OFF
SETLOCAL
::删除目录中多余文件

FOR %%f IN (*.pdb) DO (
IF EXIST %%f (
del /f/s/q %%f
ECHO 目录%%f已被移除
)
)

FOR %%f IN (*.vshost.*) DO (
IF EXIST %%f (
del /f/s/q %%f
ECHO 目录%%f已被移除
)
)

FOR %%f IN (*.xml) DO (
IF EXIST %%f (
del /f/s/q %%f
ECHO 目录%%f已被移除
)
)

del /f/s/q clearFile.bat

:END
ENDLOCAL
ECHO ON