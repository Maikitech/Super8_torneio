# 🎾 Padel Super 8 - Gestor de Torneios Desktop

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![WPF](https://img.shields.io/badge/UI-WPF%20Desktop-0078D7?logo=windows&logoColor=white)](https://learn.microsoft.com/dotnet/desktop/wpf/)
[![SQLite](https://img.shields.io/badge/Database-SQLite%20%2B%20EF%20Core-003B57?logo=sqlite&logoColor=white)](https://www.sqlite.org/)
[![xUnit](https://img.shields.io/badge/Tests-xUnit-blue)](https://xunit.net/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20x64-0078D6?logo=windows)](https://www.microsoft.com/windows)

Um aplicativo desktop moderno, intuitivo e completo para Windows em **C# e WPF (.NET 8)**, desenvolvido especialmente para organizar, cronometrar e gerenciar torneios de **Padel no formato Super 8** (tanto no modo **Rotativo Individual / Americano** quanto no modo **Duplas Fixas com Grupos e Mata-Mata**).

---

## 📌 Sumário
- [Visão Geral](#-visão-geral)
- [Modos de Disputa Oficiais](#-modos-de-disputa-oficiais)
- [Principais Recursos](#-principais-recursos)
- [Arquitetura e Tecnologias](#-arquitetura-e-tecnologias)
- [Estrutura do Repositório](#-estrutura-do-repositório)
- [Pré-requisitos e Como Executar](#-pré-requisitos-e-como-executar)
- [Testes Automatizados](#-testes-automatizados)
- [Publicação de Versão Portátil (Zero Instalação)](#-publicação-de-versão-portátil-zero-instalação)
- [Critérios de Classificação e Desempate](#-critérios-de-classificação-e-desempate)

---

## 🎯 Visão Geral

O **Padel Super 8** automatiza todo o fluxo operacional de um torneio de Padel em formato Super 8, eliminando pranchetas e planilhas manuais propensas a erros.

O sistema calcula em tempo real pontuações, saldo de games, confrontos, cronometragem das quadras, chaveamento eliminatório e possibilita a persistência de históricos em banco de dados SQLite local, geração de súmulas oficiais para impressão/PDF e compartilhamento instantâneo via WhatsApp.

---

## 🏆 Modos de Disputa Oficiais

### 1. Super 8 Rotativo (Formato Americano)
Projetado para **8 atletas individuais inscritos** disputando em **2 quadras simultâneas**:
- **Algoritmo de Parceria Única em 7 Rodadas**: cada atleta joga com cada um dos outros 7 participantes como parceiro de dupla exatamente 1 vez ao longo do torneio (total de 28 parcerias distintas formadas).
- **Sem repetição de parceiros** e com confrontos matematicamente balanceados.
- **Pontuação Individual**: cada atleta acumula seus próprios pontos, vitórias, derrotas, saldo de games e games pró/contra.
- **Pódio e Grande Final dos Top 4**: ao término das 7 rodadas, o sistema destaca os melhores colocados e permite gerar a partida final entre os 4 primeiros colocados com chaveamento cruzado:
  $$\text{Final:} \quad (\text{1º} + \text{4º}) \quad \text{vs} \quad (\text{2º} + \text{3º})$$

### 2. Super 8 Duplas Fixas (Grupos + Mata-Mata)
Projetado para **8 duplas inscritas**:
- **Fase de Grupos**: Divisão automática em **Grupo A** (4 duplas) e **Grupo B** (4 duplas), disputados em 3 rodadas com 2 quadras simultâneas.
- **Mata-Mata Eliminatório Automático**:
  - **Semifinal 1**: 1º colocado do Grupo A vs 2º colocado do Grupo B
  - **Semifinal 2**: 1º colocado do Grupo B vs 2º colocado do Grupo A
  - **Disputa de 3º Lugar**: Perdedor SF1 vs Perdedor SF2
  - **Grande Final**: Vencedor SF1 vs Vencedor SF2

---

## ⚡ Principais Recursos

### ⏱️ Cronômetro Independente por Quadra
- Contagem regressiva em tempo real dedicada para a **Quadra 1** e a **Quadra 2**.
- Configuração padrão de tempo (ex: 20 minutos por partida).
- Controles de **Iniciar / Pausar** e **Resetar**.
- Sinalizador visual e sonoro de **Tempo Esgotado** para controle exato dos tempos de quadra.

### 🎮 Match Center Interativo
- Navegação fluida entre rodadas através de abas.
- Cards modernos por partida exibindo os nomes dos jogadores/duplas e a quadra designada.
- **Botões Rápidos `+` e `-`**: altere o placar com apenas 1 clique diretamente na quadra.
- Alternância dinâmica de status da partida (*Pendente*, *Em Andamento*, *Finalizada*).

### 📊 Tabela de Classificação em Tempo Real
- Recálculo automático e instantâneo a cada ponto ou game inserido.
- Métricas detalhadas:
  - Posição (com destaque de medalhas 🥇🥈🥉)
  - Pontos Totais
  - Jogos Disputados (J)
  - Vitórias (V) e Derrotas (D)
  - Sets Vencidos e Perdidos
  - Games Pró (GP) e Games Contra (GC)
  - Saldo de Games (SG)
  - Percentual de Aproveitamento (%)

### 🗄️ Persistência de Dados Local (SQLite + EF Core)
- Banco de dados SQLite embarcado (`padel.db`) sem necessidade de configurar servidores externos.
- **Salvar Torneio**: grava o estado completo (participantes, pontuações, rodadas e placares).
- **Histórico de Torneios**: lista competições anteriores para consulta, carregamento ou exclusão.

### 📄 Emissão de Súmula Oficial (Impressão & PDF)
- Gera uma súmula oficial formatada em HTML profissional com design moderno.
- Inclui cabeçalho com nome do torneio, data/hora, tabela de classificação completa e grade de resultados de todas as rodadas.
- Botão integrado de impressão com estilos prontos (`@media print`) para salvar em PDF ou imprimir diretamente.

### 📲 Compartilhamento Rápido via WhatsApp
- Botão dedicado que formata toda a classificação e os jogos da rodada em texto legível com emojis.
- Copia diretamente para a **Área de Transferência do Windows**, permitindo colar e enviar no grupo dos jogadores em segundos.

---

## 🛠️ Arquitetura e Tecnologias

- **Linguagem**: [C# 12](https://learn.microsoft.com/dotnet/csharp/)
- **Plataforma**: [.NET 8 (LTS)](https://dotnet.microsoft.com/)
- **Interface Gráfica**: WPF (Windows Presentation Foundation) com padrão de design **MVVM** (Model-View-ViewModel) e data-bindings reativos.
- **Acesso a Dados**: [Entity Framework Core 8](https://learn.microsoft.com/ef/core/) com provedor SQLite (`Microsoft.EntityFrameworkCore.Sqlite`).
- **Testes**: [xUnit](https://xunit.net/) para validação lógica de regras de pontuação e algoritmos combinatórios.

---

## 📂 Estrutura do Repositório

```text
Super8_torneio/
├── PadelSuper8.slnx                     # Arquivo de Solução do Visual Studio / .NET
├── Publicar_Versao_Portatil.bat         # Script para gerar o executável autônomo (.exe)
│
├── PadelSuper8/                         # Projeto Desktop Principal (WPF)
│   ├── PadelSuper8.csproj               # Configurações do projeto e publicação Single-File
│   ├── App.xaml / App.xaml.cs           # Entrada do aplicativo WPF e recursos visuais
│   │
│   ├── Models/                          # Entidades de Domínio
│   │   ├── Jogador.cs                   # Modelo de participante, métricas e estatísticas
│   │   ├── Partida.cs                   # Modelo de confronto, duplas, placar e status
│   │   ├── Rodada.cs                    # Coleção de partidas por etapa
│   │   ├── CronometroQuadra.cs          # Temporizador regressivo DispatcherTimer por quadra
│   │   └── TipoTorneio.cs               # Enum com os modos (Rotativo vs Duplas Fixas)
│   │
│   ├── ViewModels/                      # Lógica de Apresentação (MVVM)
│   │   ├── MainViewModel.cs             # Estado do torneio, comandos, rankings e persistência
│   │   └── RelayCommand.cs              # Implementação de ICommand para UI bindings
│   │
│   ├── Services/                        # Serviços de Negócio
│   │   ├── GeradorTorneioService.cs     # Algoritmo matemático de rodadas e combinações únicas
│   │   ├── CalculadoraClassificacao.cs  # Motor de desempate e estatísticas
│   │   └── TorneioRepository.cs         # Camada de repositório para operações com EF Core
│   │
│   ├── Data/                            # Banco de Dados
│   │   └── PadelDbContext.cs            # Contexto do EF Core e mapeamento SQLite (padel.db)
│   │
│   ├── Views/                           # Telas e Componentes Gráficos
│   │   └── MainWindow.xaml              # Janela principal com Match Center, Cronômetro e Ranking
│   │
│   └── Converters/                      # Conversores de dados XAML (visibilidade, cores, etc.)
│
└── PadelSuper8.Tests/                   # Projeto de Testes Unitários
    ├── PadelSuper8.Tests.csproj
    └── TorneioPadelTests.cs             # Testes do algoritmo de duplas e pontuação
```

---

## 🚀 Pré-requisitos e Como Executar

### Pré-requisitos
- Sistema Operacional: **Windows 10 ou Windows 11 (64-bit)**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado (para compilação via código-fonte).

### Executando em Modo Desenvolvimento

1. Clone este repositório:
   ```powershell
   git clone https://github.com/Maikitech/Super8_torneio.git
   cd Super8_torneio
   ```

2. Execute o projeto WPF:
   ```powershell
   dotnet run --project PadelSuper8/PadelSuper8.csproj
   ```

---

## 🧪 Testes Automatizados

Os testes cobrem:
1. **Algoritmo Combinatório Super 8**: Garante matematicamente a geração das 7 rodadas e que todas as 28 duplas possíveis sejam formadas sem repetição.
2. **Cálculo de Desempate e Classificação**: Valida a atualização de pontos, vitórias, saldo de games e posições.

Para rodar a suíte de testes:

```powershell
dotnet test PadelSuper8.slnx
```

---

## 📦 Publicação de Versão Portátil (Zero Instalação)

O projeto está configurado para gerar um executável independente (Self-Contained Single-File). O usuário final **não precisa** ter .NET SDK ou runtime instalados na máquina para jogar.

### Método 1: Via Script Automático
Basta dar dois cliques no arquivo:
```text
Publicar_Versao_Portatil.bat
```
O executável final será gerado em:
```text
PadelSuper8_Portatil\PadelSuper8.exe
```

### Método 2: Via Terminal
```powershell
dotnet publish PadelSuper8/PadelSuper8.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./PadelSuper8_Portatil
```

O arquivo `PadelSuper8.exe` pode ser copiado diretamente para um pen-drive ou qualquer computador com Windows e executado de imediato.

---

## 📐 Critérios de Classificação e Desempate

A classificação segue as normas oficiais de torneios de Padel por etapas:

1. **Pontos Conquistados** (3 pontos por vitória no jogo padrão)
2. **Número de Vitórias**
3. **Saldo de Games (SG)** = $\text{Games Pró} - \text{Games Contra}$
4. **Games Pró (GP)**
5. **Menos Games Contra (GC)**
6. **Confronto Direto** (quando aplicável)

---

## 📄 Licença

Este projeto é desenvolvido para a comunidade de praticantes e gestores de Padel. Sinta-se à vontade para utilizar, sugerir melhorias e reportar sugestões!
