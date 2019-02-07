open System.Collections.Concurrent
open System.Threading

let asyncMockup s = async {
    do! Async.Sleep 2000
    return sprintf "resulted %s" s
}

let inline (>>=) x f = async.Bind(x, f)
let inline (>>-) x f = async.Bind(x, f >> async.Return)

let requestMasterAsync limit urls =
    let results = ConcurrentBag()
    let mutable running = 0
    let completed = Event<_>()

    let rec loop = function
        | [] -> async.Zero()
        | work::left when running < limit ->
            Interlocked.Increment &running |> ignore
            work |>  asyncMockup
            >>- fun result ->
                results.Add result
                Interlocked.Decrement &running |> ignore
                completed.Trigger()
            |> Async.Start
            loop left
        | left ->
            Async.AwaitEvent completed.Publish
            >>= fun _ -> loop left

    loop (urls |> List.ofSeq) |> Async.Start
    results

let results =
    ["a"; "b"; "b"; "b"; "b"; "b"; "b"; "b"; "b";"b"; "b"]
    |> requestMasterAsync 3

for result in results do
    System.Console.WriteLine result