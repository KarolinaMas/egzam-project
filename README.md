# Exam Project https://github.com/KarolinaMas/egzam-project

```bash
git clone httpsUrlIšDirektorijos
```

# Kaip paleisti projektą

## 1. Reikalavimai

Prieš paleidžiant projektą įsitikinkite, kad turit:

* [.NET 10 SDK](https://dotnet.microsoft.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

## 2. Duomenų bazės paleidimas (Docker)

Projekto root kataloge paleiskite:

```bash
docker compose up -d
```

Tai sukurs:

* MySQL serverį
* phpMyAdmin (DB per browser)

---

## 3. phpMyAdmin (DB per browser)

Atidaryk naršyklėje:

```
http://localhost:8080
```

Prisijungimas:

* **Username:** root
* **Password:** my-secret-pw
* **Server:** mysql

---

## 4. Backend API paleidimas

Eikite į backend projekto folderį ir paleiskite:

```bash
dotnet run
```

API bus pasiekiamas:

```
[https://localhost:5001](http://localhost:5087)
arba terminale bus nurodomas kitas localhost port
```

---

## 5. Postman (API Testing)

If you do not have Postman installed, download and install the Postman Desktop App.

You can use Postman to test all available API endpoints.

### User Registration

**Endpoint:**

```http
POST /api/auth/register
```

**Request Body:**

```json
{
  "userName": "string",
  "email": "string",
  "password": "string"
}
```

### User Login

**Endpoint:**

```http
POST /api/auth/login
```

**Request Body:**

```json
{
  "email": "string",
  "password": "string"
}
```

After a successful login, the API returns a JWT token. Copy only the token value and include it in the `Authorization` header for all protected endpoints:

```text
Authorization: Bearer <your_token>
```

### Create a Task

**Endpoint:**

```http
POST /api/TaskItem
```

**Request Body:**

```json
{
  "title": "Task title",
  "description": "Task description"
}
```

### Get Task by ID

**Endpoint:**

```http
GET /api/TaskItem/{id}
```

### Get All Tasks (Paginated)

**Endpoint:**

```http
GET /api/TaskItem/pages/{page}/{itemsPerPage}
```

### Update a Task

**Endpoint:**

```http
PUT /api/TaskItem/{id}
```

**Request Body:**

```json
{
  "title": "Updated title",
  "description": "Updated description",
  "isCompleted": true
}
```

### Delete a Task

**Endpoint:**

```http
DELETE /api/TaskItem/{id}
```


## 6. Unit test paleidimas

ExamProject.Services.Tests aplankale atsidarius komandinę eilute paleiskite

```bash
dotnet test
```
---
# Kaip sustabdyti projektą

1. docker compose down
2. egzam-project\backend\ExamProject.API> ^C

---

## Autentifikacija (JWT)

Prisijungimas vyksta per JWT token:

1. Vartotojas prisijungia per `/login`
2. Serveris grąžina token
3. Token siunčiamas kiekviename request:

```
Authorization: Bearer <token>
```

---

## Projekto architektūra

```
Controllers → Services → Entity Framework → MySQL
```

Sluoksniai:

* **Controllers** – API endpointai
* **Services** – verslo logika
* **DbContext** – duomenų bazės valdymas
* **Entities** – duomenų modeliai

---

## Docker Compose paslaugos

* MySQL (3306)
* phpMyAdmin (8080)

---

## Greitas paleidimas (viena komanda)

```bash
docker compose up -d && dotnet run
```

---

## Autorius

Karolina Maščinskaitė
