# 🧩 TDD – User Story: “Comprar Entradas”

**Materia:** Ingeniería y Calidad de Software  
**Grupo:** Grupo 15
**Tecnologías:** C# / .NET 8 / xUnit / Moq / ASP.NET Core Minimal API  
**Paradigma:** Desarrollo Dirigido por Pruebas (TDD)  
**Frontend:** HTML + JavaScript (fetch API)
**Integrantes:**
- 👨‍💻 Galiano, Tomas 85824
- 👨‍💻 Reyna, Teodoro 89891
- 👩‍💻 Chudnosky, Paula Milagros 89385
- 👨‍💻 Vera, Agustín Alejandro 83085
- 👨‍💻 Yoles Trucco, Nicolás 81404
- 👩‍💻 Pavon, Florencia 89539
- 👨‍💻 Capellan, Jorge Gabriel 82595
- 👨‍💻 Poltawcew, Ivan Mijail 85470
- 👨‍💻 Nass, Franco David 88534
**Repositorio:** [[GitHub – TDD-Comprar-Entradas-G15](https://github.com/paulachudnosky/TDD-Comprar-Entradas-G15)]

---

## ⚙️ Ejecución

### 1️⃣ Requisitos
- .NET SDK 8.0 o superior  
- (Opcional) VS Code + extensión Live Server

### 2️⃣ Clonar y restaurar
```bash
git clone https://github.com/<tu-repo>/TDD-Comprar-Entradas-G15.git
cd TDD-Comprar-Entradas-G15
dotnet restore
```

### 3️⃣ Ejecutar los tests
```bash
dotnet test
```
Todos los tests deben pasar (7 en total).

### 4️⃣ Ejecutar la API
```bash
dotnet run --project src/EcoHarmony.Tickets.Api
```
Por defecto escucha en:
http://localhost:5080/swagger

### 5️⃣ Probar con frontend