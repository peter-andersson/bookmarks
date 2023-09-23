create table if not exists version_info
(
    version     integer       not null
        constraint version_info_pk
            primary key,
    applied     timestamp     not null,
    description varchar(1000) not null
);

