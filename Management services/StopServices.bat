@echo [%time%:Start] stop services
sc stop ManagerService
sc stop ReaderHtml
sc stop ParserHtml
sc stop DistributorService
sc stop TelegramNotification
@echo [%time%:End] stop services
pause