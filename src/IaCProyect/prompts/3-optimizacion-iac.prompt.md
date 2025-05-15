## Prompt para GitHub Copilot: Fase 3 - Optimización IaC con Bicep

**Objetivo Principal:** Mejorar la robustez, seguridad y eficiencia de la implementación de IaC mediante la introducción de políticas de Azure, pruebas automatizadas y mejores prácticas de gobernanza.

**Documento de Referencia:** `Implementación de IaC con Bicep en Entorno Azure M.md` (Secciones: "Gestión de Estados y Recuperación" (Azure Policy), "Mejores Prácticas para el Equipo")

**Tareas a Realizar:**

1.  **Implementar Políticas de Azure con Bicep:**
    *   Identifica políticas de Azure relevantes para la gobernanza de los recursos (ej: restricciones de SKU, etiquetado obligatorio, regiones permitidas).
    *   Define estas políticas utilizando Bicep. Considera el ejemplo de `denyResourceChanges` del documento para prevenir cambios manuales.
        ```bicep
        // Ejemplo: policies.bicep
        resource denyResourceChanges 'Microsoft.Authorization/policyAssignments@2022-06-01' = {
          name: 'deny-resource-changes' // Asegúrate de que el nombre sea único y descriptivo
          properties: {
            policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/8e3e61b3-0b32-22d5-4edf-55fca661d4d2' // ID de la política "No permitir cambios en recursos"
            parameters: {
              effect: { value: 'Deny' }
            }
            // scope: subscription() // O un grupo de administración/recursos específico
          }
        }
        ```
    *   Integra el despliegue de estas políticas en los pipelines de `infra-compartida` o en un pipeline dedicado a la gobernanza.

2.  **Integrar Bicep CLI en CI/CD para Validación:**
    *   Modifica los pipelines existentes (o crea nuevos pasos) para incluir la validación y compilación de plantillas Bicep en cada Pull Request (PR) y antes del despliegue.
    *   Utiliza `az bicep build --file path/to/your/main.bicep` para compilar y validar.
    *   Asegúrate de que los errores de compilación fallen el pipeline.
    *   Referencia:
        ```yaml
        # Ejemplo de paso en Azure DevOps Pipeline
        - script: az bicep build --file ./infra-compartida/networking/virtual-network.bicep
          displayName: 'Compile & Validate Shared Networking Bicep'
        ```

3.  **Desarrollar Pruebas de Módulos Bicep:**
    *   Investiga e implementa un framework de pruebas para los módulos Bicep, como Pester (mencionado en el documento) o el framework de pruebas experimental de Bicep.
    *   Crea pruebas de integración para los módulos compartidos clave (ej: verificar que un Key Vault se crea con las políticas de acceso correctas).
    *   Integra la ejecución de estas pruebas en los pipelines CI/CD.

4.  **Implementar Documentación Automatizada:**
    *   Configura un proceso para generar documentación a partir de las plantillas Bicep.
    *   Utiliza `az bicep generate-params --file path/to/main.bicep --output-format json > params.json` para generar archivos de parámetros que pueden ayudar a entender los módulos.
    *   Explora herramientas que puedan generar diagramas o visualizaciones a partir de las plantillas ARM compiladas (resultado de `az bicep build`).

5.  **Fortalecer la Seguridad y Gobernanza en Pipelines:**
    *   **RBAC en Pipelines:** Revisa los Service Principals o Managed Identities utilizados por las conexiones de servicio de Azure DevOps. Asegúrate de que siguen el principio de privilegios mínimos para cada pipeline y entorno.
    *   **Escaneo de Secretos:** Integra una herramienta de escaneo de secretos (como GitGuardian, mencionado en el documento, o alternativas como `trufflehog`) en los pipelines o como hooks de pre-commit para prevenir la exposición accidental de credenciales.
    *   **Auditoría y Logging:** Asegúrate de que los logs de despliegue de Azure DevOps y los logs de actividad de Azure Resource Manager se capturen y almacenen de forma centralizada (ej: en Log Analytics, como se menciona). Configura alertas para despliegues fallidos o actividades sospechosas.

**Entregables Esperados:**
*   Políticas de Azure definidas en Bicep y desplegadas.
*   Pipelines CI/CD actualizados con validación Bicep integrada.
*   Un conjunto de pruebas de integración para los módulos Bicep críticos.
*   Un proceso (aunque sea básico) para la generación de documentación de los módulos Bicep.
*   Configuraciones de RBAC revisadas y optimizadas para los pipelines.
*   Integración de escaneo de secretos.
*   Configuración para la recolección centralizada de logs de auditoría.

**Próximos Pasos (Anticipo Fase 4):**
*   Desarrollo de estrategias de backup automatizado para las plantillas compiladas.
*   Diseño e implementación de pipelines de recuperación ante desastres.
*   Configuración de monitoreo avanzado y alertas para la infraestructura desplegada.
