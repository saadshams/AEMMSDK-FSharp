namespace AEMMSDK.Content

open System

module AsyncResult =

    let bind (binder : 'a -> Async<Result<'b, 'c>>) (a : Async<Result<'a, 'c>>) : Async<Result<'b, 'c>> =
        async {
            let! result = a
            match result with
            | Error e -> return Error e
            | Ok x -> return! binder x
        }

    let compose (f : 'a -> Async<Result<'b, 'e>>) (g : 'b -> Async<Result<'c, 'e>>) = fun x -> bind g (f x)
    let (>>=) a f = bind f a
    let (>=>) f g = compose f g

