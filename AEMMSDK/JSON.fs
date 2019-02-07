namespace AEMMSDK

open System.IO
open System.Runtime.Serialization.Json
open System.Xml
open System.Text

module JSON =

    let stringify<'T> (data:'t) =
        use memoryStream = new MemoryStream()
        (new DataContractJsonSerializer(typeof<'T>)).WriteObject(memoryStream, data)
        Encoding.Default.GetString(memoryStream.ToArray())

    let parse<'T> (json:string) : 'T =
        use memoryStream = new MemoryStream(ASCIIEncoding.Default.GetBytes(json))
        let data = (new DataContractJsonSerializer(typeof<'T>)).ReadObject(memoryStream)
        data :?> 'T