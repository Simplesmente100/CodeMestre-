@echo off
echo ========================================
echo   Revit AI Assistant - Verification
echo ========================================
echo.

echo Checking project structure...

REM Check if main files exist
if not exist "RevitAIAssistant.sln" (
    echo ❌ Solution file missing
    goto :error
) else (
    echo ✅ Solution file found
)

if not exist "RevitAIAssistant\RevitAIAssistant.csproj" (
    echo ❌ Project file missing
    goto :error
) else (
    echo ✅ Project file found
)

if not exist "RevitAIAssistant\Application.cs" (
    echo ❌ Application class missing
    goto :error
) else (
    echo ✅ Application class found
)

if not exist "RevitAIAssistant\Commands\AIAssistantCommand.cs" (
    echo ❌ Main command missing
    goto :error
) else (
    echo ✅ Main command found
)

if not exist "RevitAIAssistant\Services\TogetherAIService.cs" (
    echo ❌ AI service missing
    goto :error
) else (
    echo ✅ AI service found
)

if not exist "RevitAIAssistant\UI\AIAssistantWindow.xaml" (
    echo ❌ UI XAML missing
    goto :error
) else (
    echo ✅ UI XAML found
)

if not exist "RevitAIAssistant\UI\AIAssistantWindow.xaml.cs" (
    echo ❌ UI code-behind missing
    goto :error
) else (
    echo ✅ UI code-behind found
)

if not exist "RevitAIAssistant\RevitAIAssistant.addin" (
    echo ❌ Plugin manifest missing
    goto :error
) else (
    echo ✅ Plugin manifest found
)

echo.
echo Checking tools...

REM Check if MSBuild is available
where msbuild >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ MSBuild not found
    echo    Please install Visual Studio or Build Tools
    goto :error
) else (
    echo ✅ MSBuild found
)

echo.
echo Checking documentation...

if not exist "README.md" (
    echo ⚠️  Main README missing
) else (
    echo ✅ Main README found
)

if not exist "SETUP_QUICK.md" (
    echo ⚠️  Quick setup guide missing
) else (
    echo ✅ Quick setup guide found
)

if not exist "EXEMPLOS_USO.md" (
    echo ⚠️  Usage examples missing
) else (
    echo ✅ Usage examples found
)

echo.
echo ========================================
echo   ✅ All components verified!
echo ========================================
echo.
echo Next steps:
echo 1. Get free API key from https://api.together.xyz/
echo 2. Run 'build.bat' to compile and install
echo 3. Open Revit 2026 and look for "AI Assistant" tab
echo 4. Enter your API key when prompted
echo 5. Start chatting with the AI!
echo.
echo For help, see:
echo - README.md (complete documentation)
echo - SETUP_QUICK.md (5-minute setup)
echo - EXEMPLOS_USO.md (usage examples)
echo.
goto :end

:error
echo.
echo ========================================
echo   ❌ Verification failed!
echo ========================================
echo.
echo Some required files are missing.
echo Please check the project structure.
echo.

:end
pause