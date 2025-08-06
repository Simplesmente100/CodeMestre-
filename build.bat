@echo off
echo ========================================
echo   Revit AI Assistant - Build Script
echo ========================================
echo.

REM Check if MSBuild is available
where msbuild >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: MSBuild not found. Please install Visual Studio or Build Tools.
    pause
    exit /b 1
)

REM Build the solution
echo Building Revit AI Assistant...
msbuild RevitAIAssistant.sln /p:Configuration=Release /p:Platform=x64 /v:minimal

if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo.

REM Ask user if they want to install
set /p install="Do you want to install the plugin to Revit 2026? (y/n): "
if /i "%install%" NEQ "y" goto end

REM Create installation directory
set INSTALL_DIR=%APPDATA%\Autodesk\Revit\Addins\2026
if not exist "%INSTALL_DIR%" (
    echo Creating installation directory...
    mkdir "%INSTALL_DIR%"
)

REM Copy files
echo Installing plugin files...
copy "RevitAIAssistant\bin\Release\RevitAIAssistant.dll" "%INSTALL_DIR%\"
copy "RevitAIAssistant\bin\Release\RevitAIAssistant.dll.config" "%INSTALL_DIR%\"
copy "RevitAIAssistant\bin\Release\Newtonsoft.Json.dll" "%INSTALL_DIR%\"
copy "RevitAIAssistant\RevitAIAssistant.addin" "%INSTALL_DIR%\"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   Installation completed successfully!
    echo ========================================
    echo.
    echo The AI Assistant plugin has been installed to:
    echo %INSTALL_DIR%
    echo.
    echo To use the plugin:
    echo 1. Start Revit 2026
    echo 2. Look for the "AI Assistant" tab in the ribbon
    echo 3. Click "AI Assistant" button
    echo 4. Enter your Together.ai API key when prompted
    echo.
    echo Get your free API key at: https://api.together.xyz/
    echo.
) else (
    echo ERROR: Installation failed!
)

:end
pause