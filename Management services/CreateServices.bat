@echo [%time%:Start] create services
for %%i in ("%~dp0\..") do set "root_parent=%%~fi"
sc create TelegramNotification binPath=%root_parent%\src\Notifications\TelegramNotification\bin\Release\netcoreapp3.1\TelegramNotification.exe
sc create DistributorService binPath=%root_parent%\src\DistributorService\bin\Release\netcoreapp3.1\DistributorService.exe
sc create ParserHtml binPath=%root_parent%\src\Parsers\ParserHtml\bin\Release\netcoreapp3.1\ParserHtml.exe
sc create ReaderHtml binPath=%root_parent%\src\Readers\ReaderHtml\bin\Release\netcoreapp3.1\ReaderHtml.exe
sc create ManagerService binPath=%root_parent%\src\ManagerService\bin\Release\netcoreapp3.1\ManagerService.exe
@echo [%time%:End] create services
pause