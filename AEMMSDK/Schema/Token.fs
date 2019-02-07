namespace AEMMSDK.Schema

open System.Runtime.Serialization

[<DataContract>]
type Token = {
    [<field: DataMemberAttribute(Name="access_token") >]
    AccessToken: string

    [<field: DataMemberAttribute(Name="token_type") >]
    TokenType: string

    [<field: DataMemberAttribute(Name="expires_in") >]
    ExpiresIn: double

    Client: Client

    SessionId: string
}