rem call: RipTVShows.bat inputdriveletter outputfolder discname
rem eg: C:\Users\Me\Desktop\RipTVShows.bat {Drive} C:\rips\{Timestamp} {Name}

rem Setup:
rem Start Windows Powershell as Administrator
rem enter: set-executionpolicy unrestricted
rem close the powershell
rem Update the path name below

mkdir %2
powershell.exe C:\Users\Andy\Desktop\RipTVShows.ps1 %1 %2 %3
