<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" class="logo" width="120"/>

# Implementación de IaC con Bicep en Entorno Azure Multientorno

La elección de Bicep como herramienta central para su estrategia de Infrastructure as Code (IaC) permite aprovechar al máximo las capacidades nativas de Azure y simplificar la gestión de recursos distribuidos en múltiples suscripciones y entornos. Este análisis detalla el plan de acción para lograr una implementación exitosa.

## Arquitectura Target con Bicep

### Estructura de Repositorios

```
├── infra-compartida (Repositorio Central)
│   ├── networking/
│   │   ├── virtual-network.bicep
│   │   └── peering.bicep
│   ├── security/
│   │   ├── key-vaults.bicep
│   │   └── policies.bicep
│   ├── pipelines/
│   │   └── deploy-shared-infra.yml
│   └── parameters/
│       ├── dev/
│       │   └── shared-params.bicepparam
│       ├── staging/
│       └── prod/

└── componente-app1 (Repositorio por Aplicación)
    ├── src/
    ├── infra/
    │   ├── app-service.bicep
    │   └── sql-database.bicep
    └── pipelines/
        └── deploy-component.yml
```


## Fases Críticas de Implementación

### 1. Exportación de Infraestructura Existente

**Herramientas clave:**

- Azure Portal: Exportación directa a plantillas ARM/Bicep
- CLI de Azure: `az bicep decompile` para convertir ARM a Bicep[^1][^5]

**Proceso recomendado:**

```bash
# Exportar grupo de recursos existente
az group export --name MyResourceGroup > exported-template.json

# Convertir a Bicep
az bicep decompile --file exported-template.json
```


### 2. Organización de Parámetros Multientorno

**Estrategia de parametrización:**

- Uso de archivos `.bicepparam` con scope por entorno[^4][^6]
- Variables sensibles almacenadas en Azure Key Vault con integración directa

**Ejemplo `shared-params.bicepparam`:**

```bicep
using './main.bicep'

param environment = 'dev'
param location = 'westeurope'
param skuTier = 'Standard'
```


### 3. Diseño de Pipelines en Azure DevOps

**Pipeline para Infraestructura Compartida:**

```yaml
# deploy-shared-infra.yml
trigger:
  branches:
    include: [main]

parameters:
  - name: environment
    type: string
    values: ['dev', 'staging', 'prod']

variables:
  serviceConnection: 'azure-shared-conn'
  location: 'westeurope'

stages:
- stage: DeployShared
  jobs:
  - job: Validate
    steps:
    - task: AzureCLI@2
      inputs:
        azureSubscription: ${{ variables.serviceConnection }}
        scriptType: 'bash'
        script: |
          az deployment sub validate \
            --location $(location) \
            --template-file ./main.bicep \
            --parameters ./parameters/${{ parameters.environment }}/shared-params.bicepparam

  - job: Deploy
    dependsOn: Validate
    steps:
    - task: AzureCLI@2
      inputs:
        azureSubscription: ${{ variables.serviceConnection }}
        scriptType: 'bash'
        script: |
          az deployment sub create \
            --location $(location) \
            --template-file ./main.bicep \
            --parameters ./parameters/${{ parameters.environment }}/shared-params.bicepparam
```

**Pipeline para Componentes de Aplicación:**

```yaml
# deploy-component.yml
resources:
  repositories:
  - repository: shared_templates
    type: git
    name: infra-compartida
    ref: main

steps:
- checkout: self
- checkout: shared_templates

- task: AzurePowerShell@5
  inputs:
    azureSubscription: 'component-conn'
    scriptType: 'inlineScript'
    inline: |
      New-AzResourceGroupDeployment `
        -TemplateFile ./infra/app-service.bicep `
        -TemplateParameterFile ./infra/params-${{ parameters.environment }}.bicepparam `
        -ResourceGroupName "rg-app1-${{ parameters.environment }}"
```


## Gestión de Estados y Recuperación

### Estrategia de Backup Automatizado

1. **Versionado de Plantillas:**
    - Commit diario de plantillas compiladas (ARM JSON) en artefactos de build
2. **Azure Policy:**
    - Política de bloqueo para prevenir cambios manuales

```bicep
resource denyResourceChanges 'Microsoft.Authorization/policyAssignments@2022-06-01' = {
  name: 'deny-resource-changes'
  properties: {
    policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/8e3e61b3-0b32-22d5-4edf-55fca661d4d2'
    parameters: {
      effect: { value: 'Deny' }
    }
  }
}
```

3. **Recuperación ante Desastres:**
    - Pipeline de emergencia con trigger manual
    - Restauración desde últimos artefactos aprobados

## Mejores Prácticas para el Equipo

### Flujo de Trabajo de Desarrollo

1. **Bicep CLI en CI/CD:**
    - Validación automática en PRs

```yaml
- script: az bicep build --file infrastructure/main.bicep
  displayName: 'Compile Bicep'
```

2. **Testing de Módulos:**
    - Uso de Pester para pruebas de integración
3. **Documentación Automatizada:**
    - Generación de diagramas ARM con `az bicep generate-params`

### Seguridad y Gobernanza

- **RBAC en Pipelines:** Cuentas de servicio con privilegios mínimos
- **Escaneo de Secretos:** Integración con GitGuardian en repositorios
- **Auditoría:** Logs de deployment almacenados en Log Analytics


## Roadmap de Implementación

| Fase | Duración | Entregables Clave |
| :-- | :-- | :-- |
| Onboarding | 2 semanas | Exportación completa de recursos existentes |
| Core | 4 semanas | Pipelines base y módulos compartidos |
| Optimización | 6 semanas | Políticas de seguridad y autom. testing |
| Madurez | 8 semanas | Sistema completo de DR y monitoreo |

**Próximos Pasos Inmediatos:**

1. Configurar entorno de desarrollo con:
    - Azure CLI 2.47.0+
    - Bicep Extension en VS Code[^5][^7]
2. Ejecutar taller práctico de 2 días para el equipo
3. Implementar pipeline piloto para un componente no crítico

Esta estrategia asegura una transición controlada desde la infraestructura manual hacia un modelo completamente automatizado, manteniendo la capacidad de recuperación y cumpliendo con los requisitos multientorno. La integración nativa de Bicep con Azure DevOps[^4][^6][^9] y su capacidad para gestionar estados sin componentes externos[^1][^5] lo convierten en la opción óptima para su escenario actual.

<div style="text-align: center">⁂</div>

[^1]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/

[^2]: https://learn.microsoft.com/es-es/azure/azure-resource-manager/bicep/deploy-cli

[^3]: https://www.varonis.com/blog/azure-bicep

[^4]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/add-template-to-azure-pipelines

[^5]: https://docs.azure.cn/en-us/azure-resource-manager/templates/bicep-tutorial-create-first-bicep

[^6]: https://learn.microsoft.com/en-us/training/modules/build-first-bicep-deployment-pipeline-using-azure-pipelines/

[^7]: https://learn.microsoft.com/en-us/training/modules/implement-bicep/

[^8]: https://www.youtube.com/watch?v=Q2HZdwTAWG0

[^9]: https://rios.engineer/getting-started-continuous-deployment-with-azure-bicep-and-azure-devops/

