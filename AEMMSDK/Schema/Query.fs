namespace AEMMSDK.Schema

type Query = {
    PublicationId: string
    EntityType: string
    Q: string
    PageSize: int
    Page: int
    SortField: string
    Descending: bool
}

module QueryModule =

    let query = {
        PublicationId = ""
        EntityType = ""
        Q = ""
        PageSize = 25
        Page = 0
        SortField = "modified"
        Descending = true
    }