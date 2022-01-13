// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_delivery_address_set_domain_event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.Modules.Sales.Domain.Events {

  /// <summary>Holder for reflection information generated from Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_delivery_address_set_domain_event.proto</summary>
  public static partial class ShoppingCartDeliveryAddressSetDomainEventReflection {

    #region Descriptor
    /// <summary>File descriptor for Modules/Sales/_schemas/Events/DomainEvents/shopping_cart_delivery_address_set_domain_event.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ShoppingCartDeliveryAddressSetDomainEventReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CmBNb2R1bGVzL1NhbGVzL19zY2hlbWFzL0V2ZW50cy9Eb21haW5FdmVudHMv",
            "c2hvcHBpbmdfY2FydF9kZWxpdmVyeV9hZGRyZXNzX3NldF9kb21haW5fZXZl",
            "bnQucHJvdG8aN1NoYXJlZEtlcm5lbC9TaGFyZWRLZXJuZWwuTWVzc2FnaW5n",
            "L19zY2hlbWFzL3V1aWQucHJvdG8itQEKKVNob3BwaW5nQ2FydERlbGl2ZXJ5",
            "QWRkcmVzc1NldERvbWFpbkV2ZW50Eh8KEHNob3BwaW5nX2NhcnRfaWQYASAB",
            "KAsyBS5VdWlkEgwKBGNpdHkYAiABKAkSFAoMY291bnRyeV9jb2RlGAMgASgJ",
            "EhMKC3Bvc3RhbF9jb2RlGAQgASgJEhYKDnN0YXRlX3Byb3ZpbmNlGAUgASgJ",
            "EhYKDnN0cmVldF9hZGRyZXNzGAYgASgJQiSqAiFWU2hvcC5Nb2R1bGVzLlNh",
            "bGVzLkRvbWFpbi5FdmVudHNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::VShop.SharedKernel.Messaging.CustomTypes.UuidReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::VShop.Modules.Sales.Domain.Events.ShoppingCartDeliveryAddressSetDomainEvent), global::VShop.Modules.Sales.Domain.Events.ShoppingCartDeliveryAddressSetDomainEvent.Parser, new[]{ "ShoppingCartId", "City", "CountryCode", "PostalCode", "StateProvince", "StreetAddress" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ShoppingCartDeliveryAddressSetDomainEvent : pb::IMessage<ShoppingCartDeliveryAddressSetDomainEvent>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ShoppingCartDeliveryAddressSetDomainEvent> _parser = new pb::MessageParser<ShoppingCartDeliveryAddressSetDomainEvent>(() => new ShoppingCartDeliveryAddressSetDomainEvent());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ShoppingCartDeliveryAddressSetDomainEvent> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::VShop.Modules.Sales.Domain.Events.ShoppingCartDeliveryAddressSetDomainEventReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartDeliveryAddressSetDomainEvent() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartDeliveryAddressSetDomainEvent(ShoppingCartDeliveryAddressSetDomainEvent other) : this() {
      shoppingCartId_ = other.shoppingCartId_ != null ? other.shoppingCartId_.Clone() : null;
      city_ = other.city_;
      countryCode_ = other.countryCode_;
      postalCode_ = other.postalCode_;
      stateProvince_ = other.stateProvince_;
      streetAddress_ = other.streetAddress_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ShoppingCartDeliveryAddressSetDomainEvent Clone() {
      return new ShoppingCartDeliveryAddressSetDomainEvent(this);
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

    /// <summary>Field number for the "city" field.</summary>
    public const int CityFieldNumber = 2;
    private string city_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string City {
      get { return city_; }
      set {
        city_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "country_code" field.</summary>
    public const int CountryCodeFieldNumber = 3;
    private string countryCode_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string CountryCode {
      get { return countryCode_; }
      set {
        countryCode_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "postal_code" field.</summary>
    public const int PostalCodeFieldNumber = 4;
    private string postalCode_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string PostalCode {
      get { return postalCode_; }
      set {
        postalCode_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "state_province" field.</summary>
    public const int StateProvinceFieldNumber = 5;
    private string stateProvince_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string StateProvince {
      get { return stateProvince_; }
      set {
        stateProvince_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "street_address" field.</summary>
    public const int StreetAddressFieldNumber = 6;
    private string streetAddress_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string StreetAddress {
      get { return streetAddress_; }
      set {
        streetAddress_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ShoppingCartDeliveryAddressSetDomainEvent);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ShoppingCartDeliveryAddressSetDomainEvent other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ShoppingCartId, other.ShoppingCartId)) return false;
      if (City != other.City) return false;
      if (CountryCode != other.CountryCode) return false;
      if (PostalCode != other.PostalCode) return false;
      if (StateProvince != other.StateProvince) return false;
      if (StreetAddress != other.StreetAddress) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (shoppingCartId_ != null) hash ^= ShoppingCartId.GetHashCode();
      if (City.Length != 0) hash ^= City.GetHashCode();
      if (CountryCode.Length != 0) hash ^= CountryCode.GetHashCode();
      if (PostalCode.Length != 0) hash ^= PostalCode.GetHashCode();
      if (StateProvince.Length != 0) hash ^= StateProvince.GetHashCode();
      if (StreetAddress.Length != 0) hash ^= StreetAddress.GetHashCode();
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
      if (City.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(City);
      }
      if (CountryCode.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(CountryCode);
      }
      if (PostalCode.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(PostalCode);
      }
      if (StateProvince.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(StateProvince);
      }
      if (StreetAddress.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(StreetAddress);
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
      if (City.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(City);
      }
      if (CountryCode.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(CountryCode);
      }
      if (PostalCode.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(PostalCode);
      }
      if (StateProvince.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(StateProvince);
      }
      if (StreetAddress.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(StreetAddress);
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
      if (City.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(City);
      }
      if (CountryCode.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(CountryCode);
      }
      if (PostalCode.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PostalCode);
      }
      if (StateProvince.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(StateProvince);
      }
      if (StreetAddress.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(StreetAddress);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ShoppingCartDeliveryAddressSetDomainEvent other) {
      if (other == null) {
        return;
      }
      if (other.shoppingCartId_ != null) {
        if (shoppingCartId_ == null) {
          ShoppingCartId = new global::VShop.SharedKernel.Messaging.CustomTypes.Uuid();
        }
        ShoppingCartId.MergeFrom(other.ShoppingCartId);
      }
      if (other.City.Length != 0) {
        City = other.City;
      }
      if (other.CountryCode.Length != 0) {
        CountryCode = other.CountryCode;
      }
      if (other.PostalCode.Length != 0) {
        PostalCode = other.PostalCode;
      }
      if (other.StateProvince.Length != 0) {
        StateProvince = other.StateProvince;
      }
      if (other.StreetAddress.Length != 0) {
        StreetAddress = other.StreetAddress;
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
            City = input.ReadString();
            break;
          }
          case 26: {
            CountryCode = input.ReadString();
            break;
          }
          case 34: {
            PostalCode = input.ReadString();
            break;
          }
          case 42: {
            StateProvince = input.ReadString();
            break;
          }
          case 50: {
            StreetAddress = input.ReadString();
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
            City = input.ReadString();
            break;
          }
          case 26: {
            CountryCode = input.ReadString();
            break;
          }
          case 34: {
            PostalCode = input.ReadString();
            break;
          }
          case 42: {
            StateProvince = input.ReadString();
            break;
          }
          case 50: {
            StreetAddress = input.ReadString();
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
