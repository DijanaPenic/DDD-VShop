protoc --proto_path=. --csharp_out=Modules/Sales/Sales.Domain --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.Domain Modules/Sales/_schemas/Events/*.proto

protoc --proto_path=. --csharp_out=Modules/Sales/Sales.Domain --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.Domain Modules/Sales/_schemas/Events/*.proto