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

## API

### Autenticação

```http
POST /api/Auth/login
```

Corpo:

```json
{
  "username": "operador",
  "password": "operador123"
}
```

O login retorna um JWT. Envie-o nas rotas protegidas:

```text
Authorization: Bearer seu-token-jwt
```

Para cadastrar um usuário, use `POST /api/Auth/register` com `username`, `password` e `activationCode`. O código padrão é `1234`, salvo se alterado na configuração.

### Inspeções

Todas as rotas de inspeção exigem autenticação:

| Método   | Rota                      | Descrição                                             |
| -------- | ------------------------- | ----------------------------------------------------- |
| `GET`    | `/api/Inspecao`           | Lista inspeções do usuário; administrador lista todas |
| `GET`    | `/api/Inspecao/{id}`      | Consulta uma inspeção                                 |
| `GET`    | `/api/Inspecao/{id}/foto` | Retorna a foto da inspeção                            |
| `POST`   | `/api/Inspecao`           | Cria uma inspeção                                     |
| `PUT`    | `/api/Inspecao/{id}`      | Atualiza uma inspeção própria                         |
| `DELETE` | `/api/Inspecao/{id}`      | Exclui uma inspeção própria                           |

Cadastro e atualização usam `multipart/form-data`. Os campos principais são `titulo`, `descricao`, `data`, `localizacao`, `latitude`, `longitude` e `Photo`. No cadastro, a foto é obrigatória; a localização pode ser enviada como texto ou coordenadas válidas.

### Usuários

As rotas de usuários exigem o papel `admin`:

| Método   | Rota                   | Descrição        |
| -------- | ---------------------- | ---------------- |
| `GET`    | `/api/Users`           | Lista usuários   |
| `POST`   | `/api/Users`           | Cria usuário     |
| `PUT`    | `/api/Users/{id}/role` | Atualiza o papel |
| `DELETE` | `/api/Users/{id}`      | Remove usuário   |

Os únicos papéis normalizados pela API são `admin` e `user`.

## Modelo de dados

A entidade `Inspecao` é persistida na tabela Oracle `INSPECAO` com título, descrição, data, foto em BLOB, localização, latitude, longitude e usuário proprietário. A entidade `Usuario` é persistida na tabela `USUARIO` com identificador, nome de usuário, senha com hash e papel.

## PWA e modo offline

O frontend usa `vite-plugin-pwa` com atualização automática do service worker. A configuração mantém caches para recursos estáticos, imagens, tiles do OpenStreetMap, reverse geocoding e respostas da API.

Quando a conexão cai, o frontend exibe um banner de offline e mantém alterações de inspeção em uma fila local. Ao recuperar a conexão, as operações pendentes são sincronizadas com a API. Consulte `GUIA_TESTES.md` para validar cache, fila, sincronização, upload e instalação do PWA.

## Configuração segura

Não versione strings de conexão Oracle, segredos JWT ou senhas reais. O projeto contém valores padrão de desenvolvimento em arquivos de configuração; substitua-os por User Secrets, variáveis de ambiente ou configuração protegida antes de qualquer publicação.
