@echo off
echo ========================================================
echo    COMPILANDO VERSAO PORTATIL DO PADEL SUPER 8 PRO
echo ========================================================
echo.
echo Gerando executavel autonomo (Self-Contained Single-File)...
echo Isso pode levar alguns segundos...
echo.

dotnet publish PadelSuper8\PadelSuper8.csproj -c Release -o PadelSuper8_Portatil

echo.
echo ========================================================
echo   SUCESSO! O EXECUTAVEL FOI GERADO COM EXITO EM:
echo   pasta: PadelSuper8_Portatil\PadelSuper8.exe
echo ========================================================
echo.
echo Voce pode copiar o arquivo "PadelSuper8.exe" para qualquer
echo pen-drive ou outro computador Windows (10 ou 11 64-bit).
echo Ele rodara perfeitamente sem precisar instalar nada!
echo.
pause
