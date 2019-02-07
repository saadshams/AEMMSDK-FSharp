namespace AEMMSDK.Schema

open System.Runtime.Serialization

[<DataContract>]
type List = {
    [<field: DataMemberAttribute(Name="href")>]
    Href: string
}