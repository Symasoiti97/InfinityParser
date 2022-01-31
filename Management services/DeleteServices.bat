@echo [%time%:Start] delete services
sc delete ManagerService
sc delete ReaderHtml
sc delete ParserHtml
sc delete DistributorService
sc delete TelegramNotification
@echo [%time%:End] delete services
pause