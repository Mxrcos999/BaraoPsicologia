# API consumida pelo frontend (Barao Psicologia UI)

Este documento lista **todas as rotas HTTP** chamadas pelo Angular (`src/services/*`), com método, query params, corpo, respostas de sucesso esperadas e formato de erro tratado pela UI.

**Base URL:** definida em `src/env.ts` (padrão: `http://localhost:3000`). Todas as rotas abaixo são relativas a essa base.

**Cabeçalhos:** em todas as requisições o `BaseService` envia:

```http
Authorization: Bearer <valor do cookie accessToken>
```

Antes do login o cookie pode estar ausente; o valor enviado segue a implementação atual de `AUTH.DEFAULT_AUTHORIZATION_TOKEN()`.

---

## Convenções

### Modais de seleção (paciente, clínica, sala, profissional)

Os componentes de diálogo reutilizam as mesmas rotas de listagem acima e enviam **`page`** / **`pageSize`** na query, exibindo **`mat-paginator`** com `totalRecords` retornado pelo servidor (inclui `GET /users/options`).

### Listas paginadas (domínio Psicologia)

Endpoints `GET /patients`, `GET /appointments`, `GET /clinics`, `GET /rooms` e **`GET /users/options`** devem retornar:

```json
{
  "data": [],
  "totalRecords": 0
}
```

### Query params comuns de lista

| Parâmetro      | Tipo     | Descrição                                      |
|----------------|----------|------------------------------------------------|
| `searchInput`  | string   | Texto de busca (opcional).                     |
| `page`         | number   | Página (1-based no uso da UI).                 |
| `pageSize`     | number   | Itens por página.                              |

Além disso, o objeto `filterPayload` do frontend é **espalhado** na query string. Exemplos gerados pelo modal de filtros (`mergeFilterResultToPayload`):

- Intervalo de datas: `{idDoFiltro}Start`, `{idDoFiltro}End` (ex.: `cadastroStart`, `cadastroEnd`, `criadoStart`, `criadoEnd`).
- Checkboxes: chave = `id` do campo; valor = opções separadas por vírgula (ex.: `profile=admin,supervisor`).

Outras chaves podem ser adicionadas no futuro da mesma forma (pares chave/valor na query).

### Sem objetos de entidades relacionadas nos JSON de resposta

As respostas de **sucesso** devem conter **apenas os dados do recurso solicitado** e **chaves estrangeiras** (`patientId`, `roomId`, `userId`, `clinicId`, etc.), **sem** embutir objetos completos de entidades relacionadas (`patient`, `room`, `user`, lista `rooms` dentro de `clinic`, etc.).

Para montar telas com nomes ou detalhes da relação, o cliente faz chamadas adicionais, por exemplo:

| Necessidade                         | Chamadas típicas no projeto atual                          |
|-------------------------------------|------------------------------------------------------------|
| Nome do paciente num agendamento    | `GET /patients/:patientId`                                 |
| Número da sala                      | `GET /rooms/:roomId`                                       |
| Nome do profissional (`userId`)     | `GET /users/options` (lista de opções) e cruzamento por `id` |

> **Nota:** no domínio interno/EF, entidades podem ter navegações; o contrato **HTTP** exposto ao frontend segue a regra acima (DTOs sem navegação serializada).

### Erros HTTP (tratamento global)

O `BaseService` encaminha falhas para `useErrors` (`src/utils/hooks/errors-hook.ts`), que **espera** o corpo da resposta de erro no formato:

```json
{
  "errors": {
    "NomeDoCampoOuDominio": ["Mensagem para o usuário"],
    "Outro": ["Erro 1", "Erro 2"]
  }
}
```

Regras:

- `error.errors` deve ser um **objeto** cujos valores são **arrays** de strings.
- Cada string é exibida num `MatSnackBar` com severidade `error`.
- Se o corpo não seguir esse formato (ou não existir `errors`), a UI mostra: **"Ocorreu um erro desconhecido."**

Além disso, o `HttpClient` do Angular expõe o status em `error.status` e o corpo bruto em `error.error` (objeto ou texto, conforme o servidor).

---

## Pacientes

### `GET /patients`

**Query:** `searchInput`, `page`, `pageSize`, + chaves de `filterPayload`.

**Body:** não.

**Sucesso (200):** `IPaged<IPatient>`

```json
{
  "data": [
    {
      "id": 1,
      "name": "",
      "dateBirth": "",
      "phoneNumber": "",
      "parentName": "",
      "parentDegree": "",
      "createdAt": "",
      "updatedAt": null
    }
  ],
  "totalRecords": 0
}
```

### `GET /patients/:id`

**Sucesso (200):** um objeto `IPatient` (mesmo formato de um item de `data`).

### `POST /patients`

**Body (criação):**

```json
{
  "name": "",
  "dateBirth": "",
  "phoneNumber": "",
  "parentName": "",
  "parentDegree": ""
}
```

(`dateBirth` é enviado como ISO string pelo formulário.)

**Sucesso (201/200):** `IPatient` completo (incluindo `id`).

### `PATCH /patients/:id`

**Body:** subconjunto parcial de `IPatient` (campos editáveis).

**Sucesso (200):** `IPatient` atualizado.

### `DELETE /patients/:id`

**Body:** não.

**Sucesso (200/204):** corpo vazio ou ignorado pela UI (`void`).

---

## Consultas (agendamentos)

### `GET /appointments`

**Query:** `searchInput`, `page`, `pageSize`, + filtros.

**Sucesso (200):** `IPaged<IAppointment>`

```json
{
  "data": [
    {
      "id": 1,
      "patientId": 1,
      "userId": "",
      "appointmentDate": "",
      "roomId": 1,
      "status": 1,
      "createdAt": "",
      "updatedAt": null
    }
  ],
  "totalRecords": 0
}
```

**Importante:** não incluir `patient`, `room`, `user` ou outros objetos de relação no JSON (apenas FKs). A UI resolve rótulos com `GET /patients/:id`, `GET /rooms/:id` e `GET /users/options`.

### `GET /appointments/:id`

**Sucesso (200):** mesmo formato de um item da lista — apenas campos do agregado + FKs (sem objetos aninhados).

### `POST /appointments`

**Body:**

```json
{
  "patientId": 0,
  "userId": "",
  "roomId": 0,
  "appointmentDate": "",
  "status": 1
}
```

### `PATCH /appointments/:id`

**Body:** parcial de `IAppointment` / dos campos acima.

**Sucesso (200):** `IAppointment`.

### `DELETE /appointments/:id`

**Sucesso:** corpo ignorado (`void`).

---

## Clínicas

### `GET /clinics`

**Query:** `searchInput`, `page`, `pageSize`, + filtros.

**Sucesso (200):** `IPaged<IClinic>`

```json
{
  "data": [
    {
      "id": 1,
      "name": "",
      "address": "",
      "createdAt": "",
      "updatedAt": null
    }
  ],
  "totalRecords": 0
}
```

Não incluir `rooms` (ou outras coleções de entidades filhas) na clínica; use `GET /rooms?clinicId=...` para listar salas.

### `GET /clinics/:id`

**Sucesso (200):** `IClinic` (apenas `id`, `name`, `address`, `createdAt`, `updatedAt` — **sem** `rooms`).

### `POST /clinics`

**Body:**

```json
{
  "name": "",
  "address": ""
}
```

**Sucesso (200/201):** `IClinic`.

### `PATCH /clinics/:id`

**Body:** parcial `{ "name"?, "address"? }`.

**Sucesso (200):** `IClinic`.

### `DELETE /clinics/:id`

**Sucesso:** `void`.

---

## Salas

### `GET /rooms`

**Query:** `searchInput`, `page`, `pageSize`, `clinicId` (opcional; filtra por clínica), + filtros.

**Sucesso (200):** `IPaged<IRoom>`

```json
{
  "data": [
    {
      "id": 1,
      "number": "",
      "clinicId": 1,
      "createdAt": "",
      "updatedAt": null
    }
  ],
  "totalRecords": 0
}
```

### `GET /rooms/:id`

**Sucesso (200):** `IRoom`.

### `POST /rooms`

**Body:**

```json
{
  "number": "",
  "clinicId": 1
}
```

**Sucesso (200/201):** `IRoom`.

### `PATCH /rooms/:id`

**Body:** parcial `{ "number"?, "clinicId"? }`.

**Sucesso (200):** `IRoom`.

### `DELETE /rooms/:id`

**Sucesso:** `void`.

---

## Profissionais (opções para agendamento)

### `GET /users/options`

Usado para preencher o seletor de psicólogo/profissional.

**Query:** `searchInput`, `page`, `pageSize`, + filtros (mesmo mecanismo de `qparams`).

**Sucesso (200):** lista paginada (`IPaged`), mesmo formato das outras listas:

```json
{
  "data": [
    {
      "id": "",
      "name": "",
      "email": ""
    }
  ],
  "totalRecords": 0
}
```

---

## Usuários / autenticação (rotas `/user/...`)

Estas rotas seguem o `UserService`. Várias respostas são tipadas de forma flexível (`any`) na UI; abaixo está o que o código **efetivamente consome**.

### `POST /user/user-login`

**Body:**

```json
{
  "username": "",
  "password": ""
}
```

**Sucesso (200):** objeto JSON cujas **chaves de primeiro nível** são gravadas como cookies (valor stringificado se for objeto). O código também lê `expirationTimeAccessToken` (número) para `expires` dos cookies.

Exemplos típicos de chaves esperadas em ambientes integrados: `accessToken`, `expirationTimeAccessToken`, `id`, `name`, `email`, `type`, etc. — a UI itera `Object.keys(response)` sem lista fixa.

### `POST /user/forgot-password`

**Body:**

```json
{
  "email": ""
}
```

**Sucesso (200):** qualquer corpo; a tela de login só trata como sucesso e exibe mensagem amigável.

### `POST /user/post-user`

Criação **centralizada** de usuários (qualquer perfil). Não existe mais `POST /user/post-student-user` nem `POST /user/post-admin-user`.

**Body:**

```json
{
  "email": "",
  "name": "",
  "username": "",
  "profile": "admin | atendente | aluno | supervisor"
}
```

**Sucesso (200):** o frontend trata como `IBaseResponse<string>` e usa **`data`** como string (senha provisória exibida no diálogo):

```json
{
  "errors": null,
  "success": true,
  "pageSize": 0,
  "page": 0,
  "totalRecords": 0,
  "data": "senha-provisoria"
}
```

(Campos numéricos de paginação podem ser 0 se não aplicáveis.)

### `GET /user/get-user-list`

Lista usuários (tela de colaboradores / administração).

**Query:** obrigatórios no uso atual: `page`, `pageSize`. Opcional: `searchInput`. Demais chaves vêm de `filterPayload` (ex.: `criadoStart`, `criadoEnd`, `profile`).

**Sucesso (200):** o componente espera pelo menos:

```json
{
  "data": [
    {
      "id": "",
      "name": "",
      "email": "",
      "username": "",
      "profile": "admin | atendente | aluno | supervisor",
      "receiveEmails": true
    }
  ],
  "totalRecords": 0
}
```

Compatível com `IBaseResponse<IUser[]>` se `data` for o array de usuários.

### `DELETE /user/delete-user`

**Query:** `userId` (string).

**Exemplo:** `DELETE /user/delete-user?userId=abc`

**Sucesso:** corpo tratado como `any`.

### `PATCH /user/update-password`

**Body:**

```json
{
  "password": "",
  "currentPassword": ""
}
```

**Sucesso:** qualquer (`any`).

### `PATCH /user/update-name`

**Body:**

```json
{
  "userId": "",
  "name": ""
}
```

**Sucesso:** qualquer (`any`).

### `PUT /user/update-user`

**Query:** `userId` (string) — enviado como query string junto à rota.

**Body:** objeto `IUser` completo no formulário de edição:

```json
{
  "id": "",
  "name": "",
  "email": "",
  "username": "",
  "profile": "admin | atendente | aluno | supervisor",
  "receiveEmails": true
}
```

**Sucesso (200):** `IBaseResponse<IUser>` (a UI só confirma sucesso genérico).

### `PATCH /user/set-received-email`

**Query:** `userId` (string ou número serializado na URL).

**Body:**

```json
{
  "receiveEmail": true
}
```

**Sucesso:** qualquer (`any`).

---

## Resumo rápido de erros

| Situação                         | Comportamento na UI                                                                 |
|----------------------------------|-------------------------------------------------------------------------------------|
| Corpo com `errors` no formato    | Várias mensagens em snackbar (uma por string em cada array).                        |
| Outro formato / corpo vazio      | Snackbar: **"Ocorreu um erro desconhecido."**                                       |
| Falha de rede / CORS / 0        | Mesmo fluxo de `useErrors` (geralmente cai no desconhecido se `error.error` não for o esperado). |

Para depuração, inspecionar no navegador: **status HTTP**, **corpo** (`error.error`) e **URL** chamada (aba Network).

---

## Referência de código

| Área        | Arquivo principal        |
|------------|---------------------------|
| HTTP base  | `src/services/base-service.ts` |
| Erros      | `src/utils/hooks/errors-hook.ts` |
| Pacientes  | `src/services/patient-service.ts` |
| Consultas  | `src/services/appointment-service.ts` |
| Clínicas/salas | `src/services/clinic-service.ts` |
| Usuários   | `src/services/user-service.ts` |
| Tipos      | `src/interfaces/entities/*`, `src/interfaces/shared/paged-response.ts` |