#!/bin/bash
# ═══════════════════════════════════════════════════════════════
#  Blue Notes — Build APK Script
#  Uso: ./build-apk.sh [debug|release]
# ═══════════════════════════════════════════════════════════════

set -e

MODE="${1:-release}"
PROJECT="BlueNotes/BlueNotes.csproj"
VERSION="0.0.0.0.1"
OUT="./dist"

echo ""
echo "╔══════════════════════════════════════╗"
echo "║  🔵  Blue Notes — Build APK           ║"
echo "║  Versão: $VERSION                     ║"
echo "║  Modo:   $MODE                        ║"
echo "╚══════════════════════════════════════╝"
echo ""

# ── Pré-requisitos ───────────────────────────────────────────────
command -v dotnet >/dev/null 2>&1 || { echo "❌ .NET SDK não encontrado. Instale em https://dotnet.microsoft.com"; exit 1; }

echo "✅  .NET SDK: $(dotnet --version)"

# ── Instalar workload se necessário ─────────────────────────────
echo "📲  Verificando workload MAUI Android..."
dotnet workload install maui-android --skip-sign-check 2>/dev/null || true

# ── Restaurar pacotes ────────────────────────────────────────────
echo "📦  Restaurando pacotes NuGet..."
dotnet restore "$PROJECT"

# ── Build ────────────────────────────────────────────────────────
mkdir -p "$OUT"

if [ "$MODE" = "debug" ]; then
    echo "🔨  Compilando APK Debug..."
    dotnet build "$PROJECT" \
        -f net8.0-android \
        -c Debug \
        -p:AndroidPackageFormat=apk

    APK=$(find . -path "*/Debug/net8.0-android*" -name "*.apk" | head -1)
else
    echo "🔨  Compilando APK Release..."
    dotnet publish "$PROJECT" \
        -f net8.0-android \
        -c Release \
        -p:AndroidPackageFormat=apk \
        -p:AndroidKeyStore=false \
        -o "$OUT/publish"

    APK=$(find "$OUT/publish" -name "*.apk" | head -1)
fi

# ── Copiar APK final ─────────────────────────────────────────────
if [ -n "$APK" ] && [ -f "$APK" ]; then
    FINAL="$OUT/BlueNotes-v${VERSION}-${MODE}.apk"
    cp "$APK" "$FINAL"
    echo ""
    echo "╔══════════════════════════════════════════════════════╗"
    echo "║  ✅  APK gerado com sucesso!                          ║"
    echo "║  📁  $FINAL"
    echo "╚══════════════════════════════════════════════════════╝"
    echo ""
    ls -lh "$FINAL"
else
    echo "❌  APK não encontrado. Verifique os erros acima."
    exit 1
fi

# ── Instruções de instalação ─────────────────────────────────────
echo ""
echo "📱  Para instalar via ADB:"
echo "    adb install -r \"$FINAL\""
echo ""
echo "📱  Para instalar manualmente:"
echo "    Copie o APK para o celular e abra o arquivo"
echo "    (habilite 'Fontes desconhecidas' nas configurações)"
echo ""
