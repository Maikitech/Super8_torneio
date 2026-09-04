# 🎾 Padel Super 8 - Gestor de Torneios Desktop

Um aplicativo desktop completo para Windows em **C# e WPF (.NET 8/10)**, projetado especificamente para gerenciar torneios de **Padel no formato Super 8**.

---

## ⚡ Principais Funcionalidades

### 1. Dois Modos de Torneio Oficiais:
- **Super 8 Rotativo (Americano)**:
  - 8 atletas individuais inscritos.
  - 2 quadras simultâneas.
  - **Algoritmo de Parceria Única em 7 Rodadas**: cada atleta joga com cada um dos outros 7 participantes como parceiro de dupla exatamente 1 vez (28 parcerias distintas).
  - Pontuação, vitórias e saldo de games creditados individualmente.
  - Pódio automático e botão para gerar a **Grande Final dos Top 4** (1º & 4º vs 2º & 3º).

- **Super 8 Duplas Fixas (Grupos + Mata-Mata)**:
  - 8 duplas inscritas.
  - **Fase de Grupos**: Grupo A (4 duplas) e Grupo B (4 duplas) em 3 rodadas.
  - **Mata-Mata Eliminatório Automático**:
    - Semifinal 1: 1º do Grupo A vs 2º do Grupo B
    - Semifinal 2: 1º do Grupo B vs 2º do Grupo A
    - Grande Final e Disputa de 3º Lugar.

### 2. Match Center Interativo:
- Navegação entre rodadas por abas.
- Cards modernos por quadra com botões rápidos `+` e `-` para alterar o placar com 1 clique diretamente na quadra.
- Indicadores de status da partida (*Pendente*, *Em Andamento*, *Finalizada*).

### 3. Tabela de Classificação em Tempo Real:
- Atualização instantânea a cada ponto/game digitado.
- Critérios oficiais de ordenação e desempate:
  1. Pontos Totais
  2. Vitórias
  3. Saldo de Games (SG)
  4. Games Pró (GP)
  5. Menos Games Contra (GC)
- Destaque visual com medalhas para o pódio (🥇 Ouro, 🥈 Prata, 🥉 Bronze).

### 4. Integração com WhatsApp:
- Botão **"Compartilhar no WhatsApp"** copia a tabela de classificação e os jogos da rodada formatados em texto com emojis diretamente para a Área de Transferência do Windows.

---

## 🚀 Como Executar o Aplicativo

No terminal, navegue até a pasta `PadelSuper8` e execute:

```powershell
cd PadelSuper8
dotnet run
```

---

## 🧪 Como Rodar os Testes Automatizados

Para rodar os testes unitários do algoritmo matemático e do cálculo de pontuação:

```powershell
dotnet test
```
