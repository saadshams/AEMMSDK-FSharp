namespace AEMMSDK.Schema

open System.Runtime.Serialization
open System
open System.Net

[<DataContract>]
type Error = {

    [<field: DataMemberAttribute(Name="code")>]
    Code: string

    [<field: DataMemberAttribute(Name="message")>]
    Message: string

    Status: int
}

// https://stackoverflow.com/questions/49345307/multiple-datamemberattribute/49351927#49351927
//
//    [<field: DataMemberAttribute(Name="error_code")>]
//    error_code: string

//with
//    member this.Code : string =
//        if not (String.IsNullOrEmpty(this.code)) then this.code
//        else if not (String.IsNullOrEmpty(this.error_code)) then this.error_code
//        else ""
//
//    member this.Message : string =
//        if not (String.IsNullOrEmpty(this.message)) then this.message
//        else if not (String.IsNullOrEmpty(this.error)) then this.error
//        else ""


//
//    [<field: DataMemberAttribute(Name="error")>]
//    Error: string
