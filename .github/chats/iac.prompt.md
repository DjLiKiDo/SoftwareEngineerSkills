acabo de entrar en un nuevo equipo de desarrollo. 
Tienen toda la infraestructura del producto en Azure, distribuido en diferentes subscripciones, resource groups y demas, por componentes de la aplicacion y por entorno.
Todo lo han creado y configurado de manera manual

Mi mision en el equipo es la de implementar IaC (infrastructure as code)
- Generar el codigo necesario y una pipeline para desplegar la infraestructura completa, y la configuracion en una pipeline general, de elementos compartidos o independientes de los componentes de la aplicacion
- Generar el codigo necesario y una pipeline para desplegar la infraestructura y configuracion requerida por cada uno de los componentes, incluida en el repositorio de cada componente
- Que una vez este implementado, sea capaz de restaurar la infraestructura actual que ya esta configurada en Azure en caso de desastre o error humano
- Que una vez que este implementado, los futuros cambios a realizar en la infra se realicen a traves de codigo, y se desplieguen en su pipeline correspondiente

Tenemos 3 entornos
- Development
- Staging
- Production

Necesito un informe de las opciones que tengo a nivel de stack para llevar a cabo la implementacion con exito

- Toda la infraestructura esta en azure
- Las pipelines son de Azure Devops