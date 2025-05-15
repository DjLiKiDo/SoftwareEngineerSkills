# Repositorio Base para Desarrollo de Software Asistido por IA

## Introducción

Este repositorio sirve como plantilla y configuración base para iniciar proyectos de software que serán desarrollados en colaboración con Inteligencia Artificial (IA). La estructura y configuración del espacio de trabajo están diseñadas para optimizar la gestión de la documentación y las interacciones con la IA, con el objetivo de que esta pueda desarrollar el proyecto de la forma más autónoma posible. **Este entorno está especialmente pensado para ser utilizado con Visual Studio Code y GitHub Copilot, aprovechando al máximo sus capacidades de asistencia inteligente.**

## Propósito

El principal objetivo de este repositorio es establecer un estándar para:

*   **Organización del Proyecto**: Facilitar a la IA la comprensión de la estructura del código y los artefactos del proyecto.
*   **Gestión de la Documentación**: Centralizar y estructurar la documentación de manera que sea fácilmente accesible y procesable por la IA.
*   **Interacción Eficaz con la IA**: Definir pautas y herramientas para una comunicación y colaboración fluidas con los modelos de IA.
*   **Desarrollo Autónomo**: Proveer las bases para que la IA pueda realizar tareas de desarrollo, refactorización, y generación de código con mínima intervención humana, **especialmente cuando se utilizan herramientas como GitHub Copilot en un entorno como Visual Studio Code.**

## Estructura del Workspace

La estructura de carpetas y archivos está pensada para ser intuitiva tanto para desarrolladores humanos como para la IA. A continuación, se describe la organización general:

```
.
├── README.md                   # Este archivo, introducción al proyecto.
├── Docs/                       # Documentación general, diagramas, especificaciones.
│   └── ...
├── SoftwareEngineerSkills/     # Raíz del proyecto de software principal (ej. solución .NET).
│   ├── SoftwareEngineerSkills.sln
│   ├── src/                    # Código fuente de la aplicación.
│   │   ├── Project.API/        # Proyecto API (ej. ASP.NET Core).
│   │   ├── Project.Application/ # Lógica de aplicación, casos de uso.
│   │   ├── Project.Domain/     # Entidades del dominio, lógica de negocio core.
│   │   ├── Project.Infrastructure/ # Implementaciones de infraestructura (BD, servicios externos).
│   │   └── Project.Common/     # Utilidades y elementos comunes.
│   └── tests/                  # Pruebas unitarias, de integración, etc.
│       ├── Project.UnitTests/
│       └── Project.IntegrationTests/
└── ...                         # Otros archivos de configuración (linters, formatters, etc.)
```

**Consideraciones Clave para la IA:**

*   **Nomenclatura Clara**: Utilizar nombres descriptivos y consistentes para archivos, carpetas, clases y métodos.
*   **Comentarios Significativos**: Documentar el código de forma concisa y clara, explicando el "por qué" además del "cómo", especialmente en lógica compleja.
*   **Documentación en `Docs/`**: Mantener en esta carpeta diagramas de arquitectura, flujos de usuario, decisiones de diseño y cualquier otro artefacto que ayude a la IA a entender el contexto del proyecto.
*   **Prompts y Directrices para la IA**: Considerar la creación de una subcarpeta en `Docs/` (ej. `AI_Prompts/`) para almacenar prompts efectivos y directrices específicas para interactuar con la IA en tareas comunes.

## Primeros Pasos

1.  **Clonar el Repositorio**: `git clone <URL_DEL_REPOSITORIO>`
2.  **Revisar la Documentación**: Familiarizarse con la estructura y las guías en la carpeta `Docs/`.
3.  **Configurar el Entorno de Desarrollo**: 
    *   Asegurarse de tener las herramientas y SDKs necesarios para el stack tecnológico del proyecto (ej. .NET SDK, Node.js, Python).
    *   **Instalar Visual Studio Code**: Es el editor de código recomendado para este flujo de trabajo.
    *   **Instalar la extensión GitHub Copilot en Visual Studio Code**: Para asistencia en la codificación en tiempo real.
4.  **Iniciar la Interacción con la IA**: Utilizar los prompts y directrices sugeridos para comenzar a delegar tareas de desarrollo a la IA.

## Entorno de Desarrollo Recomendado

Para una experiencia óptima y aprovechar al máximo las capacidades de desarrollo asistido por IA, se recomienda el siguiente entorno:

*   **Editor de Código**: **Visual Studio Code**. Su amplia gama de extensiones y su profunda integración con herramientas de desarrollo lo convierten en una opción ideal.
*   **Asistente de IA**: **GitHub Copilot**. Integrado directamente en Visual Studio Code, GitHub Copilot ofrece sugerencias de código, autocompletado avanzado y la capacidad de generar bloques de código a partir de comentarios en lenguaje natural.
*   **Control de Versiones**: Git, gestionado preferiblemente a través de plataformas como GitHub, GitLab o Bitbucket.

## Interacción con la Inteligencia Artificial

Para maximizar la autonomía y eficacia de la IA, especialmente con **GitHub Copilot en Visual Studio Code**, se recomienda:

*   **Proporcionar Contexto Completo**: Al solicitar una tarea, incluir referencias a archivos relevantes, documentación y decisiones de diseño previas.
*   **Ser Específico en los Requisitos**: Definir claramente el comportamiento esperado, las entradas, las salidas y cualquier restricción.
*   **Iterar y Refinar**: La IA puede no acertar a la primera. Estar preparado para revisar, dar feedback y solicitar refinamientos.
*   **Utilizar Herramientas de IA Integradas**: Aprovechar las extensiones y herramientas de IA en el IDE (**Visual Studio Code es primordial aquí, con GitHub Copilot** como principal asistente) para una colaboración más fluida.
*   **Versionar los Prompts**: Guardar y versionar los prompts efectivos puede ahorrar tiempo y mejorar la consistencia en futuras interacciones. **Considera guardar ejemplos de prompts efectivos para GitHub Copilot en la carpeta `Docs/AI_Prompts/`.**

## Contribuciones

Las contribuciones que mejoren esta plantilla base y las directrices para la colaboración con IA son bienvenidas. Por favor, siga las prácticas estándar de desarrollo (fork, branch, pull request).

## Licencia

Este proyecto se distribuye bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles (si aplica).
