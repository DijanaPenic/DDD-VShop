// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: SharedKernel/SharedKernel.Infrastructure/_schemas/gender.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace VShop.SharedKernel.Infrastructure.Types {

  /// <summary>Holder for reflection information generated from SharedKernel/SharedKernel.Infrastructure/_schemas/gender.proto</summary>
  public static partial class GenderReflection {

    #region Descriptor
    /// <summary>File descriptor for SharedKernel/SharedKernel.Infrastructure/_schemas/gender.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static GenderReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cj5TaGFyZWRLZXJuZWwvU2hhcmVkS2VybmVsLkluZnJhc3RydWN0dXJlL19z",
            "Y2hlbWFzL2dlbmRlci5wcm90byopCgZHZW5kZXISCgoGRkVNQUxFEAASCAoE",
            "TUFMRRABEgkKBU9USEVSEAJCKqoCJ1ZTaG9wLlNoYXJlZEtlcm5lbC5JbmZy",
            "YXN0cnVjdHVyZS5UeXBlc2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::VShop.SharedKernel.Infrastructure.Types.Gender), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum Gender {
    [pbr::OriginalName("FEMALE")] Female = 0,
    [pbr::OriginalName("MALE")] Male = 1,
    [pbr::OriginalName("OTHER")] Other = 2,
  }

  #endregion

}

#endregion Designer generated code
