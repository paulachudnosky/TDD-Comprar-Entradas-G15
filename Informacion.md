## 🎯 Objetivo

Implementar la **user story “Comprar Entradas”** aplicando el enfoque **Test Driven Development (TDD)**.  
El sistema permite que un usuario registrado compre entradas para un parque, validando las reglas de negocio y generando un comprobante.

---

## 🧠 Historia de Usuario

> **Como** visitante registrado,  
> **quiero** poder comprar entradas para una fecha disponible,  
> **para** recibir una confirmación de mi compra o saber por qué fue rechazada.

### Criterios de aceptación
- Solo usuarios **registrados** pueden comprar.  
- La **fecha de visita** debe ser actual o futura, y el **parque debe estar abierto** ese día.  
- Se pueden comprar hasta **10 entradas** por transacción.  
- Debe especificarse la **forma de pago** (efectivo o tarjeta).  
- Si es **tarjeta**, redirige a una URL simulada de pago.  
- Si es **efectivo**, marca “pago en boletería”.  
- En ambos casos, envía una **confirmación por email** (mockeada).  

---

## 🧪 Enfoque TDD

El desarrollo se realizó siguiendo el ciclo clásico:

| Etapa | Descripción | Resultado |
|-------|--------------|------------|
| 🟥 **Red** | Se escriben los tests unitarios antes de implementar la lógica. | Tests fallan (NotImplementedException). |
| 🟩 **Green** | Se implementa el código mínimo para que pasen los tests. | Tests pasan exitosamente. |
| 🟦 **Refactor** | Se mejora la estructura, nombres y duplicaciones sin romper los tests. | Código limpio y probado. |

### Pruebas Implementadas (xUnit)
- Rechaza si el usuario no está registrado.  
- Rechaza si la fecha está en el pasado.  
- Rechaza si el parque está cerrado.  
- Rechaza si la cantidad de entradas > 10.  
- Rechaza si no se elige forma de pago.  
- Acepta pago con tarjeta → retorna URL de redirección.  
- Acepta pago en efectivo → marca “pago en boletería”.

---

## 🏗️ Arquitectura del proyecto
```
TDD-Comprar-Entradas-G15/
│
├─ src/
│ ├─ EcoHarmony.Tickets.Domain/ → Dominio puro (reglas y entidades)
│ │ ├─ Entities/ → Visitor, PurchaseRequest, PurchaseResult
│ │ ├─ Services/ → TicketingService + ITicketingService
│ │ └─ Ports/ → Interfaces: IUserRepository, IPaymentGateway, etc.
│ │
│ └─ EcoHarmony.Tickets.Api/ → API REST (Minimal API)
│ ├─ Program.cs → Endpoint POST /tickets/purchase
│ └─ Properties/launchSettings.json (puerto fijo 5080)
│
├─ tests/
│ └─ EcoHarmony.Tickets.Tests/ → Pruebas xUnit + Moq
│ └─ TicketingServiceTests.cs
│
├─ frontend/
│ └─ index.html → Demo HTML + JS para probar la API
│
└─ README.md → Documento del trabajo práctico
```

Arquitectura basada en **puertos y adaptadores**:
- Dominio no depende de infraestructura.  
- Interfaces (ports) permiten testear sin servicios externos.  
- API y frontend son adaptadores que consumen el dominio.

---

---

## 🧰 Tecnologías usadas

| Componente | Tecnología |
|-------------|-------------|
| **Lenguaje** | C# (.NET 8) |
| **Testing** | xUnit + Moq |
| **Backend** | ASP.NET Core Minimal API |
| **Frontend** | HTML + JS (Fetch API) |
| **Documentación** | Markdown + Swagger UI |

---

## 📊 Resultados

- Todos los criterios de aceptación validados por tests automáticos.  
- Cobertura completa del flujo de negocio principal.  
- API y Frontend conectados correctamente.  
- Cumple con prácticas TDD y Clean Architecture.  

---

## 🏁 Conclusión

El trabajo permitió aplicar el **Desarrollo Dirigido por Pruebas (TDD)** en un caso real.  
Se evidenció cómo los tests guían la implementación, garantizan calidad y permiten evolucionar el sistema sin temor a romper funcionalidades existentes.  
El proyecto demuestra la importancia de la **separación de capas**, el uso de **mocks**, y la validación automática de requisitos funcionales.

---

**Repositorio:** [[GitHub – TDD-Comprar-Entradas-G15](https://github.com/paulachudnosky/TDD-Comprar-Entradas-G15)]
