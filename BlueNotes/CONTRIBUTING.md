# 🤝 Contribuindo com o Blue Notes

Obrigado por querer contribuir! Siga as orientações abaixo.

## Fluxo de Trabalho

```
main ← pull requests ← feature/sua-feature
```

1. **Fork** → 2. **Branch** → 3. **Código** → 4. **PR**

## Padrão de Branch

| Tipo | Formato |
|------|---------|
| Feature | `feature/nome-da-feature` |
| Bug fix | `fix/descricao-do-bug` |
| Docs | `docs/o-que-foi-documentado` |
| Style/UI | `style/componente-alterado` |

## Padrão de Commits (Conventional Commits)

```
feat: adiciona exportação para Markdown
fix: corrige crash ao deletar nota vazia
style: ajusta espaçamento do card de nota
refactor: extrai lógica de busca para SearchService
docs: atualiza README com instrução de build
chore: atualiza dependências NuGet
```

## Checklist antes do PR

- [ ] O código compila sem erros
- [ ] Testei no emulador Android
- [ ] Segui o padrão MVVM do projeto
- [ ] Não hardcodei strings (usar resources)
- [ ] Atualizei o CHANGELOG.md se necessário

## Arquitetura MVVM — Regras

- **Models**: apenas dados e propriedades `[Ignore]`
- **Services**: toda lógica de negócio e acesso ao banco
- **ViewModels**: usam `[ObservableProperty]` e `[RelayCommand]`; não referenciam Views
- **Pages**: apenas animações e interações que precisam do código-behind

## Dúvidas?

Abra uma [Issue](../../issues) com o label `question`.
