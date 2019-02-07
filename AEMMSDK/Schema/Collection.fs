namespace AEMMSDK.Schema
open System.Runtime.Serialization

[<DataContract>]
type Collection = {
    [<field: DataMemberAttribute(Name="entityName") >]
    EntityName: string

    [<field: DataMemberAttribute(Name="entityType") >]
    EntityType: string

    [<field: DataMemberAttribute(Name="publicationId") >]
    PublicationId: string

    [<field: DataMemberAttribute(Name="version") >]
    Version: string
}