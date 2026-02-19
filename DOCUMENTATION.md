# 🏁 RaceLabsOverlay - Documentação Completa

## 📖 Introdução

RaceLabsOverlay é uma aplicação de telemetria profissional para iRacing, inspirada no RaceLabs. Oferece overlays in-game configuráveis com widgets modulares para mostrar todos os dados importantes em tempo real.

## ✨ Features

### Core Features
- ✅ **Overlay Transparente** - Sempre visível por cima do iRacing
- ✅ **Widgets Modulares** - Sistema extensível de widgets
- ✅ **Ghost Racing** - Gravar e comparar voltas em tempo real
- ✅ **Temas Customizáveis** - Cores, fontes, opacidade configuráveis
- ✅ **Modo Edição** - Arrastar widgets para posicionar
- ✅ **Click-Through** - Clicar no jogo através do overlay

### Widgets Incluídos
- 🏎️ **Speedometer** - Velocidade com estilo digital
- 📊 **Delta Bar** - Diferença para melhor volta (visual)
- 🌡️ **Tire Temps** - Temperaturas dos 4 pneus (inner/middle/outer)
- 🔧 **RPM Gauge** - RPM com shift lights
- ⏱️ **Lap Timer** - Tempos de volta (atual, última, melhor)
- 👻 **Ghost Comparator** - Comparação em tempo real com ghost
- ⛽ **Fuel Info** - Combustível e consumo
- 🎮 **Inputs** - Barras de throttle/brake/clutch

### Atalhos de Teclado
| Atalho | Ação |
|--------|------|
| `Ctrl + Shift + O` | Toggle modo edição |
| `Ctrl + Shift + H` | Toggle click-through |
| `Ctrl + Shift + R` | Iniciar/parar gravação |

---

## 🏗️ Arquitetura

### Camadas
```
RaceLabsOverlay/
├── Core/                 # Lógica de negócio
│   ├── Telemetry/        # Providers de telemetria
│   └── Widgets/          # Sistema de widgets
├── Widgets/              # Implementações de widgets
├── Services/             # Serviços (Ghost, Session)
├── UI/                   # Interface gráfica
│   └── Themes/           # Temas e estilos
└── App.xaml.cs           # Entry point
```

### Componentes Principais

#### 1. OverlayWindow
Janela WPF transparente que fica sempre no topo:
- Win32 APIs para click-through
- Gerenciamento de widgets
- Modo edição vs overlay mode

#### 2. WidgetManager
Sistema de gerenciamento de widgets:
- Add/remove widgets dinamicamente
- Drag-and-drop no modo edição
- Save/load de layouts
- Update de todos os widgets

#### 3. GhostRecorder
Sistema de ghost racing:
- Gravar voltas em tempo real
- Comparar com volta atual
- Salvar/carregar ghosts
- Cálculo de delta

#### 4. IRacingProvider
Provider de telemetria:
- Interface com irsdkSharp
- Polling a 60 Hz
- Reconexão automática
- Conversão de unidades

---

## 🚀 Como Usar

### Instalação
1. Compilar o projeto em Release
2. Copiar para pasta desejada
3. Executar `RaceLabsOverlay.exe`
4. Abrir iRacing

### Primeira Execução
1. Overlay aparece automaticamente
2. Pressionar `Ctrl + Shift + O` para modo edição
3. Arrastar widgets para posição desejada
4. Pressionar `Ctrl + Shift + O` novamente para voltar ao modo overlay

### Gravar Ghost
1. Completar uma volta rápida
2. Ghost é gravado automaticamente se for a melhor volta
3. Ou pressionar `Ctrl + Shift + R` para gravar manualmente

### Configurar Widgets
- Clicar com botão direito no widget (quando click-through está OFF)
- Selecionar opções de configuração

---

## 🎨 Customização

### Criar Novo Widget

```csharp
public class MyWidget : UserControl, IWidget
{
    public string WidgetName => "My Widget";
    public Size DefaultSize => new Size(200, 100);
    public bool IsConfigurable => true;
    
    public void Update(TelemetryData data)
    {
        // Atualizar UI com dados
        MyTextBlock.Text = $"Speed: {data.Speed:F0}";
    }
    
    public void Configure()
    {
        // Abrir janela de configuração
    }
}
```

### Adicionar Widget ao Overlay

```csharp
// No OverlayWindow.cs
var myWidget = new MyWidget();
_widgetManager.AddWidget(myWidget, new Point(100, 100));
```

### Criar Tema Personalizado

```xml
<!-- Themes/MyTheme.xaml -->
<SolidColorBrush x:Key="BackgroundDarkBrush">#0A0A0A</SolidColorBrush>
<SolidColorBrush x:Key="AccentColorBrush">#FF00FF</SolidColorBrush>
```

---

## 🛠️ Desenvolvimento

### Requisitos
- .NET 8.0 ou superior
- Visual Studio 2022 ou Rider
- iRacing (para testar)
- irsdkSharp NuGet package

### Compilar
```bash
dotnet build -c Release
```

### Estrutura de Pastas
```
RaceLabsOverlay/
├── Core/
│   ├── Telemetry/
│   │   ├── ITelemetryProvider.cs
│   │   └── IRacingProvider.cs
│   └── Widgets/
│       ├── IWidget.cs
│       └── WidgetManager.cs
├── Widgets/
│   ├── SpeedometerWidget.xaml.cs
│   ├── DeltaBarWidget.xaml.cs
│   ├── TireTempsWidget.xaml.cs
│   ├── RPMGaugeWidget.xaml.cs
│   ├── LapTimerWidget.xaml.cs
│   ├── GhostComparatorWidget.xaml.cs
│   └── ...
├── Services/
│   └── GhostRecorder.cs
├── UI/
│   ├── OverlayWindow.xaml.cs
│   └── Themes/
│       └── RaceLabsTheme.xaml
└── App.xaml.cs
```

---

## 📊 Performance

### Otimizações Implementadas
- **60 Hz update rate** - Sincronizado com iRacing
- **Ghost recording a 10 Hz** - Suficiente para comparação
- **Span<T>** - Para parsing eficiente (quando possível)
- **Object pooling** - Reutilização de objetos
- **Async/await** - Não bloqueia UI thread

### Requisitos Mínimos
- Windows 10/11
- 4GB RAM
- iRacing instalado
- GPU com suporte a transparência WPF

---

## 🐛 Troubleshooting

### Overlay não aparece
1. Verificar se iRacing está a correr
2. Verificar permissões de administrador
3. Verificar se há erros no log

### Widgets não atualizam
1. Verificar conexão com iRacing
2. Verificar se o SDK está ativo
3. Reiniciar a aplicação

### Performance baixa
1. Reduzir número de widgets
2. Desativar animações
3. Verificar GPU drivers

---

## 📝 TODO / Roadmap

### Fase 1 (Atual)
- ✅ Sistema base de overlay
- ✅ Widgets essenciais
- ✅ Ghost recording
- ✅ Temas

### Fase 2 (Futuro)
- [ ] Gráficos de telemetria (ScottPlot)
- [ ] Mapa da pista 2D
- [ ] Relative standings
- [ ] Export de dados

### Fase 3 (Futuro)
- [ ] Suporte a outros sims (ACC, F1)
- [ ] Plugin system
- [ ] Comunidade de widgets
- [ ] Mobile companion app

---

## 🤝 Contribuir

1. Fork do repositório
2. Criar branch (`git checkout -b feature/novo-widget`)
3. Commit das alterações
4. Push para o branch
5. Abrir Pull Request

### Guidelines
- Seguir Clean Architecture
- Adicionar testes para novos widgets
- Documentar código
- Manter compatibilidade com temas existentes

---

## 📄 Licença

Projeto open-source para uso pessoal.

---

## 🙏 Créditos

- Inspirado no [RaceLabs](https://racelab.app/)
- Usa [irsdkSharp](https://github.com/axeon-software/irsdkSharp) para telemetria
- Temas baseados em designs de sim racing

---

*Criado em: 19 Fev 2026*  
*Versão: 0.1.0 Alpha*
