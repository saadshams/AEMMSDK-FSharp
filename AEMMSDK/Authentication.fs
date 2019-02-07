namespace AEMMSDK

open AEMMSDK.Schema
open System
open System.IO
open System.Net
open System.Runtime.Serialization.Json
open System.Text

module Authentication =

    let requestTokenAsync (client : Client) : Async<Result<Token, Error>> =
        let url =
            sprintf
                "https://ims-na1.adobelogin.com/ims/token/v1?grant_type=device&scope=AdobeID,openid&client_id=%s&client_secret=%s&device_id=%s&device_token=%s"
                client.Id client.Secret client.DeviceId client.DeviceToken

        let request = WebRequest.CreateHttp url
        request.Method <- "POST"
        request.ContentType <- "application/x-www-form-urlencoded"
        request.Accept <- "application/json;charset=UTF-8"

        async {
            try
                use! response = request.AsyncGetResponse()
                use streamReader = new StreamReader(response.GetResponseStream())
                let! body = streamReader.ReadToEndAsync() |> Async.AwaitTask
                use memoryStream = new MemoryStream(ASCIIEncoding.Default.GetBytes(body))
                let token = ((new DataContractJsonSerializer(typeof<Token>)).ReadObject(memoryStream) :?> Token)
                return Ok { token with Client = client
                                       SessionId = Guid.NewGuid().ToString() }
            with :? WebException as webException ->
                use streamReader = new StreamReader(webException.Response.GetResponseStream())
                let! body = streamReader.ReadToEndAsync() |> Async.AwaitTask
                use memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body))
                return Error((new DataContractJsonSerializer(typeof<Error>)).ReadObject(memoryStream) :?> Error)
        }