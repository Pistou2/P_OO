@echo off 
REM Auteur      : GGZ
REM Date        : 9.01.2016
REM But         : Script DOS pour démarrer la console mysql en la démarrant avec l'utilisateur root, 
REM               sans mot de passe, dans un environnement WAMP réalisé avec le progiciel EasyPHP
REM Remarques   : Le chemin est adapté selon le "user" connecté, l'environnement, et le chemin où Easyphp
REM               a été installé
REM
REM               Le progiciel doit avoir été démarré auparavant ...
REM
REM Paramètre   : le paramètre %1 correspond au nom du fichier sql à faire interpréter
REM
REM Utilisation : C:> lanceur 
REM				  mysql> source <fichier>.sql
REM
REM Placer ici le chemin correspondant au répertoire où easyphp a été installé ...
"F:\AUTRES\Programmes\EasyPHP-Devserver-16.1\eds-binaries\dbserver\mysql5711x86x160823145809\bin\mysql.exe" --user=root --port=3306
REM 
pause