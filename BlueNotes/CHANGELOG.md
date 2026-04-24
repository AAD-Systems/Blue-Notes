# Changelog — Blue Notes

Todas as mudanças notáveis do projeto serão documentadas aqui.  
Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/).

---

## [v0.0.0.0.1] — 2025-04-23 🎉 Lançamento Inicial

### ✨ Adicionado
- Criação, edição e exclusão de notas com auto-save (3s)
- Editor com seleção de tamanho de fonte (P/M/G)
- Personalização de cor de fundo por nota (8 tons azuis)
- Sistema de cadernos com ícone e cor customizável
- Sistema de etiquetas (tags) com badge de uso
- Busca global com debounce de 300ms
- Histórico de buscas recentes
- Fixar notas no topo do feed (pin)
- Swipe para arquivar, excluir e fixar
- Arquivo de notas (`/archive`)
- Lixeira com auto-limpeza de 30 dias (`/trash`)
- Restauração de notas da lixeira
- Exportação de nota em `.txt`
- Exportação de nota em `.pdf`
- Compartilhamento via Android ShareSheet
- Proteção de notas individuais com cadeado
- Toggle de biometria nas configurações
- Sidebar (Navigation Drawer) com todas as seções
- Tela de Perfil com estatísticas (notas, cadernos, tags)
- Tela de Configurações (tema, idioma, ordenação, auto-save, lixeira)
- Animações de entrada em todas as telas (Fade, SlideX, SlideY)
- Swipe gestures em cards de notas
- Banco de dados SQLite local (sem internet)
- Tema azul escuro (`#0D1B2A`) em todo o app
- Contador de palavras e caracteres no editor
- CI/CD com GitHub Actions (build APK automático)

### 🗄️ Banco de Dados
- Tabelas: `Notes`, `Notebooks`, `Tags`, `NoteTags`, `Settings`
- Seeds automáticos na primeira inicialização

### 🛠️ Técnico
- .NET MAUI 8 — Android target API 34, min API 24
- MVVM com CommunityToolkit.Mvvm
- SQLite via sqlite-net-pcl
- Exportação PDF via PdfSharpCore
- Injeção de dependência nativa do MAUI

---

## [Próximas versões — Roadmap]

### v0.0.0.0.2 — Planejado
- [ ] Suporte a Markdown no editor
- [ ] Widget Android para criação rápida de nota
- [ ] Modo claro (`light theme`)
- [ ] Reordenação de notas fixadas por drag & drop
- [ ] Seleção de ícone por caderno na UI

### v0.0.0.1.0 — Planejado
- [ ] Backup local em JSON
- [ ] Importação de notas `.txt`
- [ ] Notificações de lembrete por nota
- [ ] Pesquisa por tag e caderno combinados

### v0.1.0.0 — Futuro
- [ ] Sync opcional via Google Drive
- [ ] Tema completamente customizável
- [ ] Suporte a imagens embutidas nas notas
