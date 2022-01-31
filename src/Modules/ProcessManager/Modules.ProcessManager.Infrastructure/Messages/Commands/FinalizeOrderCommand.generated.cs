// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/ProcessManager/_schemas/finalize_order_command.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.ProcessManager.Infrastructure.Messages.Commands {

  /// <summary>Holder for reflection information generated from Modules/ProcessManager/_schemas/finalize_order_command.proto</summary>
  internal static partial class FinalizeOrderCommandReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/ProcessManager/_schemas/finalize_order_command.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static FinalizeOrderCommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjxNb2R1bGVzL1Byb2Nlc3NNYW5hZ2VyL19zY2hlbWFzL2ZpbmFsaXplX29y",
            "ZGVyX2NvbW1hbmQucHJvdG8aPFNoYXJlZEtlcm5lbC9TaGFyZWRLZXJuZWwu",
            "SW5mcmFzdHJ1Y3R1cmUvX3NjaGVtYXMvdXVpZC5wcm90byKpAQoURmluYWxp",
            "emVPcmRlckNvbW1hbmQSFwoIb3JkZXJfaWQYASABKAsyBS5VdWlkEjQKC29y",
            "ZGVyX2xpbmVzGAIgAygLMh8uRmluYWxpemVPcmRlckNvbW1hbmQuT3JkZXJM",
            "aW5lGkIKCU9yZGVyTGluZRIZCgpwcm9kdWN0X2lkGAEgASgLMgUuVXVpZBIa",
            "ChJPdXRPZlN0b2NrUXVhbnRpdHkYAiABKAVCQKoCPVZTaG9wLk1vZHVsZXMu",
            "UHJvY2Vzc01hbmFnZXIuSW5mcmFzdHJ1Y3R1cmUuTWVzc2FnZXMuQ29tbWFu",
            "ZHNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Infrastructure.Types.UuidReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand), global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Parser, new[]{ "OrderId", "OrderLines" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Types.OrderLine), global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Types.OrderLine.Parser, new[]{ "ProductId", "OutOfStockQuantity" }, null, null, null, null)})
          }));
    }
    #endregion

  }
  #region Messages
  internal sealed partial class FinalizeOrderCommand : pb::IMessage<FinalizeOrderCommand>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<FinalizeOrderCommand> _parser = new pb::MessageParser<FinalizeOrderCommand>(() => new FinalizeOrderCommand());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<FinalizeOrderCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommandReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public FinalizeOrderCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public FinalizeOrderCommand(FinalizeOrderCommand other) : this() {
      orderId_ = other.orderId_ != null ? other.orderId_.Clone() : null;
      orderLines_ = other.orderLines_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public FinalizeOrderCommand Clone() {
      return new FinalizeOrderCommand(this);
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

    /// <summary>Field number for the "order_lines" field.</summary>
    public const int OrderLinesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Types.OrderLine> _repeated_orderLines_codec
        = pb::FieldCodec.ForMessage(18, global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Types.OrderLine.Parser);
    private readonly pbc::RepeatedField<global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Types.OrderLine> orderLines_ = new pbc::RepeatedField<global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Types.OrderLine>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Types.OrderLine> OrderLines {
      get { return orderLines_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as FinalizeOrderCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(FinalizeOrderCommand other) {
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
    public void MergeFrom(FinalizeOrderCommand other) {
      if (other == null) {
        return;
      }
      if (other.orderId_ != null) {
        if (orderId_ == null) {
          OrderId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
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
              OrderId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
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
              OrderId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
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
    /// <summary>Container for nested types declared in the FinalizeOrderCommand message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
      internal sealed partial class OrderLine : pb::IMessage<OrderLine>
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
          get { return global::VShop.Modules.ProcessManager.Infrastructure.Messages.Commands.FinalizeOrderCommand.Descriptor.NestedTypes[0]; }
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
          outOfStockQuantity_ = other.outOfStockQuantity_;
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

        /// <summary>Field number for the "OutOfStockQuantity" field.</summary>
        public const int OutOfStockQuantityFieldNumber = 2;
        private int outOfStockQuantity_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public int OutOfStockQuantity {
          get { return outOfStockQuantity_; }
          set {
            outOfStockQuantity_ = value;
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
          if (OutOfStockQuantity != other.OutOfStockQuantity) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public override int GetHashCode() {
          int hash = 1;
          if (productId_ != null) hash ^= ProductId.GetHashCode();
          if (OutOfStockQuantity != 0) hash ^= OutOfStockQuantity.GetHashCode();
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
          if (OutOfStockQuantity != 0) {
            output.WriteRawTag(16);
            output.WriteInt32(OutOfStockQuantity);
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
          if (OutOfStockQuantity != 0) {
            output.WriteRawTag(16);
            output.WriteInt32(OutOfStockQuantity);
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
          if (OutOfStockQuantity != 0) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(OutOfStockQuantity);
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
          if (other.OutOfStockQuantity != 0) {
            OutOfStockQuantity = other.OutOfStockQuantity;
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
                OutOfStockQuantity = input.ReadInt32();
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
                OutOfStockQuantity = input.ReadInt32();
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
