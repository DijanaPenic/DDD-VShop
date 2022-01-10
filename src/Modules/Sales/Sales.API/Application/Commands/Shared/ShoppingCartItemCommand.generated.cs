// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Commands/shopping_cart_item_command.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.API.Application.Commands.Shared {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Commands/shopping_cart_item_command.proto</summary>
  public static partial class ShoppingCartItemCommandReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Commands/shopping_cart_item_command.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ShoppingCartItemCommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CkBNb2R1bGVzL1NhbGVzL19zY2hlbWFzL0NvbW1hbmRzL3Nob3BwaW5nX2Nh",
            "cnRfaXRlbV9jb21tYW5kLnByb3RvGjdTaGFyZWRLZXJuZWwvU2hhcmVkS2Vy",
            "bmVsLk1lc3NhZ2luZy9fc2NoZW1hcy91dWlkLnByb3RvGjpTaGFyZWRLZXJu",
            "ZWwvU2hhcmVkS2VybmVsLk1lc3NhZ2luZy9fc2NoZW1hcy9kZWNpbWFsLnBy",
            "b3RvImQKF1Nob3BwaW5nQ2FydEl0ZW1Db21tYW5kEhkKCnByb2R1Y3RfaWQY",
            "ASABKAsyBS5VdWlkEhwKCnVuaXRfcHJpY2UYAiABKAsyCC5EZWNpbWFsEhAK",
            "CHF1YW50aXR5GAMgASgFQjaqAjNWU2hvcC5Nb2R1bGVzLlNhbGVzLkFQSS5B",
            "cHBsaWNhdGlvbi5Db21tYW5kcy5TaGFyZWRiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Messaging.CustomTypes.UuidReflection.Descriptor, global::VShop.SharedKernel.Messaging.CustomTypes.DecimalReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommand), global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommand.Parser, new[]{ "ProductId", "UnitPrice", "Quantity" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ShoppingCartItemCommand : pb::IMessage<ShoppingCartItemCommand>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ShoppingCartItemCommand> _parser = new pb::MessageParser<ShoppingCartItemCommand>(() => new ShoppingCartItemCommand());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ShoppingCartItemCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.API.Application.Commands.Shared.ShoppingCartItemCommandReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartItemCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartItemCommand(ShoppingCartItemCommand other) : this() {
      productId_ = other.productId_ != null ? other.productId_.Clone() : null;
      unitPrice_ = other.unitPrice_ != null ? other.unitPrice_.Clone() : null;
      quantity_ = other.quantity_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartItemCommand Clone() {
      return new ShoppingCartItemCommand(this);
    }

    /// <summary>Field number for the "product_id" field.</summary>
    public const int ProductIdFieldNumber = 1;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Uuid productId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Uuid ProductId {
      get { return productId_; }
      set {
        productId_ = value;
      }
    }

    /// <summary>Field number for the "unit_price" field.</summary>
    public const int UnitPriceFieldNumber = 2;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Decimal unitPrice_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Decimal UnitPrice {
      get { return unitPrice_; }
      set {
        unitPrice_ = value;
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
      return Equals(other as ShoppingCartItemCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ShoppingCartItemCommand other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ProductId, other.ProductId)) return false;
      if (!object.Equals(UnitPrice, other.UnitPrice)) return false;
      if (Quantity != other.Quantity) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (productId_ != null) hash ^= ProductId.GetHashCode();
      if (unitPrice_ != null) hash ^= UnitPrice.GetHashCode();
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
      if (productId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ProductId);
      }
      if (unitPrice_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(UnitPrice);
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
      if (productId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ProductId);
      }
      if (unitPrice_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(UnitPrice);
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
      if (productId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ProductId);
      }
      if (unitPrice_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(UnitPrice);
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
    public void MergeFrom(ShoppingCartItemCommand other) {
      if (other == null) {
        return;
      }
      if (other.productId_ != null) {
        if (productId_ == null) {
          ProductId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        ProductId.MergeFrom(other.ProductId);
      }
      if (other.unitPrice_ != null) {
        if (unitPrice_ == null) {
          UnitPrice = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
        }
        UnitPrice.MergeFrom(other.UnitPrice);
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
            if (productId_ == null) {
              ProductId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(ProductId);
            break;
          }
          case 18: {
            if (unitPrice_ == null) {
              UnitPrice = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
            }
            input.ReadMessage(UnitPrice);
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
            if (productId_ == null) {
              ProductId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(ProductId);
            break;
          }
          case 18: {
            if (unitPrice_ == null) {
              UnitPrice = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
            }
            input.ReadMessage(UnitPrice);
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
