// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: _schema/message_metadata.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.SharedKernel.Messaging {

  /// <summary>Holder for reflection information generated from _schema/message_metadata.proto</summary>
  public static partial class MessageMetadataReflection {

    #region Descriptor
    /// <summary>File descriptor for _schema/message_metadata.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MessageMetadataReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Ch5fc2NoZW1hL21lc3NhZ2VfbWV0YWRhdGEucHJvdG8aEl9zY2hlbWEvdXVp",
            "ZC5wcm90bxofZ29vZ2xlL3Byb3RvYnVmL3RpbWVzdGFtcC5wcm90byKcAQoP",
            "TWVzc2FnZU1ldGFkYXRhEhkKCm1lc3NhZ2VfaWQYASABKAsyBS5VdWlkEh0K",
            "DmNvcnJlbGF0aW9uX2lkGAIgASgLMgUuVXVpZBIbCgxjYXVzYXRpb25faWQY",
            "AyABKAsyBS5VdWlkEjIKDmVmZmVjdGl2ZV90aW1lGAQgASgLMhouZ29vZ2xl",
            "LnByb3RvYnVmLlRpbWVzdGFtcEIfqgIcVlNob3AuU2hhcmVkS2VybmVsLk1l",
            "c3NhZ2luZ2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Messaging.CustomTypes.UuidReflection.Descriptor, global::Google.Protobuf.WellKnownTypes.TimestampReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.SharedKernel.Messaging.MessageMetadata), global::VShop.SharedKernel.Messaging.MessageMetadata.Parser, new[]{ "MessageId", "CorrelationId", "CausationId", "EffectiveTime" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class MessageMetadata : pb::IMessage<MessageMetadata>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<MessageMetadata> _parser = new pb::MessageParser<MessageMetadata>(() => new MessageMetadata());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<MessageMetadata> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.SharedKernel.Messaging.MessageMetadataReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageMetadata() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageMetadata(MessageMetadata other) : this() {
      messageId_ = other.messageId_ != null ? other.messageId_.Clone() : null;
      correlationId_ = other.correlationId_ != null ? other.correlationId_.Clone() : null;
      causationId_ = other.causationId_ != null ? other.causationId_.Clone() : null;
      effectiveTime_ = other.effectiveTime_ != null ? other.effectiveTime_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageMetadata Clone() {
      return new MessageMetadata(this);
    }

    /// <summary>Field number for the "message_id" field.</summary>
    public const int MessageIdFieldNumber = 1;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Uuid messageId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Uuid MessageId {
      get { return messageId_; }
      set {
        messageId_ = value;
      }
    }

    /// <summary>Field number for the "correlation_id" field.</summary>
    public const int CorrelationIdFieldNumber = 2;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Uuid correlationId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Uuid CorrelationId {
      get { return correlationId_; }
      set {
        correlationId_ = value;
      }
    }

    /// <summary>Field number for the "causation_id" field.</summary>
    public const int CausationIdFieldNumber = 3;
    private global::VShop.SharedKernel.Messaging.CustomTypes.Uuid causationId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Messaging.CustomTypes.Uuid CausationId {
      get { return causationId_; }
      set {
        causationId_ = value;
      }
    }

    /// <summary>Field number for the "effective_time" field.</summary>
    public const int EffectiveTimeFieldNumber = 4;
    private global::Google.Protobuf.WellKnownTypes.Timestamp effectiveTime_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Google.Protobuf.WellKnownTypes.Timestamp EffectiveTime {
      get { return effectiveTime_; }
      set {
        effectiveTime_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as MessageMetadata);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(MessageMetadata other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(MessageId, other.MessageId)) return false;
      if (!object.Equals(CorrelationId, other.CorrelationId)) return false;
      if (!object.Equals(CausationId, other.CausationId)) return false;
      if (!object.Equals(EffectiveTime, other.EffectiveTime)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (messageId_ != null) hash ^= MessageId.GetHashCode();
      if (correlationId_ != null) hash ^= CorrelationId.GetHashCode();
      if (causationId_ != null) hash ^= CausationId.GetHashCode();
      if (effectiveTime_ != null) hash ^= EffectiveTime.GetHashCode();
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
      if (messageId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(MessageId);
      }
      if (correlationId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(CorrelationId);
      }
      if (causationId_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(CausationId);
      }
      if (effectiveTime_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(EffectiveTime);
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
      if (messageId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(MessageId);
      }
      if (correlationId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(CorrelationId);
      }
      if (causationId_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(CausationId);
      }
      if (effectiveTime_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(EffectiveTime);
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
      if (messageId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(MessageId);
      }
      if (correlationId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(CorrelationId);
      }
      if (causationId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(CausationId);
      }
      if (effectiveTime_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EffectiveTime);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(MessageMetadata other) {
      if (other == null) {
        return;
      }
      if (other.messageId_ != null) {
        if (messageId_ == null) {
          MessageId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        MessageId.MergeFrom(other.MessageId);
      }
      if (other.correlationId_ != null) {
        if (correlationId_ == null) {
          CorrelationId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        CorrelationId.MergeFrom(other.CorrelationId);
      }
      if (other.causationId_ != null) {
        if (causationId_ == null) {
          CausationId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        CausationId.MergeFrom(other.CausationId);
      }
      if (other.effectiveTime_ != null) {
        if (effectiveTime_ == null) {
          EffectiveTime = new global::Google.Protobuf.WellKnownTypes.Timestamp();
        }
        EffectiveTime.MergeFrom(other.EffectiveTime);
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
            if (messageId_ == null) {
              MessageId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(MessageId);
            break;
          }
          case 18: {
            if (correlationId_ == null) {
              CorrelationId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(CorrelationId);
            break;
          }
          case 26: {
            if (causationId_ == null) {
              CausationId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(CausationId);
            break;
          }
          case 34: {
            if (effectiveTime_ == null) {
              EffectiveTime = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(EffectiveTime);
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
            if (messageId_ == null) {
              MessageId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(MessageId);
            break;
          }
          case 18: {
            if (correlationId_ == null) {
              CorrelationId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(CorrelationId);
            break;
          }
          case 26: {
            if (causationId_ == null) {
              CausationId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
            }
            input.ReadMessage(CausationId);
            break;
          }
          case 34: {
            if (effectiveTime_ == null) {
              EffectiveTime = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(EffectiveTime);
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
