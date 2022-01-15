// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Commands/create_shopping_cart_command.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.API.Application.Commands {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Commands/create_shopping_cart_command.proto</summary>
  public static partial class CreateShoppingCartCommandReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Commands/create_shopping_cart_command.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CreateShoppingCartCommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CkJNb2R1bGVzL1NhbGVzL19zY2hlbWFzL0NvbW1hbmRzL2NyZWF0ZV9zaG9w",
            "cGluZ19jYXJ0X2NvbW1hbmQucHJvdG8aPFNoYXJlZEtlcm5lbC9TaGFyZWRL",
            "ZXJuZWwuSW5mcmFzdHJ1Y3R1cmUvX3NjaGVtYXMvdXVpZC5wcm90bxpATW9k",
            "dWxlcy9TYWxlcy9fc2NoZW1hcy9Db21tYW5kcy9zaG9wcGluZ19jYXJ0X2l0",
            "ZW1fY29tbWFuZC5wcm90byKqAQoZQ3JlYXRlU2hvcHBpbmdDYXJ0Q29tbWFu",
            "ZBIfChBzaG9wcGluZ19jYXJ0X2lkGAEgASgLMgUuVXVpZBIaCgtjdXN0b21l",
            "cl9pZBgCIAEoCzIFLlV1aWQSGQoRY3VzdG9tZXJfZGlzY291bnQYAyABKAUS",
            "NQoTc2hvcHBpbmdfY2FydF9pdGVtcxgEIAMoCzIYLlNob3BwaW5nQ2FydEl0",
            "ZW1Db21tYW5kQi+qAixWU2hvcC5Nb2R1bGVzLlNhbGVzLkFQSS5BcHBsaWNh",
            "dGlvbi5Db21tYW5kc2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Infrastructure.Types.UuidReflection.Descriptor, global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommandReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.API.Application.Commands.CreateShoppingCartCommand), global::VShop.Modules.Sales.API.Application.Commands.CreateShoppingCartCommand.Parser, new[]{ "ShoppingCartId", "CustomerId", "CustomerDiscount", "ShoppingCartItems" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class CreateShoppingCartCommand : pb::IMessage<CreateShoppingCartCommand>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<CreateShoppingCartCommand> _parser = new pb::MessageParser<CreateShoppingCartCommand>(() => new CreateShoppingCartCommand());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CreateShoppingCartCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.API.Application.Commands.CreateShoppingCartCommandReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CreateShoppingCartCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CreateShoppingCartCommand(CreateShoppingCartCommand other) : this() {
      shoppingCartId_ = other.shoppingCartId_ != null ? other.shoppingCartId_.Clone() : null;
      customerId_ = other.customerId_ != null ? other.customerId_.Clone() : null;
      customerDiscount_ = other.customerDiscount_;
      shoppingCartItems_ = other.shoppingCartItems_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CreateShoppingCartCommand Clone() {
      return new CreateShoppingCartCommand(this);
    }

    /// <summary>Field number for the "shopping_cart_id" field.</summary>
    public const int ShoppingCartIdFieldNumber = 1;
    private global::VShop.SharedKernel.Infrastructure.Types.Uuid shoppingCartId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Infrastructure.Types.Uuid ShoppingCartId {
      get { return shoppingCartId_; }
      set {
        shoppingCartId_ = value;
      }
    }

    /// <summary>Field number for the "customer_id" field.</summary>
    public const int CustomerIdFieldNumber = 2;
    private global::VShop.SharedKernel.Infrastructure.Types.Uuid customerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Infrastructure.Types.Uuid CustomerId {
      get { return customerId_; }
      set {
        customerId_ = value;
      }
    }

    /// <summary>Field number for the "customer_discount" field.</summary>
    public const int CustomerDiscountFieldNumber = 3;
    private int customerDiscount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CustomerDiscount {
      get { return customerDiscount_; }
      set {
        customerDiscount_ = value;
      }
    }

    /// <summary>Field number for the "shopping_cart_items" field.</summary>
    public const int ShoppingCartItemsFieldNumber = 4;
    private static readonly pb::FieldCodec<global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommand> _repeated_shoppingCartItems_codec
        = pb::FieldCodec.ForMessage(34, global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommand.Parser);
    private readonly pbc::RepeatedField<global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommand> shoppingCartItems_ = new pbc::RepeatedField<global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommand>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommand> ShoppingCartItems {
      get { return shoppingCartItems_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as CreateShoppingCartCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CreateShoppingCartCommand other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ShoppingCartId, other.ShoppingCartId)) return false;
      if (!object.Equals(CustomerId, other.CustomerId)) return false;
      if (CustomerDiscount != other.CustomerDiscount) return false;
      if(!shoppingCartItems_.Equals(other.shoppingCartItems_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (shoppingCartId_ != null) hash ^= ShoppingCartId.GetHashCode();
      if (customerId_ != null) hash ^= CustomerId.GetHashCode();
      if (CustomerDiscount != 0) hash ^= CustomerDiscount.GetHashCode();
      hash ^= shoppingCartItems_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (shoppingCartId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ShoppingCartId);
      }
      if (customerId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(CustomerId);
      }
      if (CustomerDiscount != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(CustomerDiscount);
      }
      shoppingCartItems_.WriteTo(output, _repeated_shoppingCartItems_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (shoppingCartId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ShoppingCartId);
      }
      if (customerId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(CustomerId);
      }
      if (CustomerDiscount != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(CustomerDiscount);
      }
      shoppingCartItems_.WriteTo(ref output, _repeated_shoppingCartItems_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (shoppingCartId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ShoppingCartId);
      }
      if (customerId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(CustomerId);
      }
      if (CustomerDiscount != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(CustomerDiscount);
      }
      size += shoppingCartItems_.CalculateSize(_repeated_shoppingCartItems_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CreateShoppingCartCommand other) {
      if (other == null) {
        return;
      }
      if (other.shoppingCartId_ != null) {
        if (shoppingCartId_ == null) {
          ShoppingCartId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
        }
        ShoppingCartId.MergeFrom(other.ShoppingCartId);
      }
      if (other.customerId_ != null) {
        if (customerId_ == null) {
          CustomerId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
        }
        CustomerId.MergeFrom(other.CustomerId);
      }
      if (other.CustomerDiscount != 0) {
        CustomerDiscount = other.CustomerDiscount;
      }
      shoppingCartItems_.Add(other.shoppingCartItems_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (shoppingCartId_ == null) {
              ShoppingCartId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(ShoppingCartId);
            break;
          }
          case 18: {
            if (customerId_ == null) {
              CustomerId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(CustomerId);
            break;
          }
          case 24: {
            CustomerDiscount = input.ReadInt32();
            break;
          }
          case 34: {
            shoppingCartItems_.AddEntriesFrom(input, _repeated_shoppingCartItems_codec);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (shoppingCartId_ == null) {
              ShoppingCartId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(ShoppingCartId);
            break;
          }
          case 18: {
            if (customerId_ == null) {
              CustomerId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(CustomerId);
            break;
          }
          case 24: {
            CustomerDiscount = input.ReadInt32();
            break;
          }
          case 34: {
            shoppingCartItems_.AddEntriesFrom(ref input, _repeated_shoppingCartItems_codec);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
