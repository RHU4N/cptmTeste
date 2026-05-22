# 📋 Guia Completo de Testes - CPTM Demo PWA

## Índice
1. [Preparação do Ambiente](#preparação)
2. [Iniciando os Serviços](#iniciando)
3. [Fluxo Rápido de Regressão](#fluxo-rapido)
4. [Verificação da Instalação PWA](#verificação-pwa)
5. [Testes de Funcionalidade Offline](#testes-offline)
6. [Testes de Sincronização](#testes-sincronização)
7. [Testes de Upload de Fotos](#testes-fotos)
8. [Checklist Completo](#checklist)

---

## ✅ Fluxo Rápido de Regressão {#fluxo-rapido}

Use este fluxo quando quiser validar rapidamente a aplicação inteira (backend + frontend + regras principais).

### 1) Subir a aplicação

Na pasta `cptmDemo`, execute:

```bash
npm.cmd run dev:full
```

**Esperado:**
- API online em `http://localhost:5085`
- Frontend online em `http://localhost:5173`

### 2) Login e navegação inicial

1. Abra `http://localhost:5173`
2. Faça login com um usuário válido
3. Verifique que a tela inicial carrega sem erro

**Esperado:**
- Sem erro de autenticação
- Sem erro de carregamento inicial

### 3) Fluxo de inspeção (usuário comum ou operador)

1. Crie uma nova inspeção com título e descrição
2. Preencha localização (se aplicável) e salve
3. Edite a inspeção criada e salve novamente
4. Confirme que o item atualizado aparece na lista

**Esperado:**
- Create e update funcionando
- Inspeção exibida com data/hora correta

### 4) Validação no Admin (autoria correta)

1. Faça logout
2. Entre com usuário admin
3. Vá para a tela de Admin
4. Na lista de inspeções, valide o campo de usuário

**Esperado:**
- O autor exibido deve ser quem realmente criou a inspeção (ex.: user/operator)
- Não deve aparecer sempre “admin” para todas as inspeções

### 5) Regressão visual do Admin

Com o admin logado, valide a tela principal:

**Esperado:**
- Não existem cards de Logs e Chamados
- Não existe o mapa central na tela principal de Admin
- Botões principais continuam funcionais (Inspeção, Histórico, Admin)

### 6) Sanidade de exclusão

1. Apague uma inspeção de teste
2. Recarregue a tela

**Esperado:**
- Item removido da lista
- Sem erro no fluxo

### 7) Critério de aceite rápido

Considere o teste aprovado se:
- Login e navegação funcionam
- CRUD básico de inspeção funciona
- Admin mostra autoria real da inspeção
- Admin não exibe Logs/Chamados e mapa central

---

## 📱 Preparação do Ambiente {#preparação}

### Passo 1: Limpar Cache e Dados Antigos

**1.1 - Limpar cache do navegador (Chrome/Edge/Firefox):**
- Pressione `Ctrl + Shift + Delete` (Windows) ou `Cmd + Shift + Delete` (Mac)
- Selecione **"Todos os tempos"** ou **"All time"** no período
- Marque:
  - ☑️ Cookies e outros dados de site
  - ☑️ Arquivos em cache
  - ☑️ Dados de aplicativos armazenados em cache
- Clique **"Limpar dados"** / **"Clear data"**

**1.2 - Desinstalar PWA anterior (se existir):**
- Abra https://localhost:5173 (ou a porta que está usando)
- Procure pelo ícone de instalação na barra de endereço (⬇️ ou 🔧)
- Selecione **"Desinstalar"** / **"Uninstall"**

**1.3 - Limpar localStorage (DevTools):**
- Abra **DevTools** (`F12` ou `Ctrl + Shift + I`)
- Vá para **Application** tab → **Local Storage**
- Procure por entradas começadas com `cptm` e delete-as
- Vá para **Service Workers** e clique **"Unregister"** em qualquer SW listado

### Passo 2: Verificar Dependências
```bash
# Abra um terminal PowerShell
# Verifique se Node.js está instalado
node --version
# Deve mostrar v20.0.0 ou superior

# Verifique se npm está funcional
npm --version
```

### Passo 3: Instalar Dependências (primeira vez ou se houver mudanças)
```bash
# Na pasta cptmDemo:
cd e:\Downloads\PI\cptmTeste\cptmDemo
npm install
# Aguarde cerca de 2-3 minutos

# Verificar se completou com sucesso:
npm list vite-plugin-pwa
# Deve mostrar: vite-plugin-pwa@1.2.0
```

---

## 🚀 Iniciando os Serviços {#iniciando}

### Opção A: Comando Único (Recomendado)

```bash
# Na pasta cptmDemo:
npm run preview:full
```

**O que deve acontecer:**
- Terminal mostra **2 processos rodando**:
  ```
  [0] Servidor backend: http://localhost:5085 (dotnet cptmApiTeste)
  [1] Servidor frontend: http://localhost:4173 (Vite preview)
  ```
- Aguarde até ver: `VITE v7.3.1 ready in XXX ms` e `listening on http://localhost:4173`

### Opção B: Em Dois Terminais Separados

**Terminal 1 (Backend .NET):**
```bash
cd e:\Downloads\PI\cptmTeste\cptmApiTeste
dotnet run
# Aguarde: "Now listening on: http://localhost:5085"
```

**Terminal 2 (Frontend Vue):**
```bash
cd e:\Downloads\PI\cptmTeste\cptmDemo
npm run preview
# Aguarde: "VITE v7.3.1 ready in XXX ms"
```

---

## 🔍 Verificação da Instalação PWA {#verificação-pwa}

### Passo 1: Abrir Aplicativo no Navegador

1. Abra Chrome, Edge ou Firefox
2. Navegue para: **http://localhost:4173**
3. **Esperado:** Você deve ver a tela de **Login** da CPTM Demo

### Passo 2: Verificar Manifest e Service Worker (DevTools)

**2.1 - Abra DevTools:**
- Pressione `F12` ou `Ctrl + Shift + I`
- Vá para aba **Application** (Chrome/Edge) ou **Storage** (Firefox)

**2.2 - Verificar Manifest:**
- Clique em **Manifest** (lado esquerdo)
- **Esperado:** Você deve ver:
  ```json
  {
    "name": "CPTM Demo",
    "short_name": "CPTM",
    "description": "Inspecione sem limites",
    "display": "standalone",
    "scope": "/",
    "start_url": "/",
    "theme_color": "#1976d2",
    "background_color": "#ffffff",
    "icons": [...]
  }
  ```

**2.3 - Verificar Service Worker:**
- Clique em **Service Workers** (lado esquerdo)
- **Esperado:** Um SW listado com status **"active and running"** (verde)
- Se não aparecer, recarregue a página (`F5`) e aguarde 3-5 segundos

**2.4 - Verificar Cache Storage:**
- Clique em **Cache Storage** (lado esquerdo)
- **Esperado:** Você deve ver caches nomeados:
  - `cptm-pages-v1`
  - `cptm-images-v1`
  - `cptm-css-js-v1`

### Passo 3: Instalar PWA como App

**Método 1 - Via Ícone na Barra de Endereço (Recomendado):**
1. Procure pela ícone de **download** (⬇️) ou **engrenagem** (⚙️) na barra de endereço
2. Clique nela
3. Selecione **"Instalar"** / **"Install"** / **"Instalar aplicativo"**
4. **Esperado:** Uma janela popup aparece pedindo confirmação
5. Clique **"Instalar"** novamente
6. **Esperado:** O app abre em uma janela separada (sem endereço/botões do navegador)

**Método 2 - Via Menu:**
1. Clique nos 3 pontinhos (⋮) no canto superior direito
2. Selecione **"Instalar aplicativo"** / **"Install CPTM Demo"**
3. Confirme a instalação

**2.5 - Verificar Instalação:**
- Abra o **Gerenciador de Tarefas** (`Ctrl + Shift + Esc`)
- Procure por **"CPTM Demo"** ou **"cptmDemo"** nos processos
- **Esperado:** Processo estar listado como app separado

---

## 🌐 Testes de Funcionalidade Offline {#testes-offline}

### Teste 1: Verificar Status Online (Baseline)

**1.1 - Abra a aplicação:**
- Navegue para http://localhost:4173 ou abra o app instalado
- Faça login (use credentials válidas do seu banco de dados)

**1.2 - Verifique o banner:**
- **Esperado:** Nenhum banner "offline" deve aparecer no topo
- **Esperado:** Página deve carregar inspecções normalmente da API

**1.3 - Verifique o DevTools:**
- Abra **DevTools** (`F12`)
- Aba **Network**
- Veja as requisições sendo feitas:
  - `GET http://localhost:5085/api/Inspecao` - deve retornar **200 OK**

### Teste 2: Simular Perda de Conexão (Sem Internet)

**2.1 - Desativar conexão:**
- **Opção A:** Desplug o cabo de rede / desligue WiFi
- **Opção B:** No DevTools → **Network tab** → Marque **"Offline"** no dropdown de throttling

**2.2 - Verifique a interface:**
- **Esperado:** Um banner **vermelho/laranja** apareça no topo com mensagem: 
  ```
  "Você está offline. O app continua aberto e funcionando com os dados em cache. 
   Suas alterações serão sincronizadas quando a conexão retornar."
  ```

**2.3 - Tente recarregar a página:**
- Pressione `F5` ou `Ctrl + R`
- **Esperado:** Página recarrega normalmente (Service Worker serve cache)
- **Esperado:** Inspecções anteriormente carregadas são exibidas

**2.4 - Verifique DevTools:**
- Aba **Network**
- Tente acessar uma inspeção
- **Esperado:** As requisições mostram **canceladas** ou **offline** (ícone com x)

---

## 📝 Testes de Sincronização {#testes-sincronização}

### Teste 3: Criar Inspeção Offline

**3.1 - Enquanto OFFLINE:**
1. Clique em **"Adicionar Nova Inspeção"** ou botão **"+"**
2. Preencha o formulário:
   - Local: "Teste Offline"
   - Situação: Selecione uma opção
   - Observação: "Criada enquanto offline - deve sincronizar"
   - (Foto é opcional neste teste)
3. Clique **"Salvar"** / **"Enviar"**

**3.2 - Verifique o comportamento:**
- **Esperado:** Inspeção aparece na lista **imediatamente**
- **Esperado:** Um indicador **"Pendente"** ou **"⏳"** aparece ao lado
- **Esperado:** No topo da página, apareça um contador: **"1 operação pendente de sincronização"**

**3.3 - Verifique o localStorage:**
- DevTools → **Application** → **Local Storage** → `http://localhost:4173`
- Procure pela chave: `cptm.inspecoes.queue.v1`
- **Esperado:** Você vê um array com **1 objeto** contendo a inspeção criada

### Teste 4: Criar Múltiplas Operações Offline

**4.1 - Continue offline e crie mais operações:**
1. Clique numa inspeção existente
2. Edite algum campo (ex: "Observação: Editada offline")
3. Clique **"Atualizar"** / **"Salvar"**

**4.2 - Delete uma inspeção:**
1. Procure por uma inspeção antiga
2. Clique botão **"Deletar"** / **"Lixo"** / **"🗑️"**
3. Confirme a deleção

**4.3 - Verifique o contador:**
- **Esperado:** O banner mostra **"3 operações pendentes"** (1 create + 1 update + 1 delete)
- **Esperado:** DevTools → localStorage → `cptm.inspecoes.queue.v1` mostra array com 3 objetos

### Teste 5: Sincronizar Quando Reconectar

**5.1 - Reconecte à internet:**
- **Se desligou WiFi/cabo:** Reconecte manualmente
- **Se usou DevTools Offline:** Desmarque **"Offline"** no dropdown de throttling

**5.2 - Verifique o comportamento automático:**
- **Esperado:** Banner **desaparece** automaticamente
- **Esperado:** Uma progressão aparece: **"Sincronizando... (1/3)"** → **"(2/3)"** → **"(3/3)"** → desaparece
- **Esperado:** Contador pendente volta a **0**

**5.3 - Verifique o DevTools:**
- Aba **Network**
- **Esperado:** 3 requisições **POST/PUT/DELETE** para `http://localhost:5085/api/Inspecao`
- **Esperado:** Todas retornam **200 OK** ou **204 No Content**

**5.4 - Verifique a lista:**
- A nova inspeção aparece com dados sincronizados
- A edição foi aplicada
- A deleção foi concluída

**5.5 - Verifique localStorage:**
- `cptm.inspecoes.queue.v1` agora está **vazio** ou não existe
- `cptm.inspecoes.cache.v1` foi atualizado com os novos dados

---

## 📸 Testes de Upload de Fotos {#testes-fotos}

### Teste 6: Selecionar e Visualizar Foto

**6.1 - Abra formulário de nova inspeção:**
- Online ou Offline funciona
- Vá para **"Adicionar Inspeção"**

**6.2 - Selecione uma foto:**
1. Clique no campo **"Selecionar Foto"** / **"📷 Foto"** / **"Choose File"**
2. Escolha uma imagem da sua máquina (PNG, JPG, etc)
3. **Esperado:** A foto aparece **imediatamente abaixo** do campo como preview

**6.3 - Verifique o preview:**
- **Esperado:** Você vê a imagem em miniatura (altura ~150-200px)
- **Esperado:** Botão **"Remover foto"** ou **"❌"** aparece ao lado

**6.4 - Tente remover:**
1. Clique **"Remover foto"**
2. **Esperado:** A imagem desaparece do preview
3. **Esperado:** Botão de remover também desaparece

**6.5 - Selecione novamente:**
1. Clique para escolher uma foto
2. Selecione outra imagem (ou a mesma)
3. **Esperado:** Nova foto aparece no preview

### Teste 7: Enviar Inspeção com Foto (Online)

**7.1 - Esteja ONLINE:**
- Conecte à internet
- Banner offline não deve aparecer

**7.2 - Crie inspeção com foto:**
1. Preencha os campos:
   - Local: "Teste Com Foto"
   - Situação: Selecione
   - Observação: "Contém foto"
2. Selecione uma foto (veja o preview aparecer)
3. Clique **"Salvar"** / **"Enviar"**

**7.3 - Verifique o comportamento:**
- **Esperado:** Inspeção é criada imediatamente
- **Esperado:** Nenhuma mensagem de erro

**7.4 - Verifique nos dados:**
- Na lista, clique na inspeção criada
- **Esperado:** A foto pode ser visualizada (se o app suporta visualização)
- Ou verifique no banco de dados se a foto foi armazenada

### Teste 8: Enviar Inspeção com Foto (Offline)

**8.1 - Fique OFFLINE:**
- Desconecte internet / ative Offline no DevTools

**8.2 - Crie inspeção com foto:**
1. Preencha formulário
2. Selecione foto (preview aparece)
3. Clique **"Salvar"**

**8.3 - Verifique o comportamento:**
- **Esperado:** Inspeção aparece na lista
- **Esperado:** Contador mostra **"1 operação pendente"**
- **Esperado:** Banner offline visível

**8.4 - Verifique localStorage:**
- DevTools → Application → Local Storage → `cptm.inspecoes.queue.v1`
- **Esperado:** O objeto contém um campo `foto` com valor **string** (base64 ou data URL)
- **Não deve ser** um objeto File (pois File não é serializável)

**8.5 - Reconecte e sincronize:**
1. Reative conexão
2. **Esperado:** Sincronização ocorre automaticamente
3. **Esperado:** Foto é enviada para o servidor
4. Verifique no DevTools → Network que a requisição POST/PUT foi enviada

---

## ✅ Checklist Completo {#checklist}

Use este checklist para validar que tudo está funcionando:

### ✓ Ambiente Preparado
- [ ] Cache do navegador foi limpado
- [ ] localStorage foi limpado (DevTools)
- [ ] Service Workers antigos foram desinscritos
- [ ] Dependências npm foram instaladas (`npm install`)

### ✓ Serviços Iniciados
- [ ] Backend .NET rodando em http://localhost:5085
- [ ] Frontend Vite rodando em http://localhost:4173
- [ ] Ambos inicializados com um comando (`npm run preview:full`)

### ✓ PWA Instalado
- [ ] Manifest aparece no DevTools
- [ ] Service Worker está "active and running"
- [ ] Cache Storage contém 3+ caches (pages, images, css-js)
- [ ] App foi instalado como app nativo (janela separada)

### ✓ Funcionalidade Online
- [ ] Página carrega normalmente
- [ ] Login funciona
- [ ] Lista de inspecções carrega da API
- [ ] DevTools → Network mostra requisições com status 200

### ✓ Funcionalidade Offline
- [ ] Banner offline aparece quando internet desconecta
- [ ] Página continua funcionando com dados em cache
- [ ] Página pode ser recarregada sem erro

### ✓ Operações Offline
- [ ] Consigo criar nova inspeção offline
- [ ] Indicador "Pendente" aparece
- [ ] Contador de operações pendentes mostra no banner
- [ ] localStorage → `cptm.inspecoes.queue.v1` contém a operação

### ✓ Múltiplas Operações
- [ ] Consigo editar inspeção offline
- [ ] Consigo deletar inspeção offline
- [ ] Contador mostra o número correto (3 operações = 1 create + 1 update + 1 delete)

### ✓ Sincronização
- [ ] Ao reconectar, banner offline desaparece automaticamente
- [ ] Mensagem "Sincronizando..." aparece
- [ ] DevTools → Network mostra 3 requisições (POST/PUT/DELETE)
- [ ] Todas as requisições retornam 200/204
- [ ] Contador pendente volta a 0
- [ ] localStorage → `cptm.inspecoes.queue.v1` fica vazio

### ✓ Fotos
- [ ] Preview de foto aparece após seleção
- [ ] Botão "Remover foto" funciona
- [ ] Inspeção com foto online envia corretamente
- [ ] Inspeção com foto offline envia corretamente após sincronizar
- [ ] localStorage armazena foto como string (não File)

---

## 🆘 Troubleshooting

### Problema: "Service Worker não aparece no DevTools"
**Solução:**
1. Recarregue a página (`F5`)
2. Aguarde 3-5 segundos
3. Verifique novamente em DevTools → Application → Service Workers
4. Se persistir, limpe cache e tente novamente

### Problema: "Offline mode não funciona"
**Solução:**
1. Confirme que está usando DevTools Offline mode ou internet está realmente desligada
2. Verifique se Service Worker está ativo
3. Tente em outra aba do navegador nova
4. Limpe cache e cache storage

### Problema: "Sincronização não ocorre após reconectar"
**Solução:**
1. Verifique se backend .NET está rodando em http://localhost:5085
2. Abra DevTools → Console e procure por erros (mensagens vermelhas)
3. Verifique se localStorage tem dados em `cptm.inspecoes.queue.v1`
4. Manualmente abra http://localhost:5085/api/Inspecao no navegador (deve retornar JSON)

### Problema: "Foto não aparece como preview"
**Solução:**
1. Verifique se selecionou um arquivo de imagem válido (PNG, JPG, GIF)
2. Abra DevTools → Console e procure por erros
3. Tente recarregar e selecionar foto novamente
4. Verifique se o tamanho do arquivo não é muito grande

### Problema: "'npm run preview:full' não funciona"
**Solução:**
1. Verifique se ambas as dependências estão instaladas:
   ```bash
   npm list concurrently
   # Se não aparecer, instale:
   npm install --save-dev concurrently
   ```
2. Use o comando alternativo:
   ```bash
   npx concurrently "npm run dev:api" "npm run preview"
   ```

---

## 📞 Validação Final

Quando todos os itens do checklist estiverem ✓, a implementação PWA está **100% funcional** e pronta para uso!

**A aplicação agora:**
- ✅ Funciona como app nativo (instalável)
- ✅ Continua funcionando sem internet
- ✅ Sincroniza automaticamente quando reconectar
- ✅ Suporta upload e visualização de fotos
- ✅ Armazena dados em cache para velocidade

Aproveite! 🚀
