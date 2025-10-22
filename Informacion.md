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
- Rechaza si el parque está cerrado.  (Lunes y martes cerrado)
- Rechaza si la cantidad de entradas > 10.  
- Rechaza si no se elige forma de pago.  
- Acepta pago con tarjeta → retorna URL de redirección.  
- Acepta pago en efectivo → marca “pago en boletería”.

- Acepta la fecha de hoy si el parque está abierto.  (Opcional)
- En efectivo, también envía email e incluye cantidad y fecha en el mensaje.  (Opcional)
- En tarjeta, el mensaje de confirmación incluye cantidad y fecha. (Opcional)

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
- Se definen puertos que describen operaciones que el dominio necesita para persistencia, consulta, etc.
- Los puertos permiten testear sin servicios externos.
- El dominio no conoce los detalles de los adaptadores, sólo consume mediante los puertos.
- Se proveen adaptadores concretos que implementen los puertosd, como API y frontend.

---

Esto facilita:
- Sustituir una implementación por otra.
- Testear la lógica de negocio aislada mockeando los puertos.
- Mantener el dominio limpio de dependencias externas.

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

## 🔧 Decisiones técnicas

.NET 8:

- Se eligió la versión .NET 8 por su soporte actualizado, mejoras de rendimiento y compatibilidad con Minimal API. Además, es una versión moderna que permite        aprovechar características recientes del framework.

---

xUnit como framework de pruebas:

- xUnit es uno de los frameworks más utilizados en el ecosistema .NET. Permite una buena integración con .NET, paralelismo de pruebas, atributos de configuración    clara, etc. Su uso es una decisión estándar y confiable.

---

Moq para mocking:

- Moq es una librería madura y popular para crear mocks en .NET. Permite configurar comportamientos esperados, verificar invocaciones, etc. Usarla facilita aislar   dependencias en tests unitarios.

---

API HTTP con Minimal API:

Para exponer la funcionalidad al frontend, se empleó una Minimal API en ASP.NET Core. Esto se debe a que:

- La funcionalidad es relativamente sencilla.

- Minimal API permite definir rutas de forma más clara, con menos configuración.

- Mantiene más conectada la lógica de exposición con facilidad de lectura y acoplamiento mínimo.

- Permite centrarse en la lógica de negocio y los adaptadores, sin demasiado peso en la capa HTTP.

---

Frontend simple para demo:

El proyecto incluye un frontend mínimo (HTML + JavaScript usando fetch) como demostración de uso del API. No es un foco de diseño ni es robusto: es solo para mostrar la interacción cliente-servidor. La decisión de mantenerlo simple responde a que el objetivo principal es demostrar la lógica backend y las pruebas, no hacer una aplicación cliente compleja.

---

## 📊 Resultados

- Todos los criterios de aceptación validados por tests automáticos.  
- Cobertura completa del flujo de negocio principal.  
- API y Frontend conectados correctamente.  
- Cumple con prácticas TDD y Clean Architecture.  

---

## 🏁 Conclusión

El trabajo permitió aplicar el **Desarrollo Dirigido por Pruebas (TDD)** en un caso real, mostrando la forma en que las pruebas unitarias guían el diseño y la implementación del sistema. Conseguimos asegurar la calidad del código y detectar de forma temprana los defectos para permitir que el software evolucione de forma consistente, sin romper funcionalidades ya implementadas.
Este proyecto resalta la necesidad de tener una arquitectura basada en la separación de capas, el uso de mocks y pruebas automatizadas para aislar dependencias, y la validación de los requerimientos funcionales para generar un sistema robusto, consistente y escalable.

---

**Repositorio:** [[GitHub – TDD-Comprar-Entradas-G15](https://github.com/paulachudnosky/TDD-Comprar-Entradas-G15)]
