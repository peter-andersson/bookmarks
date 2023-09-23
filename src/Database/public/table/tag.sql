create table if not exists tag
(
    id   integer generated always as identity
        constraint tag_pk
            primary key,
    name varchar(50) not null
);

