## Prompt para GitHub Copilot: Fase 2 - Core IaC con Bicep

**Objetivo Principal:** Establecer los cimientos de la estrategia de IaC, incluyendo la creación de módulos Bicep reutilizables, la organización de parámetros multientorno y el diseño de pipelines CI/CD base.

**Documento de Referencia:** `Implementación de IaC con Bicep en Entorno Azure M.md` (Secciones: "Arquitectura Target con Bicep", "Organización de Parámetros Multientorno", "Diseño de Pipelines en Azure DevOps")

**Tareas a Realizar:**

1.  **Definir Estructura de Repositorios:**
    *   Basado en la sección "Estructura de Repositorios", crea la estructura de carpetas para `infra-compartida` y un ejemplo de `componente-app1`.
    *   `infra-compartida`:
        *   `networking/` (e.g., `virtual-network.bicep`, `peering.bicep`)
        *   `security/` (e.g., `key-vaults.bicep`, `policies.bicep`)
        *   `pipelines/`
        *   `parameters/dev/`, `parameters/staging/`, `parameters/prod/`
    *   `componente-app1`:
        *   `infra/` (e.g., `app-service.bicep`, `sql-database.bicep`)
        *   `pipelines/`

2.  **Desarrollar Módulos Bicep Compartidos:**
    *   A partir de los Bicep exportados en Fase 1 (o creando nuevos), desarrolla módulos Bicep genéricos y reutilizables para los recursos comunes (ej: Virtual Network, Key Vault, App Service Plan).
    *   Coloca estos módulos en las carpetas correspondientes dentro de `infra-compartida`.
    *   Asegúrate de que los módulos estén parametrizados adecuadamente para soportar diferentes configuraciones y entornos.

3.  **Implementar Estrategia de Parametrización Multientorno:**
    *   Para los módulos compartidos y la infraestructura de componentes, crea archivos `.bicepparam` específicos para cada entorno (dev, staging, prod).
    *   Ejemplo: `infra-compartida/parameters/dev/shared-params.bicepparam`, `componente-app1/infra/params-dev.bicepparam`.
    *   Define la estructura de estos archivos, utilizando el `using './main.bicep'` (o la ruta al módulo correspondiente) y los parámetros necesarios.
    *   Investiga y planifica la integración con Azure Key Vault para variables sensibles, como se menciona en el documento.

4.  **Diseñar Pipelines Base en Azure DevOps:**
    *   **Pipeline para Infraestructura Compartida (`deploy-shared-infra.yml`):**
        *   Implementa el pipeline YAML como se describe en el documento.
        *   Incluye triggers, parámetros de entorno, y stages para validación (`az deployment sub validate`) y despliegue (`az deployment sub create`).
        *   Configura la conexión de servicio (`serviceConnection`).
    *   **Pipeline para Componentes de Aplicación (`deploy-component.yml`):**
        *   Implementa el pipeline YAML.
        *   Configura la referencia al repositorio `infra-compartida` para consumir los módulos.
        *   Utiliza `New-AzResourceGroupDeployment` (o `az deployment group create` si se prefiere CLI) para desplegar los recursos de la aplicación.

5.  **Implementar Pipeline Piloto (Próximo Paso Inmediato del Documento):**
    *   Selecciona un componente no crítico de la aplicación.
    *   Desarrolla sus plantillas Bicep en su repositorio (`componente-appX/infra/`).
    *   Crea su pipeline de despliegue (`componente-appX/pipelines/deploy-component.yml`) utilizando los módulos compartidos de `infra-compartida`.
    *   Prueba el despliegue de este componente piloto en un entorno de desarrollo.

**Entregables Esperados:**
*   Estructura de directorios para los repositorios `infra-compartida` y `componente-app1`.
*   Módulos Bicep compartidos y parametrizables.
*   Archivos `.bicepparam` para la gestión de configuraciones multientorno.
*   Pipelines YAML base para la infraestructura compartida y los componentes de aplicación.
*   Un pipeline piloto funcional para un componente de aplicación.

**Próximos Pasos (Anticipo Fase 3):**
*   Integración de validaciones de Bicep en los pipelines.
*   Implementación de políticas de Azure.
*   Desarrollo de pruebas de integración para los módulos Bicep.
