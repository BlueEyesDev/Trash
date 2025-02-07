@echo off
set "FICHIER=%~dp0Avertissement.txt"

echo Attention ne téléchargez et n'ouvrez pas tout et n'importe quoi, ceci aurait pu être un virus. Vérifiez toujours sur https://virustotal.com, même si cela peut venir de membres du staff d'un Discord ou de tout autre utilisateur, car nous ne sommes jamais à l'abri d'une fuite de données qui donnerait accès au Discord de personnes que vous connaissez ou en qui vous avez confiance. Ces personnes peuvent avoir été piratées. > "%FICHIER%"

start "" "%FICHIER%"
exit