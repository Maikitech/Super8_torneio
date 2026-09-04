# Meu Projeto API (ASP.NET Core 8)

Uma Web API RESTful moderna em C# utilizando .NET 8, documentação interativa com Swagger e arquitetura limpa com injeção de dependência.

---

## 📋 Pré-requisitos

Para compilar e executar o projeto, você precisa do **.NET 8 SDK** instalado na sua máquina:

1. Acesse a página oficial da Microsoft: [Download .NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Baixe o instalador x64 do **.NET SDK** (não apenas o Runtime).
3. Execute o instalador e conclua a instalação.
4. Abra um novo terminal e verifique executando:
   ```bash
   dotnet --version
   ```
   *(Deverá exibir uma versão `8.0.xxx`)*

---

## 🚀 Como Executar

No terminal, dentro da pasta do projeto, execute:

```bash
dotnet run
```

Ao iniciar, a API estará acessível em:
- **Swagger UI (Interface Visual):** `http://localhost:5000/swagger` ou `https://localhost:5001/swagger`
- **Redirecionamento automático:** Acessar a raiz `http://localhost:5000/` redireciona diretamente para o Swagger.

---

## 📦 Estrutura de Pastas

```
teste/
├── Controllers/
│   └── ProdutosController.cs     # Endpoints HTTP (GET, POST, PUT, DELETE)
├── Models/
│   └── Produto.cs                # Modelo de dados com validações
├── Services/
│   ├── IProdutoService.cs        # Contrato de serviço
│   └── ProdutoService.cs         # Implementação de regras de negócio em memória
├── Properties/
│   └── launchSettings.json       # Configuração de portas e ambiente
├── appsettings.json              # Configurações de logging e app
├── appsettings.Development.json  # Configurações para ambiente de desenvolvimento
├── MeuProjetoApi.csproj          # Arquivo do projeto .NET 8
├── Program.cs                    # Configuração de middlewares, DI e Swagger
└── README.md                     # Este guia
```

---

## 📡 Endpoints Disponíveis

| Método | Endpoint | Descrição |
|---|---|---|
| `GET` | `/api/produtos` | Retorna todos os produtos cadastrados |
| `GET` | `/api/produtos/{id}` | Retorna detalhes de um produto específico |
| `POST` | `/api/produtos` | Cadastra um novo produto |
| `PUT` | `/api/produtos/{id}` | Atualiza os dados de um produto existente |
| `DELETE` | `/api/produtos/{id}` | Remove um produto do sistema |

### Exemplo de Payload para `POST /api/produtos`

```json
{
  "nome": "Monitor UltraWide 29\"",
  "descricao": "Monitor IPS com resolução 2560x1080 e 75Hz",
  "preco": 1199.90
}
```
