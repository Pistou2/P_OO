@echo off 
REM Auteur      : GGZ
REM Date        : 9.01.2016
REM But         : Script DOS pour d�marrer la console mysql en la d�marrant avec l'utilisateur root, 
REM               sans mot de passe, dans un environnement WAMP r�alis� avec le progiciel EasyPHP
REM Remarques   : Le chemin est adapt� selon le "user" connect�, l'environnement, et le chemin o� Easyphp
REM               a �t� install�
REM
REM               Le progiciel doit avoir �t� d�marr� auparavant ...
REM
REM Param�tre   : le param�tre %1 correspond au nom du fichier sql � faire interpr�ter
REM
REM Utilisation : C:> lanceur 
REM				  mysql> source <fichier>.sql
REM
REM Placer ici le chemin correspondant au r�pertoire o� easyphp a �t� install� ...
"F:\AUTRES\Programmes\EasyPHP-Devserver-16.1\eds-binaries\dbserver\mysql5711x86x160823145809\bin\mysql.exe" --user=root --port=3306
REM 
pause