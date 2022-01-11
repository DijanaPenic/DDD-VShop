protoc --proto_path=. --csharp_out=. --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.SharedKernel.Messaging ./_schema/*.proto







protoc --proto_path=. --csharp_out=. --csharp_opt=file_extension=.generated.cs,base_namespace=VShop.Modules.Sales.API ./_schema/*.proto