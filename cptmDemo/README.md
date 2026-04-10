# CPTM Demo

## Executar em desenvolvimento

```bash
npm.cmd install
npm.cmd run dev
```

No modo dev, o proxy do Vite envia `/api` para `http://localhost:5085`.

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
