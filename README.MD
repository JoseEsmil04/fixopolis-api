# 🏗️ Fixopolis API

API REST desarrollada en **.NET 8** con **Clean Architecture**, **Entity Framework Core**, y **SQL Server (Docker)**.  
Forma parte del proyecto de gestión de ferretería **Fixopolis**, con estructura modular y semilla de datos (seed) inicial.

---

## 📂 Estructura del proyecto

src/
├── Fixopolis.Domain/ # Entidades del dominio (POCOs)
├── Fixopolis.Persistence/ # Contexto EF Core + configuración + Seed
└── Fixopolis.WebApi/ # API principal (.NET 8 Web API)

---

## ⚙️ Tecnologías

- **.NET 8 / C#**
- **Entity Framework Core**
- **SQL Server 2022 (Docker)**
- **Swagger / OpenAPI**
- **Clean Architecture (Domain–Persistence–WebApi)**

---

## 🧱 Configuración inicial

1. **Clonar el repositorio**

```bash
  git clone https://github.com/tu_usuario/fixopolis-api.git
  cd fixopolis-api
```

2. **Levantar SQL Server en Docker**

```bash
  docker compose up -d
```

-Usuario: sa

-Contraseña: MyStrongPass123

-Puerto: 1433

3. **Aplicar migraciones**

```bash
  dotnet ef database update --project src/Fixopolis.Persistence --startup-project src/Fixopolis.WebApi
```

4. **Ejecutar la API**

```bash
dotnet run --project src/Fixopolis.WebApi
```

5. **Probar en Swagger**
   👉 https://localhost:5001/swagger
