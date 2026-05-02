# Wild Jumper

Projeto de jogo **endless runner 3D** desenvolvido em Unity, com progressão por biomas, coleta de moedas, sistema de vidas e integração opcional com **MindWave** para coleta de sinais de atenção/meditação e geração de dados em CSV.

## Visão Geral

No Wild Jumper, o jogador avança continuamente por pistas geradas/reutilizadas em tempo real, desviando de obstáculos e coletando moedas. O projeto inclui:

- Progressão de cenários por distância (floresta, deserto, halloween, cidade e montanha).
- HUD com distância, moedas e vidas.
- Sistema de colisão com dano e estado de morte.
- Integração com plugin MindWave para leitura de sinais cerebrais.
- Registro de telemetria de gameplay em arquivos `.csv`.

## Stack e Requisitos

- **Engine:** Unity `2022.3.18f1` (LTS)
- **Pipeline de Render:** HDRP (`com.unity.render-pipelines.high-definition` `14.0.10`)
- **Linguagem:** C#
- **Principais pacotes Unity:** TextMeshPro, UGUI, Timeline, Visual Scripting, AI Navigation

### Dependências de terceiros

- `Assets/Plugins/MindwaveUnity` (integração MindWave)
- `Assets/Plugins/Jayrock` (DLLs auxiliares)
- Pacotes de assets 3D/ambiente em `Assets/Imported/*`

## Como Executar

1. Abra o projeto no **Unity Hub** usando a versão `2022.3.18f1`.
2. Aguarde a importação completa dos assets e pacotes.
3. Abra a cena de menu:
   - `Assets/Scenes/Titlescreen.unity`
4. Clique em Play no Editor.
5. Fluxo esperado:
   - `Titlescreen` -> `Gameplay` -> `Gameover`

## Controles (Gameplay)

- `A / D` ou setas: movimentação horizontal
- `Espaço`: pular
- `U`: habilita exibição de dados MindWave no HUD (quando disponível)

## Arquitetura do Projeto

### Cenas principais

- `Assets/Scenes/Titlescreen.unity`: menu principal e configuração inicial.
- `Assets/Scenes/Gameplay.unity`: loop principal do runner.
- `Assets/Scenes/Gameover.unity`: tela de fim de jogo.

### Scripts centrais

- `Assets/Scripts/player_controller.cs`
  - Movimento, pulo, gravidade, velocidade de avanço, colisão com obstáculos/coletáveis, animações de hit/morte/fim.
- `Assets/Scripts/controller_scenario.cs`
  - Reciclagem de plataformas, spawn de cenário, transição de biomas e spawn periódico de obstáculos.
- `Assets/Scripts/ui_controller.cs`
  - Atualização de distância, moedas, vidas e campos de atenção/meditação.
- `Assets/Scripts/TittleButton.cs`
  - Botões da tela inicial, toggle de painéis e fluxo para gameplay.
- `Assets/Scripts/gameover_screen.cs`
  - Retorno ao menu principal.
- `Assets/Scripts/mind_wave.cs`
  - Conexão com MindWave, leitura de métricas EEG e persistência de dados entre cenas.
- `Assets/Scripts/player_data.cs`
  - Criação de CSV e gravação periódica de telemetria do jogador.

## Telemetria e Dados

Os dados de sessão são gravados em `.csv` com colunas:

- `Atencion Level`
- `Meditation Level`
- `Blink`
- `Life`
- `Distance`
- `Speed`
- `Time played`
- `Difficulty`

Arquivos são gerados em:

- `Assets/Data/<nome_jogador>.csv`

> Observação: no repositório existe também a pasta raiz `Data/` com arquivos CSV históricos.

## Estrutura de Pastas (resumo)

- `Assets/Scripts/`: lógica de gameplay e UI.
- `Assets/Scenes/`: cenas principais.
- `Assets/Plugins/`: plugins externos (MindWave/Jayrock).
- `Assets/Imported/`: assets visuais e ambientes.
- `Packages/`: manifesto de pacotes Unity.
- `ProjectSettings/`: configurações do projeto.

## Melhorias Recomendadas

- Padronizar nomenclatura de scripts/cenas (`Title` vs `Tittle`, português/inglês misto).
- Adicionar tratamento de erros para conexão MindWave e ausência de hardware.
- Criar testes de PlayMode para fluxo básico (menu -> gameplay -> gameover).
- Separar configuração de gameplay em ScriptableObjects.

## Licença

Definir uma licença (por exemplo, MIT) antes de distribuição pública.

## Créditos

Projeto baseado em Unity e em pacotes de assets de terceiros presentes em `Assets/Imported` e `Assets/Plugins`.
