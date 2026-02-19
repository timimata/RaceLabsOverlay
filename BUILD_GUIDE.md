# 🚀 Build e Distribuição

## 📦 Como Criar o Executável (.exe)

### Opção 1: Build Local (Windows)

1. **Abre o terminal na pasta do projeto:**
   ```bash
   cd RaceLabsOverlay
   ```

2. **Corre o script de build:**
   ```bash
   build.bat
   ```

3. **Aguarda** - Vai demorar 1-2 minutos

4. **O executável fica em:**
   ```
   publish\RaceLabsOverlay.exe
   ```

5. **Para distribuir:**
   - Comprime a pasta `publish` num ficheiro `.zip`
   - Upload para GitHub Releases
   - Ou partilha diretamente o ZIP

### Opção 2: Build Automático (GitHub Actions) ⭐ RECOMENDADO

Este é o **mais fácil** e **profissional**:

1. **No GitHub, vai ao teu repositório:**
   https://github.com/timimata/RaceLabsOverlay

2. **Clica em "Actions"** (menu superior)

3. **Clica em "Build and Release"** na lista

4. **Clica no botão "Run workflow"** → "Run workflow"

5. **Aguarda** ~3 minutos

6. **O build vai aparecer em:**
   - Actions → Seleciona o workflow → Artifacts
   - Faz download do `RaceLabsOverlay-Windows`

### Opção 3: Criar Release Oficial 🏆

Quando quiseres lançar uma versão oficial:

1. **No GitHub, vai a "Releases"** (lado direito da página principal)

2. **Clica "Create a new release"**

3. **Clica "Choose a tag"** → **"Create new tag"**
   - Escreve: `v0.1.0` (ou a versão que quiseres)
   - Isto cria automaticamente um build!

4. **Aguarda** ~3 minutos

5. **O release aparece em:**
   https://github.com/timimata/RaceLabsOverlay/releases

6. **O ficheiro ZIP está anexado** ao release!

---

## 📁 O que o Build Cria

O build gera uma pasta `publish/` com:

```
publish/
├── RaceLabsOverlay.exe      ← Executável principal (único ficheiro!)
├── *.dll                    ← Libraries (incluídas no .exe)
└── ... outros ficheiros necessários
```

**Características:**
- ✅ **Single File** - Um .exe que inclui tudo!
- ✅ **Self-Contained** - Não precisa de .NET Runtime instalado
- ✅ **Standalone** - Copia para qualquer PC e corre
- ✅ ~150 MB (inclui .NET runtime)

---

## 📤 Como a Malta Usa

### Para o Utilizador Final:
1. **Download** do `RaceLabsOverlay.zip`
2. **Extrai** para uma pasta
3. **Clica duplo** em `RaceLabsOverlay.exe`
4. **Abre iRacing** e corre!

Não precisa de:
- ❌ Instalar .NET
- ❌ Instalar Visual Studio
- ❌ Compilar nada
- ❌ Configurar nada

---

## 🔄 Build Automático em Cada Push

O workflow `.github/workflows/build-and-release.yml` faz:

1. **Em cada tag `v*`** (ex: v0.1.0, v1.0.0):
   - Compila automaticamente
   - Cria Release no GitHub
   - Anexa o ficheiro ZIP

2. **Manualmente** (botão "Run workflow"):
   - Compila quando quiseres
   - Fica disponível em Actions → Artifacts

---

## 🛠️ Troubleshooting

### "dotnet não é reconhecido"
Instala o .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0

### "Build falha"
Verifica se tens:
- Visual Studio 2022 com ".NET desktop development" workload
- Ou apenas o .NET 8.0 SDK

### "O .exe não corre"
Verifica:
- Windows 10/11 (64-bit)
- Antivírus não está a bloquear
- Todos os ficheiros da pasta `publish` estão presentes

---

## 📊 Tamanho do Ficheiro

| Tipo | Tamanho |
|------|---------|
| Código fonte | ~2 MB |
| Build Debug | ~50 MB |
| **Build Release (para distribuir)** | **~150 MB** |
| ZIP comprimido | ~60-80 MB |

O tamanho grande é porque inclui o .NET Runtime completo para não depender de instalações externas.

---

## 🎯 Resumo

| Método | Complexidade | Resultado |
|--------|-------------|-----------|
| `build.bat` | ⭐ Fácil | Pasta `publish/` local |
| GitHub Actions (manual) | ⭐⭐ Médio | Download do artifact |
| GitHub Release (tag) | ⭐⭐⭐ Automático | Release oficial com ZIP |

**Recomendo:** Usar o método do Release (criar tag) para distribuir oficialmente!

---

*Documentação criada para facilitar a distribuição da aplicação* 🚀
