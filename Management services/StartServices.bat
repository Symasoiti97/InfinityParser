@echo [%time%:Start] start services
sc start TelegramNotification
timeout 15
sc start DistributorService
timeout 15
sc start ReaderHtml
timeout 15
sc start ParserHtml
timeout 15
sc start ManagerService
@echo [%time%:End] start services
pause