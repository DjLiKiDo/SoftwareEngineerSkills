## Prompt para GitHub Copilot: Fase 4 - Madurez IaC con Bicep

**Objetivo Principal:** Establecer un sistema completo de recuperación ante desastres (DR), estrategias de backup automatizado y monitoreo exhaustivo para asegurar la resiliencia y la observabilidad de la infraestructura gestionada con Bicep.

**Documento de Referencia:** `Implementación de IaC con Bicep en Entorno Azure M.md` (Secciones: "Gestión de Estados y Recuperación", "Seguridad y Gobernanza" (Auditoría y Monitoreo))

**Tareas a Realizar:**

1.  **Implementar Estrategia de Backup Automatizado para Plantillas:**
    *   **Versionado de Plantillas Compiladas:**
        *   En los pipelines de CI/CD (especialmente los de `infra-compartida` y componentes críticos), añade un paso después de una compilación exitosa (`az bicep build`) para versionar la plantilla ARM JSON resultante.
        *   Almacena estas plantillas compiladas como artefactos de build en Azure DevOps.
        *   Considera también hacer commit de estas plantillas JSON a una rama o repositorio separado si se requiere una trazabilidad adicional fuera de los artefactos de build.
    *   **Backup de Archivos de Parámetros:**
        *   Asegúrate de que los archivos `.bicepparam` estén versionados en Git como parte del código fuente de la infraestructura.

2.  **Diseñar e Implementar Pipelines de Recuperación ante Desastres (DR):**
    *   **Pipeline de Emergencia:**
        *   Crea un pipeline de Azure DevOps específico para escenarios de DR.
        *   Este pipeline debe tener un trigger manual.
        *   Debe ser capaz de tomar una versión específica de las plantillas Bicep (y sus parámetros) o las plantillas ARM JSON compiladas (desde los artefactos de build) y desplegarlas en una región de DR o para restaurar un entorno.
        *   Parametriza el pipeline para seleccionar la versión/artefacto a restaurar y el entorno/región de destino.
    *   **Pruebas de DR:**
        *   Define un plan para probar periódicamente los pipelines de DR en un entorno de no producción para asegurar su funcionalidad.

3.  **Configurar Monitoreo Avanzado y Alertas:**
    *   **Monitoreo de Recursos Desplegados:**
        *   Utiliza Azure Monitor para configurar el monitoreo de los recursos clave desplegados mediante Bicep (ej: VMs, App Services, Bases de Datos, Key Vaults).
        *   Define métricas clave y logs a recolectar.
        *   Crea Dashboards en Azure Portal para visualizar el estado de la infraestructura.
    *   **Alertas:**
        *   Configura alertas en Azure Monitor para notificar sobre problemas críticos, fallos de despliegue (complementando las notificaciones de Azure DevOps), o umbrales de rendimiento excedidos.
        *   Integra estas alertas con los canales de comunicación del equipo (ej: email, Microsoft Teams).
    *   **Log Analytics:**
        *   Asegúrate de que todos los logs relevantes (actividad de Azure, diagnósticos de recursos, logs de aplicación si es pertinente) se envíen a un workspace de Log Analytics centralizado.
        *   Desarrolla consultas KQL (Kusto Query Language) para análisis proactivo y troubleshooting.

4.  **Revisión y Optimización Continua:**
    *   Establece un proceso para revisar y optimizar periódicamente las plantillas Bicep, los módulos, los pipelines y las configuraciones de monitoreo.
    *   Mantén actualizada la documentación de la infraestructura y los procesos de DR.
    *   Realiza revisiones de seguridad y cumplimiento de forma regular.

**Entregables Esperados:**
*   Pipelines CI/CD actualizados para versionar y archivar plantillas ARM compiladas.
*   Un pipeline de recuperación ante desastres funcional y documentado.
*   Un plan de pruebas de DR.
*   Configuraciones de Azure Monitor para el monitoreo de recursos clave.
*   Dashboards de monitoreo.
*   Alertas configuradas para eventos críticos.
*   Un workspace de Log Analytics configurado para la recolección centralizada de logs, con ejemplos de consultas KQL útiles.
*   Un plan para la revisión y optimización continua.

**Consideraciones Finales:**
*   Asegurar que el equipo esté completamente capacitado en el uso de Bicep, los pipelines y los procedimientos de DR.
*   Fomentar una cultura de IaC y DevOps dentro del equipo.
