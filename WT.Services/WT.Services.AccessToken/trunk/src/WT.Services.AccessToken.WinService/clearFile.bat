@ECHO OFF
SETLOCAL
::ɾ��Ŀ¼�ж����ļ�

FOR %%f IN (*.pdb) DO (
IF EXIST %%f (
del /f/s/q %%f
ECHO Ŀ¼%%f�ѱ��Ƴ�
)
)

FOR %%f IN (*.vshost.*) DO (
IF EXIST %%f (
del /f/s/q %%f
ECHO Ŀ¼%%f�ѱ��Ƴ�
)
)

FOR %%f IN (*.xml) DO (
IF EXIST %%f (
del /f/s/q %%f
ECHO Ŀ¼%%f�ѱ��Ƴ�
)
)

del /f/s/q clearFile.bat

:END
ENDLOCAL
ECHO ON