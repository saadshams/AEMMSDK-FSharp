namespace AEMMSDK.Tests

open AEMMSDK
open AEMMSDK.Authentication
open AEMMSDK.Content
open AEMMSDK.Schema
open Config
open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Diagnostics
open System.IO
open System.Runtime.Serialization.Json
open System.Text
open System.Text.RegularExpressions

[<TestClass>]
type TestEntity () =

    [<TestMethod>]
    member this.TestRequestMetadataAsync() =
        match requestTokenAsync(Config.client) |> Async.RunSynchronously with
        | Ok token ->
            let schema: Article = {ArticleModule.article with EntityName = "1"; EntityType = "article"; PublicationId = Config.PublicationId; Version = ""}
            match Entity.requestMetadataAsync token (JSON.stringify<Article>(schema)) |> Async.RunSynchronously with
                | Ok result ->
                    let article: Article = JSON.parse<Article>(result)
                    Assert.IsTrue(article.ContentSize = 4251030)
                    Assert.IsTrue(article.Created = "2018-03-20T16:16:48Z")
                    Assert.IsTrue(article.AccessState = "metered")
                    Assert.IsTrue(article.Keywords.Length = 2)
                    Assert.IsTrue(article.Importance = "normal")
                    Assert.IsTrue(article.HideFromBrowsePage = false)
                    Assert.IsTrue(article.IsPublishable = true)
                    Assert.IsTrue(article.ShortTitle = "Tiffany Setting in Platinum")
                    Assert.IsTrue(article.IsTrustedContent = true)
                    Assert.IsTrue(article.AdType = "static")
                    Assert.IsTrue(article.SocialShareUrl = "https://edge-ssp.aemmobile.adobe.com/ssp?entityRef=%2Fpublication%2Fd40a4e4c-d6a3-45ae-98b3-924b31d8712a%2Farticle%2F1")
                    Assert.IsTrue(article.FitToScreen = true)
                    Assert.IsTrue(article.IsAd = false)
                    Assert.IsTrue(article.EntityId = "urn:d40a4e4c-d6a3-45ae-98b3-924b31d8712a:article:1")
                    Assert.IsTrue(article.EntityName = "1")
                    Assert.IsTrue(article.EntityType = "article")
                    Assert.IsTrue(article.PublicationId = "d40a4e4c-d6a3-45ae-98b3-924b31d8712a")
                    Assert.IsTrue(article._Links.ContentUrl.Href = "/publication/d40a4e4c-d6a3-45ae-98b3-924b31d8712a/article/1/contents;contentVersion=1523449049014/")
                    Assert.IsTrue(article._Links.Thumbnail.Width = 1728)
                    Assert.IsTrue(article._Links.Thumbnail.Height = 1080)
                    Assert.IsTrue(article._Links.Thumbnail.Type = "image/jpeg")
                    Assert.IsTrue(article._Links.Thumbnail.Href = "contents/images/thumbnail")
                    article._Links.Thumbnail.DownSamples |> Array.iter(fun downSample -> Assert.IsNotNull(downSample.Size); Assert.IsNotNull(downSample.Href))
                    Assert.IsTrue(article._Links.SocialSharing.Width = 500)
                    Assert.IsTrue(article._Links.SocialSharing.Height = 486)
                    Assert.IsTrue(article._Links.SocialSharing.Type = "image/jpeg")
                    Assert.IsTrue(article._Links.SocialSharing.Href = "contents/images/socialSharing")
                    article._Links.SocialSharing.DownSamples |> Array.iter(fun downSample -> Assert.IsNotNull(downSample.Size); Assert.IsNotNull(downSample.Href))
                    Assert.IsTrue(article._Links.ArticleFolio.Href = "contents/folio/manifest.xml")
                    Assert.IsTrue(article._Links.ArticleFolio.Type = "application/xml")

                | Error error ->
                    Debug.WriteLine("\n" + error.Code + ": " + error.Message + "\n")
                    Assert.IsTrue(false)

        | Error error -> Debug.WriteLine("\n" + error.Code + ": " + error.Message + "\n")
                         Assert.IsTrue(false)

        // let chain = create >> Result.bind update >> Result.bind (upload2 "image.jpg") >> Result.bind publish

    [<TestMethod>]
    member this.TestRequestMetadataInvalid() =
        match requestTokenAsync(Config.client) |> Async.RunSynchronously with
        | Ok token ->
              let schema = {ArticleModule.article with EntityName = "nonexistententity"; EntityType = "article"; PublicationId = Config.PublicationId;}
              match Entity.requestMetadataAsync token (JSON.stringify<Article> schema) |> Async.RunSynchronously with
              | Ok article -> Assert.IsTrue(false)
              | Error error -> Assert.IsTrue(true)
        | Error error -> Assert.IsTrue(false)


    [<TestMethod>]
    member this.TestUpdateMetadataAsync() =
        let schema: Article = {ArticleModule.article with EntityName = "test"; EntityType = "article"; PublicationId = Config.PublicationId; Version = ""}
        match requestTokenAsync Config.client |> Async.RunSynchronously with
        | Ok token ->

            let chain = Entity.requestMetadataAsync token
                     >> AsyncResult.bind (fun (result: string) -> async {
                            let schema = {JSON.parse<Article>(result) with Title = "test1"}
                            return Ok (JSON.stringify<Article>(schema))
                        })
                     >> AsyncResult.bind (Entity.updateMetadataAsync token)

            match chain (JSON.stringify<Article>(schema)) |> Async.RunSynchronously with
            | Ok result ->
                let article = JSON.parse<Article>(result)
                Assert.IsTrue(article.Title = "test1")
            | Error error ->
                Debug.WriteLine("\n" + error.Code + ": " + error.Message + "\n")
                Assert.IsTrue(false)

        | Error error -> Assert.IsTrue(false)


    [<TestMethod>]
    member this.TestRequestListAsync() =
        let schema: Article = {ArticleModule.article with EntityName = "test"; EntityType = "article"; PublicationId = Config.PublicationId;}
        match requestTokenAsync Config.client |> Async.RunSynchronously with
        | Ok token ->
            let query = { QueryModule.query with PublicationId = Config.PublicationId; EntityType = "article"; Q = "" }
            let chain = Entity.requestListAsync token
                     >> AsyncResult.bind (fun (result: string) -> async {
                            let list = JSON.parse<List[]>(result)
                            return Ok (
                                list
                                |> Array.map (fun data ->
                                    let m = Regex.Match(data.Href, "(([a-f0-9]+\-)+[a-f0-9]+)\/(.*?)\/(.*?);version=(\d*)")
                                    {ArticleModule.article with EntityName = m.Groups.[4].Value; EntityType = m.Groups.[3].Value; PublicationId = m.Groups.[1].Value; Version = m.Groups.[5].Value}
                                )
                            )
                        })
                     >> AsyncResult.bind(fun (articles: Article[]) -> async {
                            let result = articles
                                        |> Array.map (fun (article) -> JSON.stringify<Article>(article))
                                        |> Array.chunkBySize 5
                                        |> Array.map (Seq.map (Entity.requestMetadataAsync(token)) >> Async.Parallel >> Async.RunSynchronously)
                                        |> Array.concat
                            return Ok result
                     })

            match chain query |> Async.RunSynchronously with
            | Ok result ->
                Assert.IsNotNull(result)
                result
                    |> Array.iter (
                        fun data ->
                            match data with
                            | Ok schema ->
                                let article = JSON.parse<Article>(schema)
                                Assert.IsNotNull(article.EntityName)
                                Assert.IsNotNull(article.EntityType)
                                Assert.IsNotNull(article.PublicationId)
                                Assert.IsNotNull(article.Version)
                            | Error error ->
                                Console.WriteLine error
                                Assert.IsTrue(false)
                    )
            | Error error ->
                Console.WriteLine error
                Assert.IsTrue(false)
        | Error error -> Assert.IsTrue(false)


    [<TestMethod>]
    member this.requestThumbnailAsync() =
        match requestTokenAsync Config.client |> Async.RunSynchronously with
        | Ok token ->
            let article = {ArticleModule.article with EntityName = "1"; EntityType = "article"; PublicationId = Config.PublicationId}
            match Entity.requestMetadataAsync token (JSON.stringify<Article>(article)) |> Async.RunSynchronously with
            | Ok schema ->
                Assert.IsTrue(true)
                match Entity.requestThumbnailAsync token -1 schema |> Async.RunSynchronously with
                | Ok filename ->
                    Console.WriteLine filename
                | Error error ->
                    Console.WriteLine error
                    Assert.IsTrue(false)
            | Error error ->
                Console.WriteLine error
                Assert.IsTrue(false)
        | Error error ->
            Console.WriteLine error
            Assert.IsTrue(false)

//https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/asynchronous-and-concurrent-programming/async