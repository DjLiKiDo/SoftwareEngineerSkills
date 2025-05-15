# Instrucciones para GitHub Copilot

Este archivo proporciona directrices a GitHub Copilot para ayudarle a generar código y respuestas que se alineen mejor con las prácticas y objetivos de este proyecto.

## Propósito del Proyecto

Este repositorio es una plantilla base para proyectos de software desarrollados con asistencia de IA. El objetivo es tener una configuración de workspace que facilite la interacción con la IA para un desarrollo lo más autónomo posible.

**Tecnologías Clave del Workspace:**

*   **IDE Principal:** Visual Studio Code
*   **Asistente IA Principal:** GitHub Copilot
*   **Control de Versiones:** Git (con GitHub, GitLab, o similar)
*   **Stack Tecnológico Ejemplo (SoftwareEngineerSkills):** .NET, C# (pero las instrucciones deben ser adaptables si el stack cambia)

## Directrices Generales para Copilot

1.  **Comprender la Estructura del Proyecto:**
    *   Familiarízate con la estructura de carpetas descrita en el `README.md`.
    *   Presta atención a la separación de responsabilidades entre proyectos (API, Application, Domain, Infrastructure, Common, Tests).
    *   La carpeta `Docs/` es crucial. Contiene documentación relevante, diagramas y, potencialmente, prompts específicos para tareas.
    *   La carpeta `.github/` contiene configuraciones y prompts específicos para Copilot y otras automatizaciones de GitHub.

2.  **Generación de Código:**
    *   **Lenguaje y Estilo:**
        *   Sigue las convenciones de nomenclatura y estilo del código existente en el proyecto. Si es un nuevo proyecto dentro del workspace, pregunta por el lenguaje y estilo preferido si no está claro.
        *   Para C# (.NET), sigue las directrices de Microsoft y las convenciones comunes de la comunidad .NET.
        *   Utiliza nombres descriptivos para variables, funciones, clases, etc.
    *   **Comentarios:**
        *   Genera comentarios Javadoc/XMLDoc significativos para clases públicas, métodos y propiedades.
        *   Explica lógica compleja o decisiones de diseño no obvias.
    *   **Manejo de Errores:**
        *   Implementa un manejo de errores robusto (try-catch, resultados con errores, etc., según el patrón del proyecto).
    *   **Pruebas:**
        *   Cuando generes código para una funcionalidad, considera también generar pruebas unitarias o de integración correspondientes en las carpetas de `tests/`.
    *   **Seguridad:**
        *   Ten en cuenta las mejores prácticas de seguridad al generar código, especialmente para APIs y manejo de datos.

3.  **Interacción y Comunicación:**
    *   **Contexto:** Utiliza el contexto del archivo actual, los archivos abiertos y el `README.md` para informar tus sugerencias. Si se te proporciona un prompt específico desde un archivo `.prompt.md` en `.github/prompts/` o `.github/chats/`, prioriza esas instrucciones.
    *   **Claridad:** Si una solicitud es ambigua, pide clarificación.
    *   **Autonomía:** Intenta completar tareas de la forma más autónoma posible, pero no dudes en pedir confirmación para decisiones importantes de diseño o arquitectura si no están especificadas.
    *   **Iteración:** Prepárate para iterar sobre las soluciones. El primer intento puede no ser perfecto.

4.  **Uso de Documentación del Proyecto:**
    *   Consulta el `README.md` para entender el propósito general y la estructura.
    *   Si se te indica, consulta archivos específicos en la carpeta `Docs/` para obtener información detallada sobre requisitos, diseño o decisiones de arquitectura. Por ejemplo, el archivo `Docs/Implementación de IaC con Bicep en Entorno Azure M.md` (o ahora en `src/IaCProyect/Docs/`) es clave para tareas relacionadas con Infrastructure as Code.

5.  **Trabajando con Archivos Específicos y Estructura de Instrucciones de Copilot:**
    *   **Instrucciones Globales (Este Archivo):**
        *   **Ubicación:** `.github/copilot-instructions.md`
        *   **Propósito:** Contiene las directrices generales para todo el workspace. Estas son las instrucciones de más alto nivel.
    *   **Instrucciones Específicas de Código Fuente (Opcional):**
        *   **Ubicación Potencial:** `SoftwareEngineerSkills/src/.copilot/instructions.md`
        *   **Propósito:** Si se crea, contendría directrices aplicables a todo el código fuente dentro de la carpeta `SoftwareEngineerSkills/src/`.
    *   **Instrucciones por Proyecto/Componente (Opcional y Recomendado para Especificidad):**
        *   **Ubicación Potencial:** `SoftwareEngineerSkills/src/{NombreDelProyecto}/.copilot/instructions.md` (ej. `SoftwareEngineerSkills/src/SoftwareEngineerSkills.API/.copilot/instructions.md`)
        *   **Propósito:** Contendría directrices muy específicas para un proyecto o componente individual (API, Application, Domain, etc.). Estas instrucciones tendrían precedencia para los archivos dentro de ese proyecto/componente.
    *   **Prompts Detallados para Tareas:**
        *   **Ubicación:** `.github/prompts/*.prompt.md` (o subcarpetas como `src/IaCProyect/prompts/`)
        *   **Propósito:** Contienen prompts detallados para tareas específicas o flujos de trabajo. Cuando se te pida ejecutar uno de estos prompts, sigue sus instrucciones cuidadosamente.
    *   **Documentación Principal del Proyecto:**
        *   **Ubicación:** `README.md`
        *   **Propósito:** La guía principal del proyecto.
    *   **Archivos de Configuración:**
        *   **Ejemplos:** `.editorconfig`, `*.csproj`, `*.sln`, `appsettings.json`
        *   **Propósito:** Presta atención a estos archivos para entender las configuraciones del proyecto y generar código compatible.

## Directrices Específicas para "SoftwareEngineerSkills" (Ejemplo .NET)

*   **Arquitectura Limpia (Clean Architecture):** El proyecto `SoftwareEngineerSkills` sigue principios de Arquitectura Limpia. Asegúrate de que el código generado respete las dependencias (Domain no depende de Application, Application no depende de Infrastructure, etc.).
*   **Inyección de Dependencias:** Utiliza la inyección de dependencias (ver `DependencyInjection.cs` en cada proyecto).
*   **Result Pattern:** El proyecto utiliza un `Result` pattern (ver `SoftwareEngineerSkills.Common/Result.cs`) para el manejo de operaciones que pueden fallar. Prefiere este patrón sobre lanzar excepciones para errores de lógica de negocio esperados.
*   **ASP.NET Core:** Para el proyecto API, sigue las convenciones de ASP.NET Core.

## Para Tareas de Infrastructure as Code (IaC) con Bicep (Referencia: `Docs/Implementación de IaC con Bicep en Entorno Azure M.md`)

*   **Herramienta Principal:** Azure Bicep.
*   **Entornos:** Development, Staging, Production.
*   **Pipelines:** Azure DevOps.
*   **Estructura de Repositorios IaC:**
    *   `infra-compartida`: Para recursos compartidos.
    *   Repositorios por componente de aplicación: Para infraestructura específica del componente.
*   **Parametrización:** Utiliza archivos `.bicepparam` para la configuración por entorno.
*   **Módulos Bicep:** Favorece la creación de módulos Bicep reutilizables.
*   **Exportación y Descompilación:** Para infraestructura existente, el proceso es exportar a ARM JSON y luego descompilar a Bicep (`az bicep decompile`).

## Al Escribir o Modificar Archivos

*   **Respeta `.editorconfig`:** Asegúrate de que el formato del código se adhiera a las reglas definidas en el archivo `.editorconfig` del repositorio.
*   **Archivos de Solución y Proyecto (.sln, .csproj):** Si añades nuevos archivos o proyectos, asegúrate de que se referencian correctamente.
*   **Ubicación de Nuevos Archivos/Documentos:** Cuando se solicite la generación de nuevos documentos o un conjunto de archivos para una nueva funcionalidad o proyecto, la ruta de salida por defecto deberá ser `src/NombreDelProyectoSugerido/`. El usuario se encargará de mover o ajustar la ubicación final de estos archivos según sea necesario. Por ejemplo, si se pide generar un nuevo proyecto API llamado "MyNewApi", los archivos se generarán en `src/MyNewApi/`.

Estas instrucciones ayudarán a GitHub Copilot a ser un asistente más efectivo y alineado con los objetivos de este proyecto.