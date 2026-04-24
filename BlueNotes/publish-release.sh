#!/bin/bash
# ═══════════════════════════════════════════════════════════════
#  Blue Notes — Publicar Release no GitHub
#  Uso: ./publish-release.sh v0.0.0.0.1 "Mensagem da release"
# ═══════════════════════════════════════════════════════════════

set -e

TAG="${1:-v0.0.0.0.1}"
MSG="${2:-Blue Notes: initial release}"

echo ""
echo "🚀  Publicando Blue Notes $TAG no GitHub..."
echo ""

# Garantir que está na branch main
git checkout main

# Adicionar todos os arquivos
git add -A

# Commit
git commit -m "chore: release $TAG

$MSG" || echo "Nada para commitar."

# Push
git push origin main

# Criar tag anotada
git tag -a "$TAG" -m "🔵 Blue Notes $TAG

$MSG"

# Push da tag (dispara o CI/CD)
git push origin "$TAG"

echo ""
echo "╔══════════════════════════════════════════════════════╗"
echo "║  ✅  Tag $TAG publicada!                              ║"
echo "║  🤖  GitHub Actions irá compilar o APK automaticamente║"
echo "║  📋  Acompanhe em: Actions → Build & Release APK      ║"
echo "╚══════════════════════════════════════════════════════╝"
echo ""
echo "🔗  Releases: https://github.com/SEU_USUARIO/blue-notes/releases"
echo ""
