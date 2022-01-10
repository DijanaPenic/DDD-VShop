// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// using VShop.SharedKernel.Domain.Enums;
// using VShop.SharedKernel.Domain.ValueObjects;
// using VShop.SharedKernel.Infrastructure;
// using VShop.SharedKernel.Messaging.Commands;
// using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
// using VShop.SharedKernel.EventSourcing.Stores.Contracts;
// using VShop.Modules.Sales.Domain.Models.ShoppingCart;
//
// namespace VShop.Modules.Sales.API.Application.Commands
// {
//     public class SetContactInformationCommandHandler : ICommandHandler<SetContactInformationCommand>
//     {
//         private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
//
//         public SetContactInformationCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
//             => _shoppingCartStore = shoppingCartStore;
//         
//         public async Task<Result> Handle(SetContactInformationCommand command, CancellationToken cancellationToken)
//         {
//             ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
//             (
//                 EntityId.Create(command.ShoppingCartId).Data,
//                 command.MessageId,
//                 command.CorrelationId,
//                 cancellationToken
//             );
//             if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
//
//             if (shoppingCart.Events.Count is 0)
//             {
//                 Result<FullName> fullNameResult = FullName.Create(command.FirstName, command.MiddleName, command.LastName);
//                 if (fullNameResult.IsError) return fullNameResult.Error;
//             
//                 Result setContactInformationResult = shoppingCart.Customer.SetContactInformation
//                 (
//                     fullNameResult.Data,
//                     EmailAddress.Create(command.EmailAddress).Data,
//                     PhoneNumber.Create(command.PhoneNumber).Data,
//                     command.Gender
//                 );
//                 if (setContactInformationResult.IsError) return setContactInformationResult.Error;
//             }
//
//             await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);
//
//             return Result.Success;
//         }
//     }
//     
//     public record SetContactInformationCommand : Command
//     {
//         public Guid ShoppingCartId { get; init; }
//         public string FirstName { get; init; }
//         public string MiddleName { get; init; }
//         public string LastName { get; init; }
//         public string EmailAddress { get; init; }
//         public string PhoneNumber { get; init; }
//         public GenderType Gender { get; init; }
//         
//         public SetContactInformationCommand() { }
//
//         public SetContactInformationCommand
//         (
//             Guid shoppingCartId,
//             string firstName,
//             string middleName,
//             string lastName,
//             string emailAddress,
//             string phoneNumber,
//             GenderType gender
//         )
//         {
//             ShoppingCartId = shoppingCartId;
//             FirstName = firstName;
//             MiddleName = middleName;
//             LastName = lastName;
//             EmailAddress = emailAddress;
//             PhoneNumber = phoneNumber;
//             Gender = gender;
//         }
//     }
// }