# 🌐 RaceLabs Overlay Website

Website oficial da aplicação RaceLabs Overlay.

## 🚀 Deploy no GitHub Pages (GRÁTIS!)

### 1. Ativar GitHub Pages no Repositório

1. Ir ao repositório: https://github.com/timimata/RaceLabsOverlay
2. Clicar em **Settings**
3. No menu lateral, clicar em **Pages**
4. Em "Source", selecionar:
   - Branch: `main`
   - Folder: `/ (root)`
   - OU selecionar `/docs` se movermos para lá
5. Clicar **Save**

### 2. Opção A: Deploy na pasta /docs (Recomendado)

```bash
# Mover website para pasta docs
mkdir -p docs
cp -r website/* docs/

# Commit e push
git add docs/
git commit -m "Add website to docs folder for GitHub Pages"
git push

# No GitHub Settings > Pages:
# Source: Deploy from a branch
# Branch: main /docs folder
```

### 3. Opção B: Deploy na branch gh-pages

```bash
# Criar branch orphan para o website
git checkout --orphan gh-pages
git rm -rf .
cp -r website/* .
git add .
git commit -m "Initial website commit"
git push origin gh-pages

# No GitHub Settings > Pages:
# Source: Deploy from a branch
# Branch: gh-pages / (root)
```

### 4. Configurar Domain (Opcional)

Para usar domínio gratuito:

**Opção 1: GitHub Pages (subdomínio gratuito)**
- URL: `https://timimata.github.io/RaceLabsOverlay`
- Já funciona automaticamente!

**Opção 2: Netlify (mais rápido)**
1. Ir a https://www.netlify.com/
2. Clicar "Add new site" > "Import an existing project"
3. Selecionar GitHub > Repositório RaceLabsOverlay
4. Build settings:
   - Build command: (deixar em branco - é static)
   - Publish directory: `website`
5. Deploy!

URL será: `https://racelabs-overlay.netlify.app` (GRÁTIS!)

**Opção 3: Vercel**
1. Ir a https://vercel.com/
2. Importar projeto do GitHub
3. Deploy!

URL será: `https://racelabs-overlay.vercel.app` (GRÁTIS!)

### 5. Verificar Deploy

Após configurar, esperar 1-2 minutos e aceder a:
```
https://timimata.github.io/RaceLabsOverlay
```

Ou ver em **Settings > Pages** o link oficial.

---

## 📁 Estrutura do Website

```
website/
├── index.html      # Página principal
├── style.css       # Estilos
└── script.js       # Interatividade
```

## 🎨 Características

- ✅ Design moderno dark theme
- ✅ Responsivo (mobile-friendly)
- ✅ Animações suaves
- ✅ Preview animado dos widgets
- ✅ SEO otimizado
- ✅ Performance otimizada

## 📝 Customizar

### Alterar cores
Editar `:root` em `style.css`:
```css
:root {
    --accent-green: #00ff00;  /* Alterar cor principal */
    --bg-primary: #0a0a0a;    /* Alterar background */
}
```

### Alterar conteúdo
Editar `index.html` diretamente.

### Adicionar páginas
Criar novos ficheiros HTML e linkar no menu.

---

## 🆓 Porquê é Grátis?

| Serviço | Hosting | Domain | SSL |
|---------|---------|--------|-----|
| GitHub Pages | ✅ Grátis | ✅ subdomínio .github.io | ✅ Grátis |
| Netlify | ✅ Grátis | ✅ subdomínio .netlify.app | ✅ Grátis |
| Vercel | ✅ Grátis | ✅ subdomínio .vercel.app | ✅ Grátis |

**Tudo 100% GRÁTIS!** 🎉

---

## 🔗 Links Úteis

- [GitHub Pages Docs](https://docs.github.com/en/pages)
- [Netlify](https://www.netlify.com/)
- [Vercel](https://vercel.com/)
