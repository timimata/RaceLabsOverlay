@echo off
chcp 65001 > nul
echo.
echo ═══════════════════════════════════════════════════════════════
echo   🏎️  RaceLabs Overlay - Build Script
echo ═══════════════════════════════════════════════════════════════
echo.

:: Verificar se .NET SDK está instalado
where dotnet > nul 2>&1
if errorlevel 1 (
    echo ❌ ERRO: .NET SDK não encontrado!
    echo.
    echo Por favor instala o .NET 8.0 SDK:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

echo ✅ .NET SDK encontrado

:: Verificar versão
dotnet --version

echo.
echo 🔨 A compilar...
echo.

:: Restore packages
echo 📦 Restoring packages...
dotnet restore RaceLabsOverlay.csproj
if errorlevel 1 (
    echo ❌ Erro ao restaurar packages
    pause
    exit /b 1
)

:: Build
echo 🔧 Building...
dotnet build RaceLabsOverlay.csproj -c Release --no-restore
if errorlevel 1 (
    echo ❌ Erro no build
    pause
    exit /b 1
)

echo.
echo 📦 A criar executável standalone...
echo.

:: Publish - Single File + Self-Contained
:: Isto cria um .exe único que inclui tudo!
dotnet publish RaceLabsOverlay.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishTrimmed=false `
    -o "publish"

if errorlevel 1 (
    echo ❌ Erro no publish
    pause
    exit /b 1
)

echo.
echo ═══════════════════════════════════════════════════════════════
echo   ✅ BUILD CONCLUÍDO COM SUCESSO!
echo ═══════════════════════════════════════════════════════════════
echo.
echo 📁 Ficheiros criados em: publish\
echo.
echo 🎯 Executável principal:
echo    publish\RaceLabsOverlay.exe
echo.
echo 📋 Instruções:
echo    1. Copia a pasta 'publish' para onde quiseres
echo    2. Corre RaceLabsOverlay.exe
echo    3. Abre o iRacing e diverte-te!
echo.
echo 🚀 Para distribuir:
echo    - Comprime a pasta 'publish' num .zip
echo    - Upload para GitHub Releases
echo    - A malta faz download e corre diretamente!
echo.
echo    NOTA: Não precisa de instalar .NET Runtime!
echo          O .exe inclui tudo o que precisa.
echo.
pause
