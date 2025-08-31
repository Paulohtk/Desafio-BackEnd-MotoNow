# MotoNow – API de Locação de Motos

API para gestão de locações de motos com regras de plano por período, cálculo de multa/diárias extras e validação de CNH categoria A.

## Sumário
- [Pré-requisitos](#pré-requisitos)
- [Estrutura do projeto](#estrutura-do-projeto)
- [Rodando com Docker Compose](#rodando-com-docker-compose)
- [Rodando sem Docker Compose (local)](#rodando-sem-docker-compose-local)
- [Migrações do EF Core](#migrações-do-ef-core)
- [Erros comuns & dicas](#erros-comuns--dicas)
---

## Pré-requisitos

### Com Docker
- Docker Desktop (Windows/Mac) ou Docker Engine (Linux)
- Docker Compose v2

### Sem Docker (execução local)
- .NET 8 SDK
- PostgreSQL 16+
- Ferramentas EF Core (`dotnet tool install --global dotnet-ef`) – opcional, mas útil

---

## Estrutura do projeto

```
/
├─ docker-compose.yml
├─ src/
│  ├─ MotoNow.Api/            # WebAPI (.NET 8)
│  ├─ MotoNow.Infrastructure/ # EF Core, repositórios etc.
│  ├─ MotoNow.Domain/         # Entidades e regras de domínio
│  ├─ MotoNow.Application     # Rotina da aplicação, services, DTOS, abstrações.
│  └─ MotoNow.Consumer/       # Worker estruturado para rotina do rabbitMQ
└─ README.md
```

## Rodando com Docker Compose

1) **Build e subida:**
```bash
docker compose build
docker compose up -d
```

2) **Serviços expostos:**
- API: `http://localhost:8080` (ajuste via `API_HTTP_PORT` no `.env`)
- Postgres: `localhost:5432`

3) **Parar/limpar:**
```bash
docker compose down          # para containers
docker compose down -v       # para containers + volumes (apaga o banco)
```

> O programs já aplica de forma automatica as migrations quando iniciado pela primeira vez a aplicação.

---

## Rodando sem Docker Compose (local)

1) **Suba um Postgres local** (Docker avulso ou instalado na máquina):

```bash
docker run -d --name motonow_db   -e POSTGRES_USER=postgres   -e POSTGRES_PASSWORD=postgres   -e POSTGRES_DB=motonow   -p 5432:5432   postgres:16
```

2) **Restaurar, compilar e rodar a API:**
```bash
cd src/MotoNow.Api
dotnet restore
dotnet build
dotnet run
```

Por padrão, a API sobe em uma porta indicada pelo Kestrel.  
Opcionalmente, force a porta:
```bash
set ASPNETCORE_URLS=http://localhost:8080   # Windows (cmd)
export ASPNETCORE_URLS=http://localhost:8080 # Linux/Mac
dotnet run
```

---

## Migrações do EF Core

  Criado de forma automatica ao rodar a aplicação e o banco ativo.

---

### Regras de negócio relevantes
- **Planos aceitos**: 7, 15, 30, 45, 50 dias.
- **Tarifas diárias**: 7→30; 15→28; 30→22; 45→20; 50→18 (R$).
- **Início**: primeiro dia **após** a criação (ou conforme validação do seu serviço).
- **Somente** entregadores com **CNH categoria A** podem locar.
- **Devolução antecipada**: multa percentual sobre diárias não utilizadas (7→20%; 15→40%;).
- **Devolução tardia**: R$ 50,00 por diária adicional.

---

## Erros comuns & dicas

- **API não conecta no banco**  
  Verifique `ConnectionStrings:Default` (local) ou `DB_CONNECTION` no `.env` (Compose).  
  No Compose, o host da string deve ser **`db`** (nome do serviço).

- **Porta já em uso**  
  Troque `API_HTTP_PORT` no `.env` ou ajuste `ASPNETCORE_URLS`.

- **Migrations falhando**  
  Cheque a tabela de histórico:  
  ```sql
  SELECT * FROM motonow.__efmigrationshistory;
  ```
  Gere uma nova migration se necessário e suba novamente.

- **Dados sumiram**  
  Você executou `docker compose down -v` (removeu volumes).  
  Para persistência, **não** use `-v` ao fazer `down`.

---
