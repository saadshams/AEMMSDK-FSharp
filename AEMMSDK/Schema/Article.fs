namespace AEMMSDK.Schema

open System.Runtime.Serialization

[<DataContract>]
type ContentUrl = {
    [<field: DataMemberAttribute(Name="href")>]
    Href: string
}

[<DataContract>]
type AltAssetUrl = {
    [<field: DataMemberAttribute(Name="href")>]
    Href: string

    [<field: DataMemberAttribute(Name="expirationTime")>]
    ExpirationTime: int64
}

[<DataContract>]
type DownSample = {
    [<field: DataMemberAttribute(Name="size")>]
    Size: int

    [<field: DataMemberAttribute(Name="href")>]
    Href: string
}

[<DataContract>]
type Thumbnail = {
    [<field: DataMemberAttribute(Name="width")>]
    Width: int

    [<field: DataMemberAttribute(Name="height")>]
    Height: int

    [<field: DataMemberAttribute(Name="type")>]
    Type: string

    [<field: DataMemberAttribute(Name="href")>]
    Href: string

    [<field: DataMemberAttribute(Name="downSamples")>]
    DownSamples: DownSample[]
}

[<DataContract>]
type SocialSharing = {
    [<field: DataMemberAttribute(Name="width")>]
    Width: int

    [<field: DataMemberAttribute(Name="height")>]
    Height: int

    [<field: DataMemberAttribute(Name="type")>]
    Type: string

    [<field: DataMemberAttribute(Name="href")>]
    Href: string

    [<field: DataMemberAttribute(Name="downSamples")>]
    DownSamples: DownSample[]
}

[<DataContract>]
type ArticleFolio = {
    [<field: DataMemberAttribute(Name="href")>]
    Href: string

    [<field: DataMemberAttribute(Name="type")>]
    Type: string
}

[<DataContract>]
type Links = {
    [<field: DataMemberAttribute(Name="contentUrl")>]
    ContentUrl: ContentUrl

    [<field: DataMemberAttribute(Name="altAssetUrl")>]
    AltAssetUrl: AltAssetUrl

    [<field: DataMemberAttribute(Name="thumbnail")>]
    Thumbnail: Thumbnail

    [<field: DataMemberAttribute(Name="socialSharing")>]
    SocialSharing: SocialSharing

    [<field: DataMemberAttribute(Name="articleFolio")>]
    ArticleFolio: ArticleFolio
}

[<DataContract>]
type Article = {
    [<field: DataMemberAttribute(Name="contentSize")>]
    ContentSize: int

    [<field: DataMemberAttribute(Name="created")>]
    Created: string

    [<field: DataMemberAttribute(Name="accessState")>]
    AccessState: string

    [<field: DataMemberAttribute(Name="keywords")>]
    Keywords: string[]

    [<field: DataMemberAttribute(Name="importance")>]
    Importance: string

    [<field: DataMemberAttribute(Name="hideFromBrowsePage")>]
    HideFromBrowsePage: bool

    [<field: DataMemberAttribute(Name="isPublishable")>]
    IsPublishable: bool

    [<field: DataMemberAttribute(Name="shortTitle")>]
    ShortTitle: string

    [<field: DataMemberAttribute(Name="isTrustedContent")>]
    IsTrustedContent: bool

    [<field: DataMemberAttribute(Name="adType")>]
    AdType: string

    [<field: DataMemberAttribute(Name="socialShareUrl")>]
    SocialShareUrl: string

    [<field: DataMemberAttribute(Name="fitToScreen")>]
    FitToScreen: bool

    [<field: DataMemberAttribute(Name="title")>]
    Title: string

    [<field: DataMemberAttribute(Name="isAd")>]
    IsAd: bool

    [<field: DataMemberAttribute(Name="entityId")>]
    EntityId: string

    [<field: DataMemberAttribute(Name="entityName")>]
    EntityName: string

    [<field: DataMemberAttribute(Name="entityType")>]
    EntityType: string

    [<field: DataMemberAttribute(Name="publicationId")>]
    PublicationId: string

    [<field: DataMemberAttribute(Name="version")>]
    Version: string

    [<field: DataMemberAttribute(Name="_links")>]
    _Links: Links
}

module ArticleModule =

    let article = {
        ContentSize = -1
        Created = ""
        AccessState = "metered"
        Keywords = [||]
        ShortTitle = ""
        IsTrustedContent = false
        AdType = "static"
        SocialShareUrl = ""
        Importance = "normal"
        HideFromBrowsePage = false
        IsPublishable = false
        FitToScreen = true
        Title = ""
        IsAd = false
        EntityId = ""
        EntityName = ""
        EntityType = ""
        PublicationId = ""
        Version = ""
        _Links = {
                    ContentUrl = {Href = ""}
                    AltAssetUrl = {Href = ""; ExpirationTime = 0L}
                    Thumbnail = {Width = 0; Height = 0; Type=""; Href = ""; DownSamples = [||]}
                    SocialSharing = {Width = 0; Height = 0; Type=""; Href = ""; DownSamples = [||]}
                    ArticleFolio = {Href = ""; Type=""}
        }
    }


// _links -> socialSharing, collections