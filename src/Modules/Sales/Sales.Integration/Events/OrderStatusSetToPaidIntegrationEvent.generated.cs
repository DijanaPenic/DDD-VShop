// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Events/order_status_set_to_paid_integration_event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.Integration.Events {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Events/order_status_set_to_paid_integration_event.proto</summary>
  public static partial class OrderStatusSetToPaidIntegrationEventReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Events/order_status_set_to_paid_integration_event.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static OrderStatusSetToPaidIntegrationEventReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Ck5Nb2R1bGVzL1NhbGVzL19zY2hlbWFzL0V2ZW50cy9vcmRlcl9zdGF0dXNf",
            "c2V0X3RvX3BhaWRfaW50ZWdyYXRpb25fZXZlbnQucHJvdG8aN1NoYXJlZEtl",
            "cm5lbC9TaGFyZWRLZXJuZWwuTWVzc2FnaW5nL19zY2hlbWFzL3V1aWQucHJv",
            "dG8aOlNoYXJlZEtlcm5lbC9TaGFyZWRLZXJuZWwuTWVzc2FnaW5nL19zY2hl",
            "bWFzL2RlY2ltYWwucHJvdG8i2AEKJE9yZGVyU3RhdHVzU2V0VG9QYWlkSW50",
            "ZWdyYXRpb25FdmVudBIXCghvcmRlcl9pZBgBIAEoCzIFLlV1aWQSRAoLb3Jk",
            "ZXJfbGluZXMYAiADKAsyLy5PcmRlclN0YXR1c1NldFRvUGFpZEludGVncmF0",
            "aW9uRXZlbnQuT3JkZXJMaW5lGlEKCU9yZGVyTGluZRIZCgpwcm9kdWN0X2lk",
            "GAEgASgLMgUuVXVpZBIQCghxdWFudGl0eRgCIAEoBRIXCgVwcmljZRgDIAEo",
            "CzIILkRlY2ltYWxCKaoCJlZTaG9wLk1vZHVsZXMuU2FsZXMuSW50ZWdyYXRp",
            "b24uRXZlbnRzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Messaging.CustomTypes.UuidReflection.Descriptor, global::VShop.SharedKernel.Messaging.CustomTypes.DecimalReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent), global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Parser, new[]{ "OrderId", "OrderLines" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Types.OrderLine), global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Types.OrderLine.Parser, new[]{ "ProductId", "Quantity", "Price" }, null, null, null, null)})
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class OrderStatusSetToPaidIntegrationEvent : pb::IMessage<OrderStatusSetToPaidIntegrationEvent>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<OrderStatusSetToPaidIntegrationEvent> _parser = new pb::MessageParser<OrderStatusSetToPaidIntegrationEvent>(() => new OrderStatusSetToPaidIntegrationEvent());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<OrderStatusSetToPaidIntegrationEvent> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEventReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OrderStatusSetToPaidIntegrationEvent() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OrderStatusSetToPaidIntegrationEvent(OrderStatusSetToPaidIntegrationEvent other) : this() {
      orderId_ = other.orderId_ != null ? other.orderId_.Clone() : null;
      orderLines_ = other.orderLines_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OrderStatusSetToPaidIntegrationEvent Clone() {
      return new OrderStatusSetToPaidIntegrationEvent(this);
    }

    /// <summary>Field number for the "order_id" field.</summary>
    public const int OrderIdFieldNumber = 1;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Uuid orderId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Uuid OrderId {
      get { return orderId_; }
      set {
        orderId_ = value;
      }
    }

    /// <summary>Field number for the "order_lines" field.</summary>
    public const int OrderLinesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Types.OrderLine> _repeated_orderLines_codec
        = pb::FieldCodec.ForMessage(18, global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Types.OrderLine.Parser);
    private readonly pbc::RepeatedField<global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Types.OrderLine> orderLines_ = new pbc::RepeatedField<global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Types.OrderLine>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Types.OrderLine> OrderLines {
      get { return orderLines_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as OrderStatusSetToPaidIntegrationEvent);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(OrderStatusSetToPaidIntegrationEvent other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(OrderId, other.OrderId)) return false;
      if(!orderLines_.Equals(other.orderLines_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (orderId_ != null) hash ^= OrderId.GetHashCode();
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
      size += orderLines_.CalculateSize(_repeated_orderLines_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(OrderStatusSetToPaidIntegrationEvent other) {
      if (other == null) {
        return;
      }
      if (other.orderId_ != null) {
        if (orderId_ == null) {
          OrderId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        OrderId.MergeFrom(other.OrderId);
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
              OrderId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(OrderId);
            break;
          }
          case 18: {
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
              OrderId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(OrderId);
            break;
          }
          case 18: {
            orderLines_.AddEntriesFrom(ref input, _repeated_orderLines_codec);
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the OrderStatusSetToPaidIntegrationEvent message type.</summary>
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
          get { return global::VShop.Modules.Sales.Integration.Events.OrderStatusSetToPaidIntegrationEvent.Descriptor.NestedTypes[0]; }
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
          price_ = other.price_ != null ? other.price_.Clone() : null;
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public OrderLine Clone() {
          return new OrderLine(this);
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

        /// <summary>Field number for the "price" field.</summary>
        public const int PriceFieldNumber = 3;
        private global::VShop.SharedKernel.Messaging.CustomTypes.Decimal price_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public global::VShop.SharedKernel.Messaging.CustomTypes.Decimal Price {
          get { return price_; }
          set {
            price_ = value;
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
          if (!object.Equals(Price, other.Price)) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public override int GetHashCode() {
          int hash = 1;
          if (productId_ != null) hash ^= ProductId.GetHashCode();
          if (Quantity != 0) hash ^= Quantity.GetHashCode();
          if (price_ != null) hash ^= Price.GetHashCode();
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
          if (price_ != null) {
            output.WriteRawTag(26);
            output.WriteMessage(Price);
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
          if (price_ != null) {
            output.WriteRawTag(26);
            output.WriteMessage(Price);
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
          if (price_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Price);
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
              ProductId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            ProductId.MergeFrom(other.ProductId);
          }
          if (other.Quantity != 0) {
            Quantity = other.Quantity;
          }
          if (other.price_ != null) {
            if (price_ == null) {
              Price = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
            }
            Price.MergeFrom(other.Price);
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
              case 16: {
                Quantity = input.ReadInt32();
                break;
              }
              case 26: {
                if (price_ == null) {
                  Price = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
                }
                input.ReadMessage(Price);
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
              case 16: {
                Quantity = input.ReadInt32();
                break;
              }
              case 26: {
                if (price_ == null) {
                  Price = new global::VShop.SharedKernel.Messaging.CustomTypes.Decimal();
                }
                input.ReadMessage(Price);
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
