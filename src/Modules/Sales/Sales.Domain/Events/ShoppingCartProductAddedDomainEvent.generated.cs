// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_product_added_domain_event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.Domain.Events {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_product_added_domain_event.proto</summary>
  public static partial class ShoppingCartProductAddedDomainEventReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_product_added_domain_event.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ShoppingCartProductAddedDomainEventReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CllNb2R1bGVzL1NhbGVzL19zY2hlbWFzL0V2ZW50cy9Eb21haW5FdmVudHMv",
            "c2hvcHBpbmdfY2FydF9wcm9kdWN0X2FkZGVkX2RvbWFpbl9ldmVudC5wcm90",
            "bxo3U2hhcmVkS2VybmVsL1NoYXJlZEtlcm5lbC5NZXNzYWdpbmcvX3NjaGVt",
            "YXMvdXVpZC5wcm90bxo6U2hhcmVkS2VybmVsL1NoYXJlZEtlcm5lbC5NZXNz",
            "YWdpbmcvX3NjaGVtYXMvZGVjaW1hbC5wcm90byKRAQojU2hvcHBpbmdDYXJ0",
            "UHJvZHVjdEFkZGVkRG9tYWluRXZlbnQSHwoQc2hvcHBpbmdfY2FydF9pZBgB",
            "IAEoCzIFLlV1aWQSGQoKcHJvZHVjdF9pZBgCIAEoCzIFLlV1aWQSEAoIcXVh",
            "bnRpdHkYAyABKAUSHAoKdW5pdF9wcmljZRgEIAEoCzIILkRlY2ltYWxCJKoC",
            "IVZTaG9wLk1vZHVsZXMuU2FsZXMuRG9tYWluLkV2ZW50c2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Messaging.CustomTypes.UuidReflection.Descriptor, global::VShop.SharedKernel.Messaging.CustomTypes.DecimalReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.Domain.Events.ShoppingCartProductAddedDomainEvent), global::VShop.Modules.Sales.Domain.Events.ShoppingCartProductAddedDomainEvent.Parser, new[]{ "ShoppingCartId", "ProductId", "Quantity", "UnitPrice" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ShoppingCartProductAddedDomainEvent : pb::IMessage<ShoppingCartProductAddedDomainEvent>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ShoppingCartProductAddedDomainEvent> _parser = new pb::MessageParser<ShoppingCartProductAddedDomainEvent>(() => new ShoppingCartProductAddedDomainEvent());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ShoppingCartProductAddedDomainEvent> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.Domain.Events.ShoppingCartProductAddedDomainEventReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartProductAddedDomainEvent() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartProductAddedDomainEvent(ShoppingCartProductAddedDomainEvent other) : this() {
      shoppingCartId_ = other.shoppingCartId_ != null ? other.shoppingCartId_.Clone() : null;
      productId_ = other.productId_ != null ? other.productId_.Clone() : null;
      quantity_ = other.quantity_;
      unitPrice_ = other.unitPrice_ != null ? other.unitPrice_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartProductAddedDomainEvent Clone() {
      return new ShoppingCartProductAddedDomainEvent(this);
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

    /// <summary>Field number for the "unit_price" field.</summary>
    public const int UnitPriceFieldNumber = 4;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Decimal unitPrice_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Decimal UnitPrice {
      get { return unitPrice_; }
      set {
        unitPrice_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ShoppingCartProductAddedDomainEvent);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ShoppingCartProductAddedDomainEvent other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ShoppingCartId, other.ShoppingCartId)) return false;
      if (!object.Equals(ProductId, other.ProductId)) return false;
      if (Quantity != other.Quantity) return false;
      if (!object.Equals(UnitPrice, other.UnitPrice)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (shoppingCartId_ != null) hash ^= ShoppingCartId.GetHashCode();
      if (productId_ != null) hash ^= ProductId.GetHashCode();
      if (Quantity != 0) hash ^= Quantity.GetHashCode();
      if (unitPrice_ != null) hash ^= UnitPrice.GetHashCode();
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
      if (unitPrice_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(UnitPrice);
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
      if (unitPrice_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(UnitPrice);
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
      if (unitPrice_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(UnitPrice);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ShoppingCartProductAddedDomainEvent other) {
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
      if (other.unitPrice_ != null) {
        if (unitPrice_ == null) {
          UnitPrice = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
        }
        UnitPrice.MergeFrom(other.UnitPrice);
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
          case 34: {
            if (unitPrice_ == null) {
              UnitPrice = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
            }
            input.ReadMessage(UnitPrice);
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
          case 34: {
            if (unitPrice_ == null) {
              UnitPrice = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
            }
            input.ReadMessage(UnitPrice);
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
