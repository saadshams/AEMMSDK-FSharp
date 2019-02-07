namespace AEMMSDK.Tests

open AEMMSDK.Authentication
open AEMMSDK.Schema
open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Diagnostics
open System.Threading.Tasks

[<TestClass>]
type TestAuthentication() =

    [<TestMethod>]
    member this.TestRequestTokenAsync() =
        match requestTokenAsync (Config.client) |> Async.RunSynchronously with
        | Ok token ->
            Assert.IsTrue(true)
            Assert.IsTrue(token.AccessToken <> "")
            Assert.IsTrue(token.TokenType <> "")
            Assert.IsTrue(token.ExpiresIn <> 0.)
        | Error error ->
            Debug.WriteLine("\n" + error.Code + ": " + error.Message + "\n")
            Assert.IsFalse(false)
