create table if not exists bookmark
(
    id          integer generated always as identity
        constraint bookmark_pk
            primary key,
    url         varchar(5000) not null,
    title       varchar(500),
    description varchar(5000)
);



