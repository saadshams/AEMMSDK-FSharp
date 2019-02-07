namespace AEMMSDK.Content

open AEMMSDK
open AEMMSDK.Schema
open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Runtime.Serialization.Json
open System.Text
open System.Text.RegularExpressions

module Entity =

    let createAsync (token: Token) (schema: string) : Async<Result<string, Error>> =
        let document = JSON.parse<Article> (schema)
        let url = sprintf "https://pecs.publish.adobe.io/publication/%s/%s/%s" document.PublicationId document.EntityType document.EntityName

        let request = WebRequest.CreateHttp(Uri url)
        request.Method <- "PUT"
        request.Accept <- "application/json;charset=UTF-8"
        request.Headers.["Authorization"] <- token.TokenType + " " + token.AccessToken
        request.Headers.["x-dps-client-session-id"] <- token.SessionId
        request.Headers.["x-dps-client-request-id"] <- Guid.NewGuid().ToString()
        request.Headers.["x-dps-api-key"] <- token.Client.Id

        async {
            try
                use! response = request.AsyncGetResponse()
                use stream = response.GetResponseStream()
                use streamReader = new StreamReader(stream)
                return Ok(streamReader.ReadToEnd())
            with :? WebException as e ->
                match e.Response with
                | :? HttpWebResponse ->
                        use response = e.Response :?> HttpWebResponse
                        use stream = response.GetResponseStream()
                        use streamReader = new StreamReader(stream)
                        use memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd()))
                        let serializer = new DataContractJsonSerializer(typeof<Error>)
                        return Error { (serializer.ReadObject(memoryStream) :?> Error) with Status = int(response.StatusCode) }
                | _ -> return Error { Code = "0"; Message = "Check your connection and try again."; Status = 0 }
        }

    let requestMetadataAsync (token : Token) (schema : string) : Async<Result<string, Error>> =
        let document = JSON.parse<Article> (schema)

        let url = if String.IsNullOrEmpty(document.Version) then
                    sprintf "https://pecs.publish.adobe.io/publication/%s/%s/%s" document.PublicationId document.EntityType document.EntityName else
                    sprintf "https://pecs.publish.adobe.io/publication/%s/%s/%s;version=%s" document.PublicationId document.EntityType document.EntityName document.Version

        let request = WebRequest.CreateHttp(Uri url)
        request.Proxy <- WebProxy("localhost", 8888)
        request.Method <- "GET"
        request.Accept <- "application/json;charset=UTF-8"
        request.Headers.["Authorization"] <- token.TokenType + " " + token.AccessToken
        request.Headers.["x-dps-client-session-id"] <- token.SessionId
        request.Headers.["x-dps-client-request-id"] <- Guid.NewGuid().ToString()
        request.Headers.["x-dps-api-key"] <- token.Client.Id

        async {
            try
                use! response = request.AsyncGetResponse()
                use stream = response.GetResponseStream()
                use streamReader = new StreamReader(stream)
                return Ok(streamReader.ReadToEnd())
            with :? WebException as e ->
                match e.Response with
                | :? HttpWebResponse ->
                        use response = e.Response :?> HttpWebResponse
                        use stream = response.GetResponseStream()
                        use streamReader = new StreamReader(stream)
                        use memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd()))
                        let serializer = new DataContractJsonSerializer(typeof<Error>)
                        return Error { (serializer.ReadObject(memoryStream) :?> Error) with Status = int(response.StatusCode) }
                | _ -> return Error { Code = "0"; Message = "Check your connection and try again."; Status = 0 }
        }

    let updateMetadataAsync (token : Token) (schema : string) : Async<Result<string, Error>> =
        let document = JSON.parse<Article> (schema)
        let url =
            sprintf "https://pecs.publish.adobe.io/publication/%s/%s/%s;version=%s" document.PublicationId
                document.EntityType document.EntityName document.Version

        let request = WebRequest.CreateHttp url
        request.Method <- "PUT"
        request.ContentType <- "application/json;charset=UTF-8"
        request.Accept <- "application/json;charset=UTF-8"
        request.Headers.["Authorization"] <- token.TokenType + " " + token.AccessToken
        request.Headers.["x-dps-client-session-id"] <- token.SessionId
        request.Headers.["x-dps-client-request-id"] <- Guid.NewGuid().ToString()
        request.Headers.["x-dps-api-key"] <- token.Client.Id

        async {
            request.ContentLength <- (int64) schema.Length
            use! requestStream = request.GetRequestStreamAsync() |> Async.AwaitTask
            requestStream.Write(Encoding.UTF8.GetBytes(schema), 0, schema.Length)
            requestStream.Close()

            try
                use! response = request.AsyncGetResponse()
                use stream = response.GetResponseStream()
                use streamReader = new StreamReader(stream)
                let! data = streamReader.ReadToEndAsync() |> Async.AwaitTask
                return Ok data
            with :? WebException as e ->
                match e.Response with
                | :? HttpWebResponse ->
                        use response = e.Response :?> HttpWebResponse
                        use stream = response.GetResponseStream()
                        use streamReader = new StreamReader(stream)
                        use memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd()))
                        let serializer = new DataContractJsonSerializer(typeof<Error>)
                        return Error { (serializer.ReadObject(memoryStream) :?> Error) with Status = int(response.StatusCode) }
                | _ -> return Error { Code = "0"; Message = "Check your connection and try again."; Status = 0 }
        }

    let requestListAsync (token : Token) (query : Query) : Async<Result<string, Error>> =

        let url =
            sprintf
                "https://pecs.publish.adobe.io/publication/%s/%s?q=%s&pageSize=%i&page=%i&sortField=%s&descending=%b"
                query.PublicationId query.EntityType query.Q query.PageSize query.Page query.SortField query.Descending

        let request = WebRequest.CreateHttp url
        request.Method <- "GET"
        request.Accept <- "application/json;charset=UTF-8"
        request.Headers.["Authorization"] <- token.TokenType + " " + token.AccessToken
        request.Headers.["x-dps-client-session-id"] <- token.SessionId
        request.Headers.["x-dps-client-request-id"] <- Guid.NewGuid().ToString()
        request.Headers.["x-dps-api-key"] <- token.Client.Id

        async {
            try
                use response = request.GetResponse() :?> HttpWebResponse
                use stream = response.GetResponseStream()
                use reader = new StreamReader(stream)
                return Ok (reader.ReadToEnd())
            with :? WebException as e ->
            match e.Response with
            | :? HttpWebResponse ->
                use response = e.Response :?> HttpWebResponse
                use stream = response.GetResponseStream()
                use streamReader = new StreamReader(stream)
                use memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd()))
                let serializer = new DataContractJsonSerializer(typeof<Error>)
                return Error { (serializer.ReadObject(memoryStream) :?> Error) with Status = int(response.StatusCode) }
            | _ -> return Error { Code = "0"; Message = "Check your connection and try again."; Status = 0 }
        }

    let requestThumbnailAsync (token: Token) (size: int) (schema: string) : Async<Result<string, Error>> =
        let document = JSON.parse<Article> (schema)

        let url = if size = -1 then
                    sprintf "https://pecs.publish.adobe.io%s/images/thumbnail" document._Links.ContentUrl.Href else
                    sprintf "https://pecs.publish.adobe.io%s/images/thumbnail?size=%d" document._Links.ContentUrl.Href size

        let request = WebRequest.CreateHttp url
        request.Proxy <- WebProxy("localhost", 8888)
        request.Method <- "GET"
        request.Accept <- "application/json;charset=UTF-8"
        request.Headers.["Authorization"] <- token.TokenType + " " + token.AccessToken
        request.Headers.["x-dps-client-session-id"] <- token.SessionId
        request.Headers.["x-dps-client-request-id"] <- Guid.NewGuid().ToString()
        request.Headers.["x-dps-api-key"] <- token.Client.Id

        async {
            try
                use response = request.GetResponse() :?> HttpWebResponse
                use stream = response.GetResponseStream()
                let buffer: byte[] = Array.zeroCreate 1024
                use reader = new BinaryReader(stream)
                use fileStream = new FileStream(Path.GetTempFileName(), FileMode.Open)

                let mutable bytesRead = -1
                while not (bytesRead = 0) do
                    bytesRead <- reader.Read(buffer, 0, 1024)
                    if bytesRead > 0 then fileStream.Write(buffer, 0, bytesRead)

                return Ok fileStream.Name
            with :? WebException as e ->
                use reader = new StreamReader(e.Response.GetResponseStream())
                use memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd()))
                return Error ((new DataContractJsonSerializer(typeof<Error>)).ReadObject(memoryStream) :?> Error)
        }

