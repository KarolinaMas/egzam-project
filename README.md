# 📦 Exam Project – project name

## 🧾 Aprašymas

...

---

# Kaip paleisti projektą

## 1. Reikalavimai

Prieš paleidžiant projektą įsitikinkite, kad turit:

* [.NET 10 SDK](https://dotnet.microsoft.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* (nebūtina) Postman arba Swagger testavimui

---

## 2. Duomenų bazės paleidimas (Docker)

Projekto root kataloge paleisk:

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

Eik į backend projekto folderį ir paleisk:

```bash
dotnet restore
dotnet run
```

API bus pasiekiamas:

```
https://localhost:5001
```

---

## 5. Swagger (API testavimas)

Atidaryk:

```
https://localhost:5001/swagger
```

Čia gali testuoti visus endpointus.

---

## 6. Duomenų bazės inicializacija

Paleidus API, Entity Framework automatiškai:

* pritaiko migrations
* sukuria lenteles
* atnaujina DB struktūrą

---

## 🔐 Autentifikacija (JWT)

Prisijungimas vyksta per JWT token:

1. Vartotojas prisijungia per `/login`
2. Serveris grąžina token
3. Token siunčiamas kiekviename request:

```
Authorization: Bearer <token>
```

---

## 🧱 Projekto architektūra

```
Controllers → Services → Entity Framework → MySQL
```

Sluoksniai:

* **Controllers** – API endpointai
* **Services** – verslo logika
* **DbContext** – duomenų bazės valdymas
* **Entities** – duomenų modeliai

---

## 🐳 Docker Compose paslaugos

* MySQL (3306)
* phpMyAdmin (8080)

---

## ⚡ Greitas paleidimas (viena komanda)

```bash
docker compose up -d && dotnet run
```

---

## 👨‍💻 Autorius

Karolina Maščinskaitė
