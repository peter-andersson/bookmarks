create table if not exists bookmark_tag
(
    bookmark_id integer not null
        constraint bookmark_tag_bookmark_id_fk
            references bookmark,
    tag_id      integer not null
        constraint bookmark_tag_tag_id_fk
            references tag,
    constraint bookmark_tag_pk
        primary key (bookmark_id, tag_id)
);

