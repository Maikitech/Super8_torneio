# 📖 Manual de Uso Oficial - Padel Super 8 Pro

> **Guia completo e descomplicado para organização, cronometragem e gestão de torneios de Padel.**  
> Desenvolvido para organizadores, clubes, arenas, professores, árbitros e praticantes.

---

## 📌 Índice
1. [Apresentação do Sistema](#1-apresentação-do-sistema)
2. [Como Iniciar o Aplicativo (Zero Instalação)](#2-como-iniciar-o-aplicativo-zero-instalação)
3. [Entendendo os Formatos de Disputa](#3-entendendo-os-formatos-de-disputa)
   - [Modo 1: Super 8 Rotativo (Americano)](#modo-1-super-8-rotativo-americano)
   - [Modo 2: Super 8 Duplas Fixas (Grupos + Mata-Mata)](#modo-2-super-8-duplas-fixas-grupos--mata-mata)
4. [Passo a Passo: Como Usar o Sistema](#4-passo-a-passo-como-usar-o-sistema)
   - [Passo 1: Cadastro e Configuração Inicial](#passo-1-cadastro-e-configuração-inicial)
   - [Passo 2: Operando as Partidas no Match Center](#passo-2-operando-as-partidas-no-match-center)
   - [Passo 3: Usando os Cronômetros de Quadra](#passo-3-usando-os-cronômetros-de-quadra)
   - [Passo 4: Acompanhando a Classificação ao Vivo](#passo-4-acompanhando-a-classificação-ao-vivo)
   - [Passo 5: Fases Finais e Mata-Mata](#passo-5-fases-finais-e-mata-mata)
   - [Passo 6: Modo Telão / Pódio de Premiação](#passo-6-modo-telão--pódio-de-premiação)
5. [Exportação e Divulgação](#5-exportação-e-divulgação)
   - [Compartilhamento no WhatsApp](#compartilhamento-no-whatsapp)
   - [Súmula Oficial em PDF / Impressão](#súmula-oficial-em-pdf--impressão)
6. [Histórico e Armazenamento (Banco SQLite)](#6-histórico-e-armazenamento-banco-sqlite)
7. [Auditoria e Logs do Sistema (Diagnóstico de Erros)](#7-auditoria-e-logs-do-sistema-diagnóstico-de-erros)
8. [Perguntas Frequentes (FAQ)](#8-perguntas-frequentes-faq)

---

## 1. Apresentação do Sistema

O **Padel Super 8 Pro** é um aplicativo desktop projetado especificamente para gerenciar competições de Padel com agilidade total. Ele elimina pranchetas de papel e planilhas complexas, garantindo:

- **Chaveamento automático e matematicamente exato** (sem repetição indevida de parcerias).
- **Cálculo instantâneo** de saldo de games, pontuação e critérios oficiais de desempate.
- **Dois cronômetros independentes** para controle dos tempos das quadras.
- **Histórico seguro** gravado no computador em banco de dados local.
- **Relatórios prontos** para WhatsApp e impressora.

---

## 2. Como Iniciar o Aplicativo (Zero Instalação)

O programa pode ser executado diretamente em qualquer computador com **Windows 10 ou Windows 11 (64 bits)**:

1. **Abra a pasta do programa** ou conecte seu pen-drive onde o software está salvo.
2. Dê um duplo clique no executável:
   ```text
   PadelSuper8.exe
   ```
3. O aplicativo abrirá imediatamente em tela cheia/janela centralizada.  
   *Nota: Não é necessário instalar bancos de dados, servidores ou drivers adicionais.*

### Arquivos gerados automaticamente na pasta do programa:
- `padel.db`: Arquivo do banco de dados local SQLite contendo todos os torneios salvos.
- `logs/`: Pasta onde ficam os registros completos de auditoria e data/hora de cada ação realizada.

---

## 3. Entendendo os Formatos de Disputa

O aplicativo suporta oficialmente os dois modelos mais populares e dinâmicos de Super 8:

### Modo 1: Super 8 Rotativo (Americano)
- **Participantes**: 8 atletas individuais inscritos.
- **Quadras**: 2 quadras simultâneas (Quadra 1 e Quadra 2).
- **Rodadas**: 7 rodadas regulares de jogos.
- **O Grande Segredo do Algoritmo**: Cada um dos 8 atletas jogará **exatamente 1 vez como parceiro de cada um dos outros 7 atletas** (total de 28 parcerias formadas). Ninguém repete dupla na fase regular!
- **Classificação Individual**: Os pontos, vitórias e saldo de games conquistados em cada jogo são creditados individualmente para os 2 jogadores da dupla vencedora.
- **Grande Final dos Top 4**: Ao término da 7ª rodada, os 4 melhores atletas do ranking disputam o título no cruzamento:
  $$\text{1º Colocado} + \text{4º Colocado} \quad \text{contra} \quad \text{2º Colocado} + \text{3º Colocado}$$

---

### Modo 2: Super 8 Duplas Fixas (Grupos + Mata-Mata)
- **Participantes**: 8 duplas pré-formadas e fixas.
- **Fase 1 - Grupos (3 Rodadas)**:
  - **Grupo A** (4 duplas): jogam todos contra todos dentro do grupo.
  - **Grupo B** (4 duplas): jogam todos contra todos dentro do grupo.
- **Fase 2 - Semifinais Cruzadas (Mata-Mata)**:
  - **Semifinal 1**: 1º colocado do Grupo A vs 2º colocado do Grupo B
  - **Semifinal 2**: 1º colocado do Grupo B vs 2º colocado do Grupo A
- **Fase 3 - Grande Decisão**:
  - **Grande Final**: Vencedor SF1 vs Vencedor SF2 (disputa do 1º e 2º lugar)
  - **Disputa de 3º Lugar**: Perdedor SF1 vs Perdedor SF2 (disputa do troféu de bronze)

---

## 4. Passo a Passo: Como Usar o Sistema

### Passo 1: Cadastro e Configuração Inicial

Ao abrir o programa, você estará na **Tela de Cadastro**:

1. **Nome do Torneio**: Digite o nome do evento (ex: *"Torneio Noturno Super 8 - Arena Padel"*).
2. **Formato do Torneio**:
   - Marque a opção **"Super 8 Rotativo (Americano)"** se os 8 inscritos forem atletas individuais.
   - Marque a opção **"Super 8 Duplas Fixas"** se as inscrições forem em duplas já formadas.
3. **Participantes (Slots 1 ao 8)**:
   - Digite o nome dos 8 jogadores (ou duplas).
   - *Dica rápida*: Para fazer um teste imediato, clique no botão **"Preencher Exemplo"**. O sistema preencherá os 8 campos com nomes de estrelas do Padel mundial.
4. Clique no botão azul grande **"INICIAR TORNEIO"**.

---

### Passo 2: Operando as Partidas no Match Center

Assim que o torneio é iniciado, a tela de jogos é exibida:

1. **Navegando entre as Rodadas**:
   - Na parte superior do Match Center, você verá os botões de cada rodada: **[Rodada 1]**, **[Rodada 2]**, etc.
   - Clique na aba desejada para visualizar os confrontos daquela rodada.
2. **Cards das Quadras**:
   - Cada rodada exibe as partidas divididas por quadra: **Quadra 1** e **Quadra 2**.
3. **Alterando o Placar**:
   - Em cada time/dupla, há botões rápidos de **`+`** e **`-`**.
   - Basta clicar em **`+`** para subir os games e **`-`** para diminuir se tiver clicado por engano.
4. **Lançando o Resultado (Finalizar Partida)**:
   - Quando o jogo terminar, clique no botão **"Lançar Resultado"** (ou marcar como finalizada).
   - O status mudará para `Finalizada` e os pontos/games serão somados **instantaneamente** na tabela de classificação.
   - *Errou um placar?* Sem problemas! Basta clicar novamente no botão para **reabrir a partida**, corrigir os números e finalizar de novo. O sistema recalcula tudo na hora!

---

### Passo 3: Usando os Cronômetros de Quadra

No topo direito da tela, você encontra os cronômetros dedicados:

- **Quadra 1** e **Quadra 2** possuem cronômetros 100% independentes.
- O tempo padrão inicial é de **20:00 minutos**.
- **Botão Play / Pause**: Inicia ou congela a contagem regressiva da quadra.
- **Botão Resetar**: Reinicia o cronômetro para 20 minutos.
- **Alerta de Fim de Tempo**: Quando o cronômetro zerar (`00:00`), o visor ficará destacado alertando que o tempo da partida terminou.

---

### Passo 4: Acompanhando a Classificação ao Vivo

Ao lado do Match Center, a **Tabela de Classificação** é atualizada em tempo real:

| Coluna | Significado | Como Funciona |
| :---: | :--- | :--- |
| **Pos** | Posição no Ranking | Destaque especial com medalhas de 🥇 Ouro, 🥈 Prata e 🥉 Bronze |
| **Participante** | Nome do Atleta ou Dupla | Nome cadastrado |
| **Pts** | Pontos Ganhos | **3 pontos** por vitória conquistada |
| **J** | Jogos Disputados | Total de partidas finalizadas |
| **V** | Vitórias | Número de vitórias |
| **D** | Derrotas | Número de derrotas |
| **Sets (V/P)** | Sets Vencidos e Perdidos | Saldo de sets |
| **GP** | Games Pró | Total de games que o participante marcou |
| **GC** | Games Contra | Total de games que o participante sofreu |
| **SG** | Saldo de Games | $\text{Games Pró} - \text{Games Contra}$ (positivo ou negativo) |
| **Aprov** | Aproveitamento | Porcentagem de aproveitamento de pontos |

#### Critérios Oficiais de Desempate:
1. Maior número de **Pontos**.
2. Maior número de **Vitórias**.
3. Maior **Saldo de Games (SG)**.
4. Maior número de **Games Pró (GP)**.
5. Menor número de **Games Contra (GC)**.

---

### Passo 5: Fases Finais e Mata-Mata

- **No Modo Duplas Fixas**:  
  Assim que todos os jogos das 3 rodadas de grupos forem finalizados, aparecerá o botão **"Gerar Semifinais"**. Ao clicar, o sistema cria a Rodada 4 com os confrontos $1ºA \times 2ºB$ e $1ºB \times 2ºA$.  
  Finalizadas as semifinais, clique em **"Gerar Grande Final"** para criar a decisão de 1º lugar e a disputa de 3º lugar.

- **No Modo Rotativo**:  
  Ao finalizar as 7 rodadas, o botão **"Gerar Grande Final dos Top 4"** fica disponível para criar a decisão dos 4 primeiros colocados.

---

### Passo 6: Modo Telão / Pódio de Premiação

Quer projetar a cerimônia de encerramento em uma televisão da arena ou telão HDMI?

1. Na barra superior, clique em **"🏆 Modo Telão / Pódio"**.
2. A tela exibirá um pódio visual moderno e vibrante:
   - 🥇 **1º Lugar (Campeão)** com troféu dourado e estatísticas completas.
   - 🥈 **2º Lugar (Vice-Campeão)** com medalha de prata.
   - 🥉 **3º Lugar (3º Colocado)** com medalha de bronze.
3. Para voltar aos jogos, basta clicar no botão **"Fechar Telão"**.

---

## 5. Exportação e Divulgação

### Compartilhamento no WhatsApp
1. Clique no botão verde **"📲 WhatsApp"** na barra superior.
2. O sistema formata automaticamente toda a classificação atualizada com medalhas, saldo de games e os jogos da rodada selecionada.
3. A mensagem é copiada diretamente para a sua Área de Transferência.
4. Abra o WhatsApp Web ou Desktop, vá na conversa do grupo dos jogadores e aperte **`Ctrl + V`** e envie!

### Súmula Oficial em PDF / Impressão
1. Clique no botão **"🖨️ Súmula / Imprimir"** na barra superior.
2. Uma página HTML profissional será gerada e aberta imediatamente no seu navegador padrão (Google Chrome, Microsoft Edge, Firefox, etc.).
3. A súmula já contém o botão **"🖨️ Imprimir / Salvar PDF"**.
4. Basta clicar para imprimir na impressora do clube ou salvar o arquivo em **PDF** para enviar à diretoria ou arquivar.

---

## 6. Histórico e Armazenamento (Banco SQLite)

- **Salvamento Automático**: A cada resultado lançado ou alteração de rodada, o torneio é salvo de maneira transparente no banco SQLite local (`padel.db`).
- **Consultar Torneios Anteriores**:
  1. Clique no botão **"📂 Histórico"** na barra superior.
  2. Uma janela listará todos os torneios salvos por data e hora de criação.
  3. Você pode visualizar os detalhes ou excluir torneios antigos que não deseja mais manter.

---

## 7. Auditoria e Logs do Sistema (Diagnóstico de Erros)

O aplicativo conta com um motor completo de **Auditoria e Logs em Tempo Real** para garantir transparência total e permitir diagnóstico rápido caso ocorra qualquer dúvida ou imprevisto técnico.

### O que fica registrado nos Logs?
- Horário exato (data, hora, minuto, segundo e milissegundo) de cada evento.
- Inicialização do sistema, versão do Windows e diretório de execução.
- Nomes dos atletas cadastrados e formato de torneio selecionado.
- Cada clique de pontuação (`+` e `-`), indicando qual quadra, partida e placar anterior vs novo.
- Finalização e reabertura de partidas.
- Início, pausa, reinício e aviso de tempo esgotado dos cronômetros.
- Operações no banco de dados SQLite (sucessos ou falhas).
- Geração de fases finais, cópia para WhatsApp e súmulas impressas.
- Detalhamento técnico completo de qualquer aviso ou erro inesperado do Windows.

### Como acessar os arquivos de Log?
1. **Pela própria tela do programa**:  
   Na barra superior da janela, clique no botão **"📋 Logs"**. A pasta de logs será aberta imediatamente no Windows Explorer.
2. **Pelo Explorador de Arquivos**:  
   Vá até a pasta onde está o seu executável e abra a subpasta:
   ```text
   logs/
   ```
3. O arquivo terá o nome no formato:
   ```text
   padel_super8_AAAA-MM-DD.log  (exemplo: padel_super8_2026-09-04.log)
   ```
4. Você pode abrir este arquivo com qualquer editor de texto (Bloco de Notas, VS Code, etc.).

---

## 8. Perguntas Frequentes (FAQ)

#### P: Preciso de internet para usar o programa?
**R:** Não! O Padel Super 8 Pro funciona **100% offline**. Você não precisa de conexão com a internet para gerar rodadas, marcar placares, usar os cronômetros ou salvar torneios.

#### P: O que acontece se o computador desligar de repente por falta de energia?
**R:** O sistema salva as partidas e pontuações continuamente no banco local SQLite (`padel.db`). Ao abrir o programa novamente, seus dados salvos estarão seguros no Histórico.

#### P: Posso usar em duas telas (computador do mesário + TV da arena)?
**R:** Sim! Basta conectar um cabo HDMI da TV ao notebook e arrastar a janela do Padel Super 8 para a TV ou usar o **Modo Telão / Pódio** no momento da premiação.

#### P: Digitei o placar de uma partida errado e já cliquei em finalizar. Como arrumar?
**R:** Localize a partida correspondente na rodada e clique no botão para reabri-la. Corrija o placar usando os botões `+` e `-` e clique em finalizar novamente. A classificação geral recalculará tudo automaticamente.

#### P: Onde envio o arquivo de log se eu encontrar algum comportamento estranho?
**R:** Clique em **"📋 Logs"** no topo da tela, copie o arquivo `padel_super8_DATA.log` do dia e envie para a equipe de suporte ou desenvolvedor responsável para análise imediata.

---

*Padel Super 8 Pro — O gestor definitivo para competições ágeis e profissionais de Padel.*
