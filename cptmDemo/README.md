# CPTM Demo

## Executar em desenvolvimento

```bash
npm.cmd install
npm.cmd run dev
```

No modo dev, o proxy do Vite envia `/api` para `http://localhost:5085`.

## Subir API + Front com um comando (sem Visual Studio)

```bash
npm.cmd install
npm.cmd run dev:full
```

Esse comando sobe:

- API .NET do projeto `cptmApiTeste` em `http://localhost:5085`
- Frontend Vite em `http://localhost:5173`

Para parar os dois juntos, use `Ctrl + C` no terminal.

## Executar como PWA (build/preview)

```bash
npm.cmd run build
npm.cmd run preview
```

Para build/PWA, configure a URL da API usando `.env`:

```env
VITE_API_BASE_URL=http://localhost:5085
```

Você pode copiar de `.env.example`.

Depois de gerar o build, você também pode subir API + Preview com um comando:

```bash
npm.cmd run preview:full
```
