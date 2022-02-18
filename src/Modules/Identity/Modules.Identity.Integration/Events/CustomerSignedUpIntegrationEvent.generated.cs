// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Identity/_schemas/Events/IntegrationEvents/customer_signed_up_integration_event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Identity.Integration.Events {

  /// <summary>Holder for reflection information generated from Modules/Identity/_schemas/Events/IntegrationEvents/customer_signed_up_integration_event.proto</summary>
  public static partial class CustomerSignedUpIntegrationEventReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Identity/_schemas/Events/IntegrationEvents/customer_signed_up_integration_event.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CustomerSignedUpIntegrationEventReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cl1Nb2R1bGVzL0lkZW50aXR5L19zY2hlbWFzL0V2ZW50cy9JbnRlZ3JhdGlv",
            "bkV2ZW50cy9jdXN0b21lcl9zaWduZWRfdXBfaW50ZWdyYXRpb25fZXZlbnQu",
            "cHJvdG8aPFNoYXJlZEtlcm5lbC9TaGFyZWRLZXJuZWwuSW5mcmFzdHJ1Y3R1",
            "cmUvX3NjaGVtYXMvdXVpZC5wcm90byJ0CiBDdXN0b21lclNpZ25lZFVwSW50",
            "ZWdyYXRpb25FdmVudBIWCgd1c2VyX2lkGAEgASgLMgUuVXVpZBIgChhlbWFp",
            "bF9jb25maXJtYXRpb25fdG9rZW4YAiABKAkSFgoOYWN0aXZhdGlvbl91cmwY",
            "AyABKAlCLKoCKVZTaG9wLk1vZHVsZXMuSWRlbnRpdHkuSW50ZWdyYXRpb24u",
            "RXZlbnRzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Infrastructure.Types.UuidReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Identity.Integration.Events.CustomerSignedUpIntegrationEvent), global::VShop.Modules.Identity.Integration.Events.CustomerSignedUpIntegrationEvent.Parser, new[]{ "UserId", "EmailConfirmationToken", "ActivationUrl" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class CustomerSignedUpIntegrationEvent : pb::IMessage<CustomerSignedUpIntegrationEvent>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<CustomerSignedUpIntegrationEvent> _parser = new pb::MessageParser<CustomerSignedUpIntegrationEvent>(() => new CustomerSignedUpIntegrationEvent());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CustomerSignedUpIntegrationEvent> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Identity.Integration.Events.CustomerSignedUpIntegrationEventReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CustomerSignedUpIntegrationEvent() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CustomerSignedUpIntegrationEvent(CustomerSignedUpIntegrationEvent other) : this() {
      userId_ = other.userId_ != null ? other.userId_.Clone() : null;
      emailConfirmationToken_ = other.emailConfirmationToken_;
      activationUrl_ = other.activationUrl_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CustomerSignedUpIntegrationEvent Clone() {
      return new CustomerSignedUpIntegrationEvent(this);
    }

    /// <summary>Field number for the "user_id" field.</summary>
    public const int UserIdFieldNumber = 1;
    private global::VShop.SharedKernel.Infrastructure.Types.Uuid userId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::VShop.SharedKernel.Infrastructure.Types.Uuid UserId {
      get { return userId_; }
      set {
        userId_ = value;
      }
    }

    /// <summary>Field number for the "email_confirmation_token" field.</summary>
    public const int EmailConfirmationTokenFieldNumber = 2;
    private string emailConfirmationToken_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string EmailConfirmationToken {
      get { return emailConfirmationToken_; }
      set {
        emailConfirmationToken_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "activation_url" field.</summary>
    public const int ActivationUrlFieldNumber = 3;
    private string activationUrl_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ActivationUrl {
      get { return activationUrl_; }
      set {
        activationUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as CustomerSignedUpIntegrationEvent);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CustomerSignedUpIntegrationEvent other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(UserId, other.UserId)) return false;
      if (EmailConfirmationToken != other.EmailConfirmationToken) return false;
      if (ActivationUrl != other.ActivationUrl) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (userId_ != null) hash ^= UserId.GetHashCode();
      if (EmailConfirmationToken.Length != 0) hash ^= EmailConfirmationToken.GetHashCode();
      if (ActivationUrl.Length != 0) hash ^= ActivationUrl.GetHashCode();
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
      if (userId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(UserId);
      }
      if (EmailConfirmationToken.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(EmailConfirmationToken);
      }
      if (ActivationUrl.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(ActivationUrl);
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
      if (userId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(UserId);
      }
      if (EmailConfirmationToken.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(EmailConfirmationToken);
      }
      if (ActivationUrl.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(ActivationUrl);
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
      if (userId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(UserId);
      }
      if (EmailConfirmationToken.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(EmailConfirmationToken);
      }
      if (ActivationUrl.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ActivationUrl);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CustomerSignedUpIntegrationEvent other) {
      if (other == null) {
        return;
      }
      if (other.userId_ != null) {
        if (userId_ == null) {
          UserId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
        }
        UserId.MergeFrom(other.UserId);
      }
      if (other.EmailConfirmationToken.Length != 0) {
        EmailConfirmationToken = other.EmailConfirmationToken;
      }
      if (other.ActivationUrl.Length != 0) {
        ActivationUrl = other.ActivationUrl;
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
            if (userId_ == null) {
              UserId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(UserId);
            break;
          }
          case 18: {
            EmailConfirmationToken = input.ReadString();
            break;
          }
          case 26: {
            ActivationUrl = input.ReadString();
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
            if (userId_ == null) {
              UserId = new global::VShop.SharedKernel.Infrastructure.Types.Uuid();
            }
            input.ReadMessage(UserId);
            break;
          }
          case 18: {
            EmailConfirmationToken = input.ReadString();
            break;
          }
          case 26: {
            ActivationUrl = input.ReadString();
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
