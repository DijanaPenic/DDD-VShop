protoc --proto_path=. --csharp_out=Modules/Sales/Sales.API --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.API Modules/Sales/_schemas/Commands/*.proto
protoc --proto_path=. --csharp_out=Modules/Billing/Billing.API --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Billing.API Modules/Billing/_schemas/Commands/*.proto
//protoc --proto_path=. --csharp_out=Modules/Catalog/Catalog.API --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Catalog.API Modules/Catalog/_schemas/Commands/*.proto


protoc --proto_path=. --csharp_out=Modules/Sales/Sales.Integration --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.Integration Modules/Sales/_schemas/Events/IntegrationEvents/*.proto
protoc --proto_path=. --csharp_out=Modules/Billing/Billing.Integration --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Billing.Integration Modules/Billing/_schemas/Events/IntegrationEvents/*.proto
protoc --proto_path=. --csharp_out=Modules/Catalog/Catalog.Integration --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Catalog.Integration Modules/Catalog/_schemas/Events/IntegrationEvents/*.proto


protoc --proto_path=. --csharp_out=Modules/Sales/Sales.Domain --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.Domain Modules/Sales/_schemas/Events/DomainEvents/*.proto


protoc --proto_path=. --csharp_out=SharedKernel/SharedKernel.Messaging --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.SharedKernel.Messaging SharedKernel/SharedKernel.Messaging/_schemas/*.proto
protoc --proto_path=. --csharp_out=SharedKernel/SharedKernel.Infrastructure --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.SharedKernel.Infrastructure SharedKernel/SharedKernel.Infrastructure/_schemas/*.proto