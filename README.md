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

## 5. Postman (API testavimas)

Jei neturite įsidiegę **Postman Desktop App**, atsisiųskite ir įdiekite ją.

Naudodami Postman galite testuoti visus API endpoint'us.

### Vartotojo registracija

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

### Vartotojo prisijungimas

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

Po sėkmingo prisijungimo gausite JWT tokeną. Nukopijuokite tik jo reikšmę ir pridėkite ją prie visų apsaugotų užklausų (`request`) antraštės (`Header`) lauke:

```text
Authorization: Bearer <jūsų_tokenas>
```

### Užduoties (Task) sukūrimas

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

### Užduoties gavimas pagal ID

**Endpoint:**

```http
GET /api/TaskItem/{id}
```

### Visų užduočių gavimas su puslapiavimu

**Endpoint:**

```http
GET /api/TaskItem/pages/{page}/{itemsPerPage}
```

### Užduoties atnaujinimas

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

### Užduoties ištrynimas

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
