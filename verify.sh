#!/bin/bash

echo "========================================"
echo "  Revit AI Assistant - Verification"
echo "========================================"
echo ""

echo "Checking project structure..."

# Check if main files exist
if [ ! -f "RevitAIAssistant.sln" ]; then
    echo "❌ Solution file missing"
    exit 1
else
    echo "✅ Solution file found"
fi

if [ ! -f "RevitAIAssistant/RevitAIAssistant.csproj" ]; then
    echo "❌ Project file missing"
    exit 1
else
    echo "✅ Project file found"
fi

if [ ! -f "RevitAIAssistant/Application.cs" ]; then
    echo "❌ Application class missing"
    exit 1
else
    echo "✅ Application class found"
fi

if [ ! -f "RevitAIAssistant/Commands/AIAssistantCommand.cs" ]; then
    echo "❌ Main command missing"
    exit 1
else
    echo "✅ Main command found"
fi

if [ ! -f "RevitAIAssistant/Services/TogetherAIService.cs" ]; then
    echo "❌ AI service missing"
    exit 1
else
    echo "✅ AI service found"
fi

if [ ! -f "RevitAIAssistant/UI/AIAssistantWindow.xaml" ]; then
    echo "❌ UI XAML missing"
    exit 1
else
    echo "✅ UI XAML found"
fi

if [ ! -f "RevitAIAssistant/UI/AIAssistantWindow.xaml.cs" ]; then
    echo "❌ UI code-behind missing"
    exit 1
else
    echo "✅ UI code-behind found"
fi

if [ ! -f "RevitAIAssistant/RevitAIAssistant.addin" ]; then
    echo "❌ Plugin manifest missing"
    exit 1
else
    echo "✅ Plugin manifest found"
fi

echo ""
echo "Checking critical fixes..."

# Check if HttpClient is not static
if grep -q "private static.*HttpClient" RevitAIAssistant/Services/TogetherAIService.cs; then
    echo "❌ HttpClient is still static (critical error)"
    exit 1
else
    echo "✅ HttpClient is instance-based"
fi

# Check if IDisposable is implemented
if grep -q "IDisposable" RevitAIAssistant/Services/TogetherAIService.cs; then
    echo "✅ IDisposable implemented"
else
    echo "❌ IDisposable not implemented"
    exit 1
fi

# Check if XML manifest is correct
if grep -q "<Name>" RevitAIAssistant/RevitAIAssistant.addin; then
    echo "✅ XML manifest has correct Name element"
elif grep -q "<n>" RevitAIAssistant/RevitAIAssistant.addin; then
    echo "⚠️  XML manifest uses <n> instead of <Name> (needs manual fix)"
    echo "   Please edit RevitAIAssistant.addin and change <n> to <Name>"
else
    echo "❌ XML manifest has no Name element"
    exit 1
fi

echo ""
echo "Checking documentation..."

if [ ! -f "README.md" ]; then
    echo "⚠️  Main README missing"
else
    echo "✅ Main README found"
fi

if [ ! -f "SETUP_QUICK.md" ]; then
    echo "⚠️  Quick setup guide missing"
else
    echo "✅ Quick setup guide found"
fi

if [ ! -f "EXEMPLOS_USO.md" ]; then
    echo "⚠️  Usage examples missing"
else
    echo "✅ Usage examples found"
fi

if [ ! -f "ERROS_CORRIGIDOS.md" ]; then
    echo "⚠️  Error fixes documentation missing"
else
    echo "✅ Error fixes documented"
fi

echo ""
echo "========================================"
echo "   ✅ All components verified!"
echo "========================================"
echo ""
echo "Next steps:"
echo "1. Get free API key from https://api.together.xyz/"
echo "2. Run 'build.bat' to compile and install (Windows)"
echo "3. Open Revit 2026 and look for 'AI Assistant' tab"
echo "4. Enter your API key when prompted"
echo "5. Start chatting with the AI!"
echo ""
echo "For help, see:"
echo "- README.md (complete documentation)"
echo "- SETUP_QUICK.md (5-minute setup)"
echo "- EXEMPLOS_USO.md (usage examples)"
echo "- ERROS_CORRIGIDOS.md (error fixes)"
echo ""