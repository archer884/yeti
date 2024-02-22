#![allow(non_snake_case)]

// @generated automatically by Diesel CLI.

diesel::table! {
    Fragments (Id) {
        Id -> Int8,
        WriterId -> Int8,
        ManuscriptId -> Int8,
        Heading -> Nullable<Text>,
        Content -> Text,
        SortBy -> Float8,
        SoftDelete -> Bool,
        Created -> Timestamptz,
        Updated -> Timestamptz,
    }
}

diesel::table! {
    ManuscriptTag (ManuscriptsId, TagsId) {
        ManuscriptsId -> Int8,
        TagsId -> Int8,
    }
}

diesel::table! {
    Manuscripts (Id) {
        Id -> Int8,
        WriterId -> Int8,
        Title -> Text,
        Blurb -> Nullable<Text>,
        SoftDelete -> Bool,
        Created -> Timestamptz,
        Updated -> Timestamptz,
    }
}

diesel::table! {
    Tags (Id) {
        Id -> Int8,
        Value -> Text,
    }
}

diesel::table! {
    Writers (Id) {
        Id -> Int8,
        Username -> Text,
        AuthorName -> Nullable<Text>,
    }
}

diesel::joinable!(Fragments -> Manuscripts (ManuscriptId));
diesel::joinable!(Fragments -> Writers (WriterId));
diesel::joinable!(ManuscriptTag -> Manuscripts (ManuscriptsId));
diesel::joinable!(ManuscriptTag -> Tags (TagsId));
diesel::joinable!(Manuscripts -> Writers (WriterId));

diesel::allow_tables_to_appear_in_same_query!(Fragments, ManuscriptTag, Manuscripts, Tags, Writers,);
