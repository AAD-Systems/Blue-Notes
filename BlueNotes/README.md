# 🔵 Blue Notes

> Aplicativo Android de anotações minimalista com tema azul profundo.  
> Construído em **C# + .NET MAUI** — 100% local, sem conta, sem internet.

<p align="center">
  <img src="docs/screenshots/home.png" width="200" />
  <img src="docs/screenshots/editor.png" width="200" />
  <img src="docs/screenshots/notebooks.png" width="200" />
</p>

---

## ⬇️ Download

| Versão | Data | Download |
|--------|------|----------|
| v0.0.0.0.1 | 2025-04-23 | [BlueNotes-v0.0.0.0.1.apk](../../releases/latest) |

> **Como instalar:** Baixe o `.apk`, habilite "Fontes desconhecidas" em  
> Configurações → Segurança → Instalar apps desconhecidos.

---

## ✨ Funcionalidades

| # | Funcionalidade | Descrição |
|---|---------------|-----------|
| 1 | 📝 **Editor de notas** | Título + corpo, auto-save a cada 3s, contador de palavras |
| 2 | 📓 **Cadernos** | Organize notas em cadernos com cor e ícone personalizados |
| 3 | 🏷️ **Etiquetas** | Adicione múltiplas tags por nota, filtre por tag |
| 4 | 🔍 **Busca global** | Busca em tempo real com debounce, histórico de pesquisas |
| 5 | 📌 **Notas fixadas** | Fixe até 5 notas no topo do feed |
| 6 | 🗄️ **Arquivo & Lixeira** | Arquive ou exclua com recuperação fácil |
| 7 | 🎨 **Personalização** | 8 cores de fundo, 3 tamanhos de fonte por nota |
| 8 | 🔒 **Proteção** | Trave notas individualmente com PIN ou biometria |
| 9 | 📤 **Exportação** | Exporte nota como `.txt` ou `.pdf`, compartilhe via ShareSheet |
| 10 | ⚙️ **Configurações** | Tema, idioma, ordenação, auto-save, lixeira configurável |

---

## 🧭 Telas

```
📱 Blue Notes
├── 🏠  Início        — Feed de notas com fixadas e recentes
├── ✏️   Editor        — Editor de nota com toolbar de formatação
├── 📓  Cadernos      — Gerenciar cadernos
├── 🏷️  Etiquetas     — Gerenciar tags
├── 🔍  Busca         — Busca global em tempo real
├── 🗄️  Arquivo       — Notas arquivadas
├── 🗑️  Lixeira       — Notas excluídas (auto-limpa em 30 dias)
├── ⚙️  Configurações — Preferências do app
└── 👤  Perfil        — Estatísticas e sobre o app
```

---

## 🛠️ Stack Técnica

| Camada | Tecnologia |
|--------|-----------|
| Linguagem | C# 12 |
| Framework | .NET MAUI 8 |
| Banco de dados | SQLite (`sqlite-net-pcl`) |
| Arquitetura | MVVM (`CommunityToolkit.Mvvm`) |
| UI Components | `CommunityToolkit.Maui` |
| PDF | `PdfSharpCore` |
| CI/CD | GitHub Actions |
| Plataforma | Android 7.0+ (API 24+) |

---

## 🚀 Como Compilar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) com workload **Mobile development with .NET**
- Android SDK (instalado pelo VS)

### Passos

```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/blue-notes.git
cd blue-notes

# 2. Instale o workload MAUI
dotnet workload install maui-android

# 3. Restaure os pacotes
dotnet restore BlueNotes/BlueNotes.csproj

# 4. Build Debug (rodar no emulador/dispositivo)
dotnet build BlueNotes/BlueNotes.csproj -f net8.0-android

# 5. Build Release APK
dotnet publish BlueNotes/BlueNotes.csproj \
  -f net8.0-android \
  -c Release \
  -p:AndroidPackageFormat=apk \
  -p:AndroidKeyStore=false
```

O APK ficará em `bin/Release/net8.0-android/publish/`.

---

## 📁 Estrutura do Projeto

```
blue-notes/
├── BlueNotes/
│   ├── Models/              # Note, Notebook, Tag, Setting
│   ├── ViewModels/          # MVVM ViewModels (um por tela)
│   ├── Views/
│   │   └── Pages/           # Todas as ContentPages (XAML + CS)
│   ├── Services/            # NoteService, SearchService, ExportService...
│   ├── Resources/
│   │   └── Styles/          # Colors.xaml + Styles.xaml
│   └── Platforms/
│       └── Android/         # AndroidManifest.xml
├── .github/
│   ├── workflows/           # build-apk.yml (CI/CD)
│   └── ISSUE_TEMPLATE/      # Bug report, Feature request
├── CHANGELOG.md
└── README.md
```

---

## 🤝 Contribuindo

1. Fork o projeto
2. Crie sua branch: `git checkout -b feature/minha-feature`
3. Commit: `git commit -m 'feat: adiciona minha feature'`
4. Push: `git push origin feature/minha-feature`
5. Abra um Pull Request

### Padrão de commits

| Prefixo | Uso |
|---------|-----|
| `feat:` | Nova funcionalidade |
| `fix:` | Correção de bug |
| `style:` | Alterações visuais/UI |
| `refactor:` | Refatoração de código |
| `docs:` | Documentação |
| `chore:` | Tasks de manutenção |

---

## 📋 Roadmap

- [ ] Markdown no editor
- [ ] Widget Android
- [ ] Tema claro
- [ ] Drag & drop para reordenar notas fixadas
- [ ] Backup local em JSON
- [ ] Notificações de lembrete

Veja o [CHANGELOG](CHANGELOG.md) para histórico completo de versões.

---

## 📄 Licença

Distribuído sob a licença MIT. Veja `LICENSE` para mais informações.

---

<p align="center">
  Feito com ❤️ e .NET MAUI &nbsp;·&nbsp;
  <a href="../../issues">Reportar Bug</a> &nbsp;·&nbsp;
  <a href="../../issues">Sugerir Feature</a>
</p>
