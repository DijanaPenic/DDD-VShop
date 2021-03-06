// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Events/IntegrationEvents/order_finalized_integration_event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.Integration.Events {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Events/IntegrationEvents/order_finalized_integration_event.proto</summary>
  public static partial class OrderFinalizedIntegrationEventReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Events/IntegrationEvents/order_finalized_integration_event.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static OrderFinalizedIntegrationEventReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CldNb2R1bGVzL1NhbGVzL19zY2hlbWFzL0V2ZW50cy9JbnRlZ3JhdGlvbkV2",
            "ZW50cy9vcmRlcl9maW5hbGl6ZWRfaW50ZWdyYXRpb25fZXZlbnQucHJvdG8a",
            "PFNoYXJlZEtlcm5lbC9TaGFyZWRLZXJuZWwuSW5mcmFzdHJ1Y3R1cmUvX3Nj",
            "aGVtYXMvdXVpZC5wcm90bxoXZ29vZ2xlL3R5cGUvbW9uZXkucHJvdG8i3gEK",
            "Hk9yZGVyRmluYWxpemVkSW50ZWdyYXRpb25FdmVudBIXCghvcmRlcl9pZBgB",
            "IAEoCzIFLlV1aWQSKQoNcmVmdW5kX2Ftb3VudBgCIAEoCzISLmdvb2dsZS50",
            "eXBlLk1vbmV5Ej4KC29yZGVyX2xpbmVzGAMgAygLMikuT3JkZXJGaW5hbGl6",
            "ZWRJbnRlZ3JhdGlvbkV2ZW50Lk9yZGVyTGluZRo4CglPcmRlckxpbmUSGQoK",
            "cHJvZHVjdF9pZBgBIAEoCzIFLlV1aWQSEAoIcXVhbnRpdHkYAiABKAVCKaoC",
            "JlZTaG9wLk1vZHVsZXMuU2FsZXMuSW50ZWdyYXRpb24uRXZlbnRzYgZwcm90",
            "bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Infrastructure.Types.UuidReflection.Descriptor, global::Google.Type.MoneyReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent), global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Parser, new[]{ "OrderId", "RefundAmount", "OrderLines" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Types.OrderLine), global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Types.OrderLine.Parser, new[]{ "ProductId", "Quantity" }, null, null, null, null)})
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class OrderFinalizedIntegrationEvent : pb::IMessage<OrderFinalizedIntegrationEvent>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<OrderFinalizedIntegrationEvent> _parser = new pb::MessageParser<OrderFinalizedIntegrationEvent>(() => new OrderFinalizedIntegrationEvent());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<OrderFinalizedIntegrationEvent> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEventReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OrderFinalizedIntegrationEvent() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OrderFinalizedIntegrationEvent(OrderFinalizedIntegrationEvent other) : this() {
      orderId_ = other.orderId_ != null ? other.orderId_.Clone() : null;
      refundAmount_ = other.refundAmount_ != null ? other.refundAmount_.Clone() : null;
      orderLines_ = other.orderLines_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OrderFinalizedIntegrationEvent Clone() {
      return new OrderFinalizedIntegrationEvent(this);
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

    /// <summary>Field number for the "refund_amount" field.</summary>
    public const int RefundAmountFieldNumber = 2;
    private global::Google.Type.Money refundAmount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Google.Type.Money RefundAmount {
      get { return refundAmount_; }
      set {
        refundAmount_ = value;
      }
    }

    /// <summary>Field number for the "order_lines" field.</summary>
    public const int OrderLinesFieldNumber = 3;
    private static readonly pb::FieldCodec<global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Types.OrderLine> _repeated_orderLines_codec
        = pb::FieldCodec.ForMessage(26, global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Types.OrderLine.Parser);
    private readonly pbc::RepeatedField<global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Types.OrderLine> orderLines_ = new pbc::RepeatedField<global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Types.OrderLine>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Types.OrderLine> OrderLines {
      get { return orderLines_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as OrderFinalizedIntegrationEvent);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(OrderFinalizedIntegrationEvent other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(OrderId, other.OrderId)) return false;
      if (!object.Equals(RefundAmount, other.RefundAmount)) return false;
      if(!orderLines_.Equals(other.orderLines_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (orderId_ != null) hash ^= OrderId.GetHashCode();
      if (refundAmount_ != null) hash ^= RefundAmount.GetHashCode();
      hash ^= orderLines_.GetHashCode();
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
      if (refundAmount_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(RefundAmount);
      }
      orderLines_.WriteTo(output, _repeated_orderLines_codec);
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
      if (refundAmount_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(RefundAmount);
      }
      orderLines_.WriteTo(ref output, _repeated_orderLines_codec);
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
      if (refundAmount_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(RefundAmount);
      }
      size += orderLines_.CalculateSize(_repeated_orderLines_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(OrderFinalizedIntegrationEvent other) {
      if (other == null) {
        return;
      }
      if (other.orderId_ != null) {
        if (orderId_ == null) {
          OrderId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
        }
        OrderId.MergeFrom(other.OrderId);
      }
      if (other.refundAmount_ != null) {
        if (refundAmount_ == null) {
          RefundAmount = new global::Google.Type.Money();
        }
        RefundAmount.MergeFrom(other.RefundAmount);
      }
      orderLines_.Add(other.orderLines_);
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
            if (refundAmount_ == null) {
              RefundAmount = new global::Google.Type.Money();
            }
            input.ReadMessage(RefundAmount);
            break;
          }
          case 26: {
            orderLines_.AddEntriesFrom(input, _repeated_orderLines_codec);
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
            if (refundAmount_ == null) {
              RefundAmount = new global::Google.Type.Money();
            }
            input.ReadMessage(RefundAmount);
            break;
          }
          case 26: {
            orderLines_.AddEntriesFrom(ref input, _repeated_orderLines_codec);
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the OrderFinalizedIntegrationEvent message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
      public sealed partial class OrderLine : pb::IMessage<OrderLine>
      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
          , pb::IBufferMessage
      #endif
      {
        private static readonly pb::MessageParser<OrderLine> _parser = new pb::MessageParser<OrderLine>(() => new OrderLine());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public static pb::MessageParser<OrderLine> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public static pbr::MessageDescriptor Descriptor {
          get { return global::VShop.Modules.Sales.Integration.Events.OrderFinalizedIntegrationEvent.Descriptor.NestedTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        pbr::MessageDescriptor pb::IMessage.Descriptor {
          get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public OrderLine() {
          OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public OrderLine(OrderLine other) : this() {
          productId_ = other.productId_ != null ? other.productId_.Clone() : null;
          quantity_ = other.quantity_;
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public OrderLine Clone() {
          return new OrderLine(this);
        }

        /// <summary>Field number for the "product_id" field.</summary>
        public const int ProductIdFieldNumber = 1;
        private global::VShop.SharedKernel.Infrastructure.Types.Uuid productId_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public global::VShop.SharedKernel.Infrastructure.Types.Uuid ProductId {
          get { return productId_; }
          set {
            productId_ = value;
          }
        }

        /// <summary>Field number for the "quantity" field.</summary>
        public const int QuantityFieldNumber = 2;
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
          return Equals(other as OrderLine);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public bool Equals(OrderLine other) {
          if (ReferenceEquals(other, null)) {
            return false;
          }
          if (ReferenceEquals(other, this)) {
            return true;
          }
          if (!object.Equals(ProductId, other.ProductId)) return false;
          if (Quantity != other.Quantity) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public override int GetHashCode() {
          int hash = 1;
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
          if (productId_ != null) {
            output.WriteRawTag(10);
            output.WriteMessage(ProductId);
          }
          if (Quantity != 0) {
            output.WriteRawTag(16);
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
          if (Quantity != 0) {
            output.WriteRawTag(16);
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
        public void MergeFrom(OrderLine other) {
          if (other == null) {
            return;
          }
          if (other.productId_ != null) {
            if (productId_ == null) {
              ProductId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
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
                if (productId_ == null) {
                  ProductId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
                }
                input.ReadMessage(ProductId);
                break;
              }
              case 16: {
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
                  ProductId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
                }
                input.ReadMessage(ProductId);
                break;
              }
              case 16: {
                Quantity = input.ReadInt32();
                break;
              }
            }
          }
        }
        #endif

      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code
