<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" class="logo" width="120"/>

# Informe de Opciones para Implementación de Infrastructure as Code (IaC) en Azure

La adopción de Infrastructure as Code (IaC) representa un paso fundamental para modernizar la gestión de infraestructura en un entorno Azure, permitiendo tratar los recursos de infraestructura como componentes de software sujetos a las mismas prácticas de desarrollo, versionado y automatización. Este informe analiza las principales opciones disponibles para implementar una solución IaC efectiva adaptada a su caso específico.

## Conceptos Fundamentales de IaC

La Infraestructura como Código (IaC) permite definir y gestionar recursos de infraestructura mediante archivos de configuración versionables, eliminando la configuración manual y los procesos propensos a errores. En el contexto de Azure, esto significa transformar la gestión de recursos desde el portal hacia un enfoque basado en código que ofrezca reproducibilidad, escalabilidad y control de versiones.

### Beneficios Clave para su Escenario

La implementación de IaC para su infraestructura existente en Azure proporcionará:

- Documentación viva de toda la infraestructura actual distribuida en múltiples suscripciones
- Capacidad de recuperación rápida ante desastres mediante despliegues automatizados
- Consistencia entre los tres entornos (Development, Staging y Production)
- Integración del ciclo de vida de la infraestructura con el desarrollo de aplicaciones
- Posibilidad de realizar pruebas de infraestructura antes de aplicar cambios en entornos productivos


## Opciones Tecnológicas Principales

### Azure Resource Manager (ARM) y Bicep

Bicep es un lenguaje declarativo desarrollado por Microsoft específicamente para definir la infraestructura de Azure. Funciona como una abstracción sobre las plantillas JSON de ARM, ofreciendo una sintaxis más sencilla y legible.

#### Ventajas:

- Integración nativa con el ecosistema Azure
- No requiere gestión de estado separada, ya que Azure Resource Manager mantiene el estado directamente[^2]
- Excelente soporte en Visual Studio Code y Azure CLI
- Compatibilidad total con plantillas ARM existentes (se puede compilar Bicep a plantillas ARM)[^2]
- Actualizaciones rápidas para nuevas características de Azure
- Integración perfecta con las pipelines de Azure DevOps[^7]


#### Limitaciones:

- Específico para Azure, sin soporte multicloud[^2]
- Ecosistema más pequeño y más reciente en comparación con alternativas[^2]


### Terraform

Terraform es una herramienta de IaC de código abierto desarrollada por HashiCorp que permite definir y provisionar infraestructura en múltiples proveedores de nube.

#### Ventajas:

- Soporte multicloud, ideal si se prevé expansión fuera de Azure en el futuro[^2]
- Gran ecosistema con fuerte soporte comunitario y amplia biblioteca de módulos reutilizables[^2]
- Flexibilidad para gestionar recursos que están fuera del alcance de ARM/Bicep
- Buena integración con Azure DevOps mediante extensiones específicas[^4][^6]
- Herramientas empresariales como Terraform Cloud para colaboración y gobernanza[^2]


#### Limitaciones:

- Requiere gestión explícita del estado mediante archivos de estado externos[^2]
- Mayor complejidad inicial para configurar el almacenamiento seguro del estado
- No se integra tan estrechamente con las características específicas de Azure
- Posibles riesgos al utilizarlo específicamente para Azure (según se menciona en una de las fuentes)[^2]


### Pulumi

Pulumi permite definir infraestructura utilizando lenguajes de programación completos como TypeScript, Python, Go, C\# y Java.

#### Ventajas:

- Flexibilidad para utilizar lenguajes de programación familiares[^3]
- Soporte para múltiples proveedores de nube, incluido Azure
- Buena integración con Azure y capacidad para gestionar recursos de contenedores y funciones sin servidor[^3]
- Potencialmente más flexible para lógica compleja de infraestructura


#### Limitaciones:

- Menor adopción en comparación con Terraform o las herramientas nativas de Azure
- Curva de aprendizaje potencialmente mayor
- Integración menos documentada con Azure DevOps


## Comparación de Enfoques para su Caso Específico

### Exportación de Recursos Existentes

Dado que su infraestructura ya existe en Azure, necesitará exportar la configuración actual:

- **Con Azure/Bicep**: Azure Portal permite exportar recursos existentes como plantillas ARM o Bicep[^8]. Esta exportación captura el estado actual de los recursos y puede servir como punto de partida para sus definiciones de IaC[^8].
- **Con Terraform**: Terraform ofrece herramientas como `terraform import` para importar recursos existentes al estado de Terraform, aunque requiere definir primero los recursos en código.
- **Con Pulumi**: Permite importar recursos existentes de manera similar a Terraform, pero usando el lenguaje de programación seleccionado.


### Estructura de Organización del Código

Para satisfacer su necesidad de tener infraestructura compartida e infraestructura específica por componente:

**Propuesta de estructura con Bicep:**

```
/infrastructure-repo
  /shared-resources
    /networking
    /security
    /monitoring
    main.bicep
    parameters-dev.json
    parameters-staging.json
    parameters-prod.json
  /pipelines
    shared-infra-pipeline.yml

/component-repo-1
  /src
  /infrastructure
    component-specific.bicep
    parameters-dev.json
    parameters-staging.json
    parameters-prod.json
  /pipelines
    component-infra-pipeline.yml
```

**Estructura similar para Terraform o Pulumi**, con sus respectivas extensiones y particularidades.

### Integración con Azure DevOps

Todos los enfoques pueden integrarse bien con Azure DevOps, pero con algunas diferencias:

- **Bicep**: Integración directa con Azure Pipelines, con tareas específicas disponibles[^7]. Ejemplo de configuración de pipeline:

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: AzureCLI@2
  inputs:
    azureSubscription: 'Mi-Servicio-Conexión'
    scriptType: 'bash'
    scriptLocation: 'inlineScript'
    inlineScript: |
      az deployment group create \
        --resource-group myResourceGroup \
        --template-file main.bicep \
        --parameters parameters-$(Environment).json
```

- **Terraform**: Requiere configurar el almacenamiento del estado (típicamente en Azure Storage) y adicionalmente instalar las extensiones apropiadas en la pipeline[^4][^6].

```yaml
steps:
- task: TerraformInstaller@0
  inputs:
    terraformVersion: 'latest'
- task: TerraformTaskV4@4
  inputs:
    provider: 'azurerm'
    command: 'init'
    backendServiceArm: 'Mi-Servicio-Conexión'
    backendAzureRmResourceGroupName: 'terraform-state-rg'
    backendAzureRmStorageAccountName: 'terraformstate'
    backendAzureRmContainerName: 'tfstate'
    backendAzureRmKey: 'infrastructure.tfstate'
```

- **Pulumi**: Ofrece integración a través de comandos CLI en las pipelines de Azure DevOps, con configuración adicional para el estado.


## Estrategia de Implementación Recomendada

### Fase 1: Definición y Extracción de la Infraestructura Actual

1. **Inventario completo**: Documentar todas las suscripciones, grupos de recursos y recursos actualmente utilizados.
2. **Exportar recursos existentes**: Utilizar las herramientas de exportación del portal de Azure para obtener plantillas ARM/Bicep de la infraestructura actual[^8].
3. **Organizar recursos**: Clasificar los recursos en componentes compartidos y específicos de aplicación.

### Fase 2: Configuración del Repositorio y Estructura

1. **Crear repositorio para infraestructura compartida**: Establecer la estructura de carpetas para separar lógicamente los componentes.
2. **Establecer estructura en repositorios de componentes**: Definir la estructura dentro de cada repositorio de componentes de aplicación.
3. **Definir estrategia de parametrización**: Configurar archivos de parámetros separados para cada entorno (Development, Staging, Production).

### Fase 3: Implementación Progresiva

1. **Comenzar con un subconjunto crítico**: Implementar IaC primero para un conjunto limitado pero importante de recursos.
2. **Validación en Development**: Probar la capacidad de recrear exactamente la infraestructura en el entorno de desarrollo.
3. **Ampliar progresivamente**: Expandir la implementación a más recursos y finalmente a todos los entornos.

### Fase 4: Automatización con Pipelines

1. **Crear pipeline para recursos compartidos**: Implementar la pipeline principal que despliega la infraestructura común.
2. **Crear pipelines de componentes**: Implementar las pipelines específicas para cada componente de aplicación.
3. **Configurar seguridad y aprobaciones**: Establecer políticas de aprobación, especialmente para cambios en entornos productivos.

## Recomendación Final

Basándome en los requisitos específicos y en que toda la infraestructura está en Azure utilizando Azure DevOps, recomiendo:

### Opción Primaria: Bicep

Bicep ofrece la mejor combinación de facilidad de uso, integración nativa con Azure y soporte para su caso específico por las siguientes razones:

1. **Exportación directa**: Puede exportar fácilmente sus recursos actuales a Bicep o ARM y luego convertirlo a Bicep puro[^8].
2. **Integración nativa con Azure DevOps**: Bicep cuenta con soporte completo en Azure Pipelines[^7].
3. **Sin gestión de estado adicional**: Aprovecha el estado almacenado directamente en Azure Resource Manager[^2].
4. **Actualizaciones rápidas**: Acceso inmediato a nuevas características de Azure.

### Alternativa: Terraform

Si se prevé la posibilidad de expandirse a otras nubes en el futuro o si el equipo ya tiene experiencia con Terraform, esta sería una alternativa viable:

1. **Portabilidad multicloud**: Ofrece flexibilidad para extenderse más allá de Azure en el futuro[^2].
2. **Ecosistema maduro**: Amplia comunidad y recursos disponibles[^2].
3. **Integración buena pero no nativa**: Requiere configuración adicional para integrar con Azure DevOps[^4][^6].

## Conclusión

La implementación de IaC en su entorno Azure existente es un proyecto ambicioso pero alcanzable. Bicep destaca como la solución más adecuada debido a su integración nativa con Azure y su facilidad para recuperar el estado actual de su infraestructura. La clave del éxito será una implementación gradual, comenzando por exportar y codificar los recursos existentes, seguido por la creación de pipelines automatizadas que puedan reconstruir confiablemente su infraestructura en caso necesario.

Con una estrategia bien planificada, podrá transformar su actual configuración manual en un sistema robusto basado en código, mejorando la resiliencia, la trazabilidad y la eficiencia de su infraestructura de Azure.

<div style="text-align: center">⁂</div>

[^1]: https://learn.microsoft.com/es-es/devops/deliver/what-is-infrastructure-as-code

[^2]: https://dev.to/spacelift/bicep-vs-terraform-46on

[^3]: https://www.pulumi.com/docs/iac/get-started/azure/

[^4]: https://www.linkedin.com/pulse/integrating-infrastructure-code-iac-tools-terraform-ansible-djokic-okkwf

[^5]: https://learn.microsoft.com/es-es/azure/azure-resource-manager/templates/overview

[^6]: https://learn.microsoft.com/es-es/azure/developer/terraform/best-practices-integration-testing

[^7]: https://learn.microsoft.com/en-us/training/modules/build-first-bicep-deployment-pipeline-using-azure-pipelines/

[^8]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/export-template-portal

[^9]: https://techcommunity.microsoft.com/discussions/microsoftlearn/importing-terraform-state-in-azure/3421289

[^10]: https://www.pulumi.com/docs/iac/adopting-pulumi/migrating-to-pulumi/from-arm/

[^11]: https://transformaciondigital.elantia.es/automatizacion-de-infraestructuras-con-azure-devops/

[^12]: https://www.pulumi.com/docs/iac/using-pulumi/continuous-delivery/azure-devops/

[^13]: https://insight-services-apac.github.io/2022/07/13/terraform-multienvironment

[^14]: https://github.com/sree7k7/multiple-environments-by-using-Bicep-pipelines

[^15]: https://learn.microsoft.com/es-es/azure/cloud-adoption-framework/ready/considerations/infrastructure-as-code

[^16]: https://learn.microsoft.com/es-es/azure/azure-resource-manager/management/overview

[^17]: https://spacelift.io/blog/terraform-azure-devops

[^18]: https://learn.microsoft.com/es-es/devops/deliver/iac-github-actions

[^19]: https://learn.microsoft.com/es-es/azure/architecture/solution-ideas/articles/devsecops-infrastructure-as-code

[^20]: https://apiumhub.com/es/tech-blog-barcelona/comparacion-de-herramientas-iac-para-azure-terraform-y-bicep/

[^21]: https://es.linkedin.com/pulse/automatización-eficiente-en-azure-devops-para-de-como-luis-josé-gkdre

[^22]: https://es.linkedin.com/pulse/terraform-vs-azure-bicep-cuál-es-la-mejor-opción-para-soleto-olguin

[^23]: https://www.bravent.net/noticias/como-usar-plantillas-arm-para-crear-el-despliegue-en-azure/

[^24]: https://dev.to/isaacojeda/infrastructure-as-code-con-bicep-3g2p

[^25]: https://www.carbonlogiq.io/post/azure-devops-pipeline-terraform-deployment-tutorial

[^26]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/add-template-to-azure-pipelines

[^27]: https://github.com/MicrosoftDocs/azure-docs/blob/main/articles/azure-resource-manager/templates/export-template-portal.md

[^28]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/decompile

[^29]: https://azure.github.io/IoTTrainingPack/modules/DevOps/iac.html

[^30]: https://learn.microsoft.com/en-us/azure/devops/pipelines/process/create-multistage-pipeline?view=azure-devops

[^31]: https://www.techtarget.com/searchitoperations/tutorial/Use-Pulumi-and-Azure-DevOps-to-deploy-infrastructure-as-code

[^32]: https://dzone.com/articles/run-anisble-playbook-from-azure-devops-release-pip

[^33]: https://learn.microsoft.com/en-us/training/modules/manage-multiple-environments-using-bicep-azure-pipelines/

[^34]: https://spot.io/resources/azure-automation/infrastructure-as-code-iac-in-azure-a-practical-guide/

[^35]: https://www.cosmoconsult.com/cl/insights/blog/que-es-azure-resource-manager

[^36]: https://learn.microsoft.com/es-es/azure/azure-resource-manager/templates/frequently-asked-questions

[^37]: https://es.linkedin.com/pulse/ventajas-de-azure-resource-manager-ademir-soleto-olguin

[^38]: https://cloudriders.es/terraform-vs-biceps-que-es-mejor-para-azure

[^39]: https://www.reddit.com/r/AZURE/comments/10967am/which_iac_tool_to_chose_for_azure_ressources/?tl=es-es

[^40]: https://www.azuredevopslabs.com/labs/vstsextend/terraform/

[^41]: https://www.youtube.com/watch?v=09XyJPLls6k

[^42]: https://www.techielass.com/terraform-with-azure-devops-ci-cd-pipeline/

[^43]: https://www.reddit.com/r/azuredevops/comments/lhhzlg/how_to_deploy_infrastructure_arm_templates_using/

[^44]: https://www.reddit.com/r/AZURE/comments/1eom33d/exporting_resources_to_arm_templates/

[^45]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/template-tutorial-export-template

[^46]: https://www.youtube.com/watch?v=M0v9s6oKO5c

[^47]: https://stackoverflow.com/questions/61638094/how-do-i-export-an-arm-template-correctly-from-azure

[^48]: https://github.com/MicrosoftDocs/azure-docs/blob/main/articles/azure-resource-manager/bicep/bicep-cli.md

[^49]: https://learn.microsoft.com/es-es/devops/deliver/what-is-infrastructure-as-code

[^50]: https://learn.microsoft.com/es-es/devops/deliver/iac-github-actions

[^51]: https://learn.microsoft.com/es-es/azure/cloud-adoption-framework/ready/considerations/infrastructure-as-code

[^52]: https://github.com/JFolberth/Azure_IaC_Flavors

[^53]: https://www.devopswithritesh.in/improving-infrastructure-as-code-iac-using-devops-and-cicd-multi-environment-deployment

[^54]: https://azure.microsoft.com/es-es/products/deployment-environments

[^55]: https://www.pulumi.com/registry/packages/azuredevops/

[^56]: https://github.com/pulumi/pulumi-azuredevops

[^57]: https://www.pulumi.com/registry/packages/azuredevops/api-docs/

[^58]: https://www.pulumi.com/registry/packages/azuredevops/api-docs/serviceendpointpipeline/

[^59]: https://azure.kocsistem.com.tr/en/blog/ansible-on-azure-devops-pipelines

[^60]: https://marketplace.visualstudio.com/items?itemName=pulumi.build-and-release-task

[^61]: https://www.youtube.com/watch?v=xx299QNUSvc

[^62]: https://www.youtube.com/watch?v=ieloWncWCMU

[^63]: https://dev.to/pwd9000/multi-environment-azure-deployments-with-terraform-and-github-2450

[^64]: https://www.linkedin.com/pulse/creating-multi-environment-pipeline-terraform-using-bhattacharjee-uqxqc

[^65]: https://discuss.hashicorp.com/t/terraform-multiple-environments-best-practices/63527

[^66]: https://www.applytosupply.digitalmarketplace.service.gov.uk/g-cloud/services/963002103062903

[^67]: https://zeet.co/blog/terraform-multiple-environments

