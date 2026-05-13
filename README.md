# cptmTeste

Aplicação de inspeções da CPTM composta por um frontend em Vue/Vite (PWA) e uma API em ASP.NET Core com autenticação JWT e persistência em Oracle.

## Visão geral

O projeto permite:

- autenticação de usuários
- cadastro, edição, listagem e remoção de inspeções
- upload de fotos nas inspeções
- captura e consulta de localização no mapa
- uso offline com cache local e fila de sincronização

## Estrutura do repositório

```text
.
├── cptmDemo/                 # frontend Vue 3 + Vite + PWA
├── cptmApiTeste/             # backend ASP.NET Core + Entity Framework + Oracle
└── GUIA_TESTES.md            # roteiro manual de testes da aplicação/PWA
```

## Stack

- **Frontend:** Vue 3, Vite, vite-plugin-pwa, Leaflet
- **Backend:** ASP.NET Core, Entity Framework Core, JWT Bearer
- **Banco de dados:** Oracle

## Pré-requisitos

- Node.js 20+ e npm
- .NET SDK 10
- Oracle Database acessível pela connection string da API

## Configuração

### 1. Backend

Arquivo principal de configuração:

- `cptmApiTeste/cptmApiTeste/appsettings.json`

A API usa por padrão:

- porta `http://localhost:5085`
- Swagger em ambiente de desenvolvimento
- conexão Oracle definida em `ConnectionStrings:DefaultConnection`
- JWT configurado em `Jwt`

Ao subir a aplicação, as migrations são aplicadas automaticamente.

Usuário padrão criado na inicialização, caso não exista:

- usuário: `operador`
- senha: `operador123`

O código de ativação padrão para cadastro via API é `1234`, salvo se sobrescrito em configuração.

### 2. Frontend

Na pasta `cptmDemo`, copie o arquivo de exemplo:

```bash
cd cptmDemo
cp .env.example .env
```

Valor esperado:

```env
VITE_API_BASE_URL=http://localhost:5085
```

## Como executar

### Backend

```bash
cd cptmApiTeste
dotnet run --project cptmApiTeste/cptmApiTeste.csproj --launch-profile http
```

### Frontend

```bash
cd cptmDemo
npm install
npm run dev
```

No modo de desenvolvimento, o Vite faz proxy de `/api` para `http://localhost:5085`.

### Subir frontend + backend com um comando

```bash
cd cptmDemo
npm install
npm run dev:full
```

## Build

### Frontend

```bash
cd cptmDemo
npm install
npm run build
```

### Backend

```bash
cd cptmApiTeste
dotnet build cptmApiTeste.slnx
```

## Funcionalidades relevantes

- login com JWT
- separação de acesso entre perfil `admin` e `user`
- inspeções com título, descrição, data, foto e localização
- mapa com Leaflet e reverse geocoding
- operação offline com cache local e sincronização posterior
- suporte a instalação como PWA

## Documentação complementar

- README do frontend: `cptmDemo/README.md`
- Guia manual de testes: `GUIA_TESTES.md`

## Validação usada no repositório

- `dotnet build cptmApiTeste.slnx`
- `dotnet test cptmApiTeste.slnx`
- `npm run build` em `cptmDemo` após instalar dependências
