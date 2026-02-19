# RaceLabs-Inspired Telemetry Overlay

## 🎯 Visão Geral

Aplicação WPF de telemetria profissional para iRacing, inspirada no RaceLabs.

### Características Principais
- **Overlay in-game** transparente e configurável
- **Widgets modulares** (velocidade, delta, pneus, RPM, etc.)
- **Comparador de voltas** (ghost lap) em tempo real
- **Gráficos em tempo real** (ScottPlot ou LiveCharts)
- **Temas customizáveis** (cores, fontes, opacidade)
- **Gravação e replay** de sessões
- **Multi-sim support** (futuro - iRacing first)

---

## 🏗️ Arquitetura

```
RaceLabsOverlay/
├── Core/
│   ├── Telemetry/
│   │   ├── ITelemetryProvider.cs
│   │   ├── IRacingProvider.cs
│   │   └── TelemetryData.cs
│   ├── Widgets/
│   │   ├── IWidget.cs
│   │   ├── WidgetBase.cs
│   │   └── WidgetManager.cs
│   └── Themes/
│       ├── ITheme.cs
│       └── ThemeManager.cs
├── Widgets/
│   ├── Speedometer/
│   ├── DeltaBar/
│   ├── TireTemps/
│   ├── RPMGauge/
│   ├── LapTimer/
│   ├── FuelInfo/
│   └── TrackMap/
├── UI/
│   ├── OverlayWindow.xaml
│   ├── WidgetContainer.cs
│   └── Configuration/
└── Services/
    ├── GhostRecorder.cs
    ├── SessionManager.cs
    └── LapComparator.cs
```

---

## 🎨 Widgets Planeados

| Widget | Descrição | Prioridade |
|--------|-----------|------------|
| **Speedometer** | Velocidade digital/analógica | Alta |
| **DeltaBar** | Diferença para melhor volta (gráfico) | Alta |
| **TireTemps** | Temperaturas dos 4 pneus | Alta |
| **RPMGauge** | RPM com shift lights | Alta |
| **LapTimer** | Tempo atual, última, melhor | Alta |
| **FuelInfo** | Combustível, consumo, voltas restantes | Média |
| **TrackMap** | Mapa da pista com posições | Média |
| **Inputs** | Throttle/Brake/Clutch bars | Média |
| **Standings** | Posição na corrida | Baixa |
| **Relative** | Gap para carros à frente/atrás | Baixa |

---

## 🚀 Fases de Desenvolvimento

### Fase 1: Core (2-3 dias)
- [ ] OverlayWindow sempre no topo
- [ ] Widget system base
- [ ] iRacing telemetry provider
- [ ] Theme engine básico

### Fase 2: Widgets Essenciais (3-4 dias)
- [ ] Speedometer
- [ ] DeltaBar
- [ ] TireTemps
- [ ] RPMGauge
- [ ] LapTimer

### Fase 3: Ghost/Comparação (2-3 dias)
- [ ] Gravação de voltas
- [ ] Ghost visualization
- [ ] Delta por sector

### Fase 4: Polish (2 dias)
- [ ] Config UI
- [ ] Temas adicionais
- [ ] Performance optimization

---

## 📋 Próximos Passos

1. Criar projeto base
2. Implementar OverlayWindow
3. Criar widget system
4. Implementar widgets essenciais

---

*Especificação criada em: 19 Fev 2026*
