protoc --proto_path=. --csharp_out=internal_access:Modules/Sales/Modules.Sales.Infrastructure --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.Infrastructure Modules/Sales/_schemas/Commands/*.proto
protoc --proto_path=. --csharp_out=internal_access:Modules/Billing/Modules.Billing.Infrastructure --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Billing.Infrastructure Modules/Billing/_schemas/Commands/*.proto
protoc --proto_path=. --csharp_out=internal_access:Modules/Catalog/Modules.Catalog.Infrastructure --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Catalog.Infrastructure Modules/Catalog/_schemas/Commands/*.proto


protoc --proto_path=. --csharp_out=Modules/Sales/Modules.Sales.Integration --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.Integration Modules/Sales/_schemas/Events/IntegrationEvents/*.proto
protoc --proto_path=. --csharp_out=Modules/Billing/Modules.Billing.Integration --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Billing.Integration Modules/Billing/_schemas/Events/IntegrationEvents/*.proto
protoc --proto_path=. --csharp_out=Modules/Catalog/Modules.Catalog.Integration --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Catalog.Integration Modules/Catalog/_schemas/Events/IntegrationEvents/*.proto
protoc --proto_path=. --csharp_out=Modules/Identity/Modules.Identity.Integration --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Identity.Integration Modules/Identity/_schemas/Events/IntegrationEvents/*.proto

protoc --proto_path=. --csharp_out=internal_access:Modules/Sales/Modules.Sales.Domain --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.Domain Modules/Sales/_schemas/Events/DomainEvents/*.proto


protoc --proto_path=. --csharp_out=SharedKernel/SharedKernel.EventStoreDb --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.SharedKernel.EventStoreDb SharedKernel/SharedKernel.EventStoreDb/_schemas/*.proto
protoc --proto_path=. --csharp_out=SharedKernel/SharedKernel.Infrastructure --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.SharedKernel.Infrastructure SharedKernel/SharedKernel.Infrastructure/_schemas/*.proto


protoc --proto_path=. --csharp_out=internal_access:Modules/ProcessManager/Modules.ProcessManager.API --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.ProcessManager.API Modules/ProcessManager/_schemas/*.proto
