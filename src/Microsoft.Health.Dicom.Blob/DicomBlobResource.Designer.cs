﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Health.Dicom.Blob {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class DicomBlobResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DicomBlobResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Health.Dicom.Blob.DicomBlobResource", typeof(DicomBlobResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account key authentication is not supported..
        /// </summary>
        internal static string AzureStorageAccountKeyUnsupported {
            get {
                return ResourceManager.GetString("AzureStorageAccountKeyUnsupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to authenticate for Azure Blob Storage container &apos;{0}&apos; in account &apos;{1}&apos;..
        /// </summary>
        internal static string BlobStorageAuthenticateFailure {
            get {
                return ResourceManager.GetString("BlobStorageAuthenticateFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot connect to Azure Blob Storage container &apos;{0}&apos; in account &apos;{1}&apos;..
        /// </summary>
        internal static string BlobStorageConnectionFailure {
            get {
                return ResourceManager.GetString("BlobStorageConnectionFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to write to Azure Blob Storage container &apos;{0}&apos; in account &apos;{1}&apos;..
        /// </summary>
        internal static string BlobStorageRequestFailure {
            get {
                return ResourceManager.GetString("BlobStorageRequestFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UseManagedIdentity cannot be specified if the BlobContainerUri container query string parameters..
        /// </summary>
        internal static string ConflictingBlobExportAuthentication {
            get {
                return ResourceManager.GetString("ConflictingBlobExportAuthentication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ConnectionString and BlobContainerName cannot be specified along with BlobContainerUri..
        /// </summary>
        internal static string ConflictingExportBlobConnections {
            get {
                return ResourceManager.GetString("ConflictingExportBlobConnections", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Azure Blob Storage container &apos;{0}&apos; does not exist in account &apos;{1}&apos;..
        /// </summary>
        internal static string ContainerDoesNotExist {
            get {
                return ResourceManager.GetString("ContainerDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is an invalid escape sequence..
        /// </summary>
        internal static string InvalidEscapeSequence {
            get {
                return ResourceManager.GetString("InvalidEscapeSequence", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Managed identity cannot be used with a ConnectionString..
        /// </summary>
        internal static string InvalidExportBlobAuthentication {
            get {
                return ResourceManager.GetString("InvalidExportBlobAuthentication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not parse pattern for &apos;{0}&apos; for property &apos;{1}&apos;..
        /// </summary>
        internal static string InvalidPattern {
            get {
                return ResourceManager.GetString("InvalidPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find closing % for placeholder..
        /// </summary>
        internal static string MalformedPlaceholder {
            get {
                return ResourceManager.GetString("MalformedPlaceholder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please specify both ConnectionString and BlobContainerName..
        /// </summary>
        internal static string MissingExportBlobConnection {
            get {
                return ResourceManager.GetString("MissingExportBlobConnection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export settings contain sensitive data but no secret store has been configured..
        /// </summary>
        internal static string MissingSecretStore {
            get {
                return ResourceManager.GetString("MissingSecretStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No identity has been configured..
        /// </summary>
        internal static string MissingServerIdentity {
            get {
                return ResourceManager.GetString("MissingServerIdentity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Public access to Azure Blob Storage containers is not supported..
        /// </summary>
        internal static string PublicBlobStorageConnectionUnsupported {
            get {
                return ResourceManager.GetString("PublicBlobStorageConnectionUnsupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shared Access Signatures (SAS) are not supported..
        /// </summary>
        internal static string SasTokenAuthenticationUnsupported {
            get {
                return ResourceManager.GetString("SasTokenAuthenticationUnsupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Placeholder &apos;{0}&apos; is not recognized..
        /// </summary>
        internal static string UnknownPlaceholder {
            get {
                return ResourceManager.GetString("UnknownPlaceholder", resourceCulture);
            }
        }
    }
}
