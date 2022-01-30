// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_checkout_requested_domain_event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.Domain.Events {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_checkout_requested_domain_event.proto</summary>
  internal static partial class ShoppingCartCheckoutRequestedDomainEventReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_checkout_requested_domain_event.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ShoppingCartCheckoutRequestedDomainEventReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cl5Nb2R1bGVzL1NhbGVzL19zY2hlbWFzL0V2ZW50cy9Eb21haW5FdmVudHMv",
            "c2hvcHBpbmdfY2FydF9jaGVja291dF9yZXF1ZXN0ZWRfZG9tYWluX2V2ZW50",
            "LnByb3RvGjxTaGFyZWRLZXJuZWwvU2hhcmVkS2VybmVsLkluZnJhc3RydWN0",
            "dXJlL19zY2hlbWFzL3V1aWQucHJvdG8aH2dvb2dsZS9wcm90b2J1Zi90aW1l",
            "c3RhbXAucHJvdG8ilgEKKFNob3BwaW5nQ2FydENoZWNrb3V0UmVxdWVzdGVk",
            "RG9tYWluRXZlbnQSFwoIb3JkZXJfaWQYASABKAsyBS5VdWlkEh8KEHNob3Bw",
            "aW5nX2NhcnRfaWQYAiABKAsyBS5VdWlkEjAKDGNvbmZpcm1lZF9hdBgDIAEo",
            "CzIaLmdvb2dsZS5wcm90b2J1Zi5UaW1lc3RhbXBCJKoCIVZTaG9wLk1vZHVs",
            "ZXMuU2FsZXMuRG9tYWluLkV2ZW50c2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Infrastructure.Types.UuidReflection.Descriptor, global::Google.Protobuf.WellKnownTypes.TimestampReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.Domain.Events.ShoppingCartCheckoutRequestedDomainEvent), global::VShop.Modules.Sales.Domain.Events.ShoppingCartCheckoutRequestedDomainEvent.Parser, new[]{ "OrderId", "ShoppingCartId", "ConfirmedAt" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  internal sealed partial class ShoppingCartCheckoutRequestedDomainEvent : pb::IMessage<ShoppingCartCheckoutRequestedDomainEvent>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ShoppingCartCheckoutRequestedDomainEvent> _parser = new pb::MessageParser<ShoppingCartCheckoutRequestedDomainEvent>(() => new ShoppingCartCheckoutRequestedDomainEvent());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ShoppingCartCheckoutRequestedDomainEvent> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.Domain.Events.ShoppingCartCheckoutRequestedDomainEventReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartCheckoutRequestedDomainEvent() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartCheckoutRequestedDomainEvent(ShoppingCartCheckoutRequestedDomainEvent other) : this() {
      orderId_ = other.orderId_ != null ? other.orderId_.Clone() : null;
      shoppingCartId_ = other.shoppingCartId_ != null ? other.shoppingCartId_.Clone() : null;
      confirmedAt_ = other.confirmedAt_ != null ? other.confirmedAt_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartCheckoutRequestedDomainEvent Clone() {
      return new ShoppingCartCheckoutRequestedDomainEvent(this);
    }

    /// <summary>Field number for the "order_id" field.</summary>
    public const int OrderIdFieldNumber = 1;
    private global::VShop.SharedKernel.Infrastructure.Types.Uuid orderId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Infrastructure.Types.Uuid OrderId {
      get { return orderId_; }
      set {
        orderId_ = value;
      }
    }

    /// <summary>Field number for the "shopping_cart_id" field.</summary>
    public const int ShoppingCartIdFieldNumber = 2;
    private global::VShop.SharedKernel.Infrastructure.Types.Uuid shoppingCartId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Infrastructure.Types.Uuid ShoppingCartId {
      get { return shoppingCartId_; }
      set {
        shoppingCartId_ = value;
      }
    }

    /// <summary>Field number for the "confirmed_at" field.</summary>
    public const int ConfirmedAtFieldNumber = 3;
    private global::Google.Protobuf.WellKnownTypes.Timestamp confirmedAt_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Google.Protobuf.WellKnownTypes.Timestamp ConfirmedAt {
      get { return confirmedAt_; }
      set {
        confirmedAt_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ShoppingCartCheckoutRequestedDomainEvent);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ShoppingCartCheckoutRequestedDomainEvent other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(OrderId, other.OrderId)) return false;
      if (!object.Equals(ShoppingCartId, other.ShoppingCartId)) return false;
      if (!object.Equals(ConfirmedAt, other.ConfirmedAt)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (orderId_ != null) hash ^= OrderId.GetHashCode();
      if (shoppingCartId_ != null) hash ^= ShoppingCartId.GetHashCode();
      if (confirmedAt_ != null) hash ^= ConfirmedAt.GetHashCode();
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
      if (orderId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(OrderId);
      }
      if (shoppingCartId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(ShoppingCartId);
      }
      if (confirmedAt_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(ConfirmedAt);
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
      if (orderId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(OrderId);
      }
      if (shoppingCartId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(ShoppingCartId);
      }
      if (confirmedAt_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(ConfirmedAt);
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
      if (orderId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(OrderId);
      }
      if (shoppingCartId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ShoppingCartId);
      }
      if (confirmedAt_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ConfirmedAt);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ShoppingCartCheckoutRequestedDomainEvent other) {
      if (other == null) {
        return;
      }
      if (other.orderId_ != null) {
        if (orderId_ == null) {
          OrderId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
        }
        OrderId.MergeFrom(other.OrderId);
      }
      if (other.shoppingCartId_ != null) {
        if (shoppingCartId_ == null) {
          ShoppingCartId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
        }
        ShoppingCartId.MergeFrom(other.ShoppingCartId);
      }
      if (other.confirmedAt_ != null) {
        if (confirmedAt_ == null) {
          ConfirmedAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
        }
        ConfirmedAt.MergeFrom(other.ConfirmedAt);
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
            if (orderId_ == null) {
              OrderId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(OrderId);
            break;
          }
          case 18: {
            if (shoppingCartId_ == null) {
              ShoppingCartId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(ShoppingCartId);
            break;
          }
          case 26: {
            if (confirmedAt_ == null) {
              ConfirmedAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(ConfirmedAt);
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
            if (orderId_ == null) {
              OrderId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(OrderId);
            break;
          }
          case 18: {
            if (shoppingCartId_ == null) {
              ShoppingCartId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(ShoppingCartId);
            break;
          }
          case 26: {
            if (confirmedAt_ == null) {
              ConfirmedAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(ConfirmedAt);
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
