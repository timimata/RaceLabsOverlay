#!/bin/bash

echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  🏎️  RaceLabs Overlay - Build Script (Linux/macOS)"
echo "═══════════════════════════════════════════════════════════════"
echo ""

# Verificar se .NET SDK está instalado
if ! command -v dotnet &> /dev/null
then
    echo "❌ ERRO: .NET SDK não encontrado!"
    echo ""
    echo "Por favor instala o .NET 8.0 SDK:"
    echo "https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
    exit 1
fi

echo "✅ .NET SDK encontrado"
dotnet --version

echo ""
echo "🔨 A compilar..."
echo ""

# Restore packages
echo "📦 Restoring packages..."
dotnet restore RaceLabsOverlay.csproj
if [ $? -ne 0 ]; then
    echo "❌ Erro ao restaurar packages"
    exit 1
fi

# Build
echo "🔧 Building..."
dotnet build RaceLabsOverlay.csproj -c Release --no-restore
if [ $? -ne 0 ]; then
    echo "❌ Erro no build"
    exit 1
fi

echo ""
echo "📦 A criar executável standalone..."
echo ""

# Publish - Single File + Self-Contained
dotnet publish RaceLabsOverlay.csproj \
    -c Release \
    -r win-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:PublishTrimmed=false \
    -o "publish"

if [ $? -ne 0 ]; then
    echo "❌ Erro no publish"
    exit 1
fi

echo ""
echo "═══════════════════════════════════════════════════════════════"
echo "  ✅ BUILD CONCLUÍDO COM SUCESSO!"
echo "═══════════════════════════════════════════════════════════════"
echo ""
echo "📁 Ficheiros criados em: publish/"
echo ""
echo "🎯 Executável principal:"
echo "   publish/RaceLabsOverlay.exe"
echo ""
echo "📋 Instruções:"
echo "   1. Copia a pasta 'publish' para onde quiseres"
echo "   2. Corre RaceLabsOverlay.exe"
echo "   3. Abre o iRacing e diverte-te!"
echo ""
echo "🚀 Para distribuir:"
echo "   - Comprime a pasta 'publish' num .zip"
echo "   - Upload para GitHub Releases"
echo "   - A malta faz download e corre diretamente!"
echo ""
echo "   NOTA: Não precisa de instalar .NET Runtime!"
echo "         O .exe inclui tudo o que precisa."
echo ""
