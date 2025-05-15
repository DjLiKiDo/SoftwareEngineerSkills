## Prompt para GitHub Copilot: Fase 1 - Onboarding IaC con Bicep

**Objetivo Principal:** Exportar la infraestructura existente en Azure a plantillas Bicep para iniciar la transición a Infrastructure as Code.

**Documento de Referencia:** `Implementación de IaC con Bicep en Entorno Azure M.md` (Sección: "Exportación de Infraestructura Existente")

**Tareas a Realizar:**

1.  **Identificar Grupos de Recursos Clave:**
    *   Haz una lista de los grupos de recursos críticos que contienen la infraestructura actual a exportar. Considera los entornos (desarrollo, staging, producción) y los componentes de aplicación.

2.  **Exportar Plantillas ARM desde Azure Portal:**
    *   Para cada grupo de recursos identificado, utiliza la funcionalidad "Exportar plantilla" del Portal de Azure.
    *   Guarda las plantillas ARM (archivos JSON) generadas en una estructura de directorios temporal (ej: `temp/arm-exports/[nombre-grupo-recursos]/`).

3.  **Convertir Plantillas ARM a Bicep:**
    *   Utiliza la CLI de Azure para convertir cada plantilla ARM exportada a formato Bicep.
    *   Ejemplo de comando: `az bicep decompile --file temp/arm-exports/[nombre-grupo-recursos]/template.json`
    *   Guarda los archivos Bicep resultantes en una estructura organizada (ej: `infra/exported/[nombre-grupo-recursos]/main.bicep`).

4.  **Revisión Inicial de Archivos Bicep:**
    *   Abre los archivos Bicep generados.
    *   Realiza una revisión inicial para identificar posibles problemas de descompilación o áreas que requieran refactorización manual. Presta atención a nombres de recursos, parámetros y variables.
    *   Documenta cualquier observación o advertencia generada durante el proceso de descompilación.

5.  **Organización de Parámetros (Preliminar):**
    *   Identifica los parámetros comunes y aquellos que varían por entorno en las plantillas Bicep generadas.
    *   Crea archivos `.bicepparam` preliminares si es evidente la necesidad (ej: `infra/exported/[nombre-grupo-recursos]/params.dev.bicepparam`).

**Entregables Esperados:**
*   Un conjunto de archivos Bicep que representan la infraestructura existente.
*   Documentación de cualquier problema encontrado durante la exportación y descompilación.
*   Una estructura de directorios inicial para los artefactos Bicep exportados.

**Próximos Pasos (Anticipo Fase 2):**
*   Refactorización de los módulos Bicep exportados para hacerlos más genéricos y reutilizables.
*   Definición de la estructura final del repositorio `infra-compartida`.
