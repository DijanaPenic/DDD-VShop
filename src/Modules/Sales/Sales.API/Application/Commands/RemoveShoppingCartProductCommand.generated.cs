// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Commands/remove_shopping_cart_product_command.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.API.Application.Commands {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Commands/remove_shopping_cart_product_command.proto</summary>
  public static partial class RemoveShoppingCartProductCommandReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Commands/remove_shopping_cart_product_command.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RemoveShoppingCartProductCommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CkpNb2R1bGVzL1NhbGVzL19zY2hlbWFzL0NvbW1hbmRzL3JlbW92ZV9zaG9w",
            "cGluZ19jYXJ0X3Byb2R1Y3RfY29tbWFuZC5wcm90bxo3U2hhcmVkS2VybmVs",
            "L1NoYXJlZEtlcm5lbC5NZXNzYWdpbmcvX3NjaGVtYXMvdXVpZC5wcm90byJw",
            "CiBSZW1vdmVTaG9wcGluZ0NhcnRQcm9kdWN0Q29tbWFuZBIfChBzaG9wcGlu",
            "Z19jYXJ0X2lkGAEgASgLMgUuVXVpZBIZCgpwcm9kdWN0X2lkGAIgASgLMgUu",
            "VXVpZBIQCghxdWFudGl0eRgDIAEoBUIvqgIsVlNob3AuTW9kdWxlcy5TYWxl",
            "cy5BUEkuQXBwbGljYXRpb24uQ29tbWFuZHNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Messaging.CustomTypes.UuidReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.API.Application.Commands.RemoveShoppingCartProductCommand), global::VShop.Modules.Sales.API.Application.Commands.RemoveShoppingCartProductCommand.Parser, new[]{ "ShoppingCartId", "ProductId", "Quantity" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class RemoveShoppingCartProductCommand : pb::IMessage<RemoveShoppingCartProductCommand>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<RemoveShoppingCartProductCommand> _parser = new pb::MessageParser<RemoveShoppingCartProductCommand>(() => new RemoveShoppingCartProductCommand());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<RemoveShoppingCartProductCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.API.Application.Commands.RemoveShoppingCartProductCommandReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public RemoveShoppingCartProductCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public RemoveShoppingCartProductCommand(RemoveShoppingCartProductCommand other) : this() {
      shoppingCartId_ = other.shoppingCartId_ != null ? other.shoppingCartId_.Clone() : null;
      productId_ = other.productId_ != null ? other.productId_.Clone() : null;
      quantity_ = other.quantity_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public RemoveShoppingCartProductCommand Clone() {
      return new RemoveShoppingCartProductCommand(this);
    }

    /// <summary>Field number for the "shopping_cart_id" field.</summary>
    public const int ShoppingCartIdFieldNumber = 1;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Uuid shoppingCartId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Uuid ShoppingCartId {
      get { return shoppingCartId_; }
      set {
        shoppingCartId_ = value;
      }
    }

    /// <summary>Field number for the "product_id" field.</summary>
    public const int ProductIdFieldNumber = 2;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Uuid productId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Uuid ProductId {
      get { return productId_; }
      set {
        productId_ = value;
      }
    }

    /// <summary>Field number for the "quantity" field.</summary>
    public const int QuantityFieldNumber = 3;
    private int quantity_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Quantity {
      get { return quantity_; }
      set {
        quantity_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as RemoveShoppingCartProductCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(RemoveShoppingCartProductCommand other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ShoppingCartId, other.ShoppingCartId)) return false;
      if (!object.Equals(ProductId, other.ProductId)) return false;
      if (Quantity != other.Quantity) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (shoppingCartId_ != null) hash ^= ShoppingCartId.GetHashCode();
      if (productId_ != null) hash ^= ProductId.GetHashCode();
      if (Quantity != 0) hash ^= Quantity.GetHashCode();
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
      if (productId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(ProductId);
      }
      if (Quantity != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Quantity);
      }
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
      if (productId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(ProductId);
      }
      if (Quantity != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Quantity);
      }
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
      if (productId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ProductId);
      }
      if (Quantity != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Quantity);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(RemoveShoppingCartProductCommand other) {
      if (other == null) {
        return;
      }
      if (other.shoppingCartId_ != null) {
        if (shoppingCartId_ == null) {
          ShoppingCartId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        ShoppingCartId.MergeFrom(other.ShoppingCartId);
      }
      if (other.productId_ != null) {
        if (productId_ == null) {
          ProductId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        ProductId.MergeFrom(other.ProductId);
      }
      if (other.Quantity != 0) {
        Quantity = other.Quantity;
      }
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
              ShoppingCartId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(ShoppingCartId);
            break;
          }
          case 18: {
            if (productId_ == null) {
              ProductId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(ProductId);
            break;
          }
          case 24: {
            Quantity = input.ReadInt32();
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
              ShoppingCartId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(ShoppingCartId);
            break;
          }
          case 18: {
            if (productId_ == null) {
              ProductId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(ProductId);
            break;
          }
          case 24: {
            Quantity = input.ReadInt32();
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
