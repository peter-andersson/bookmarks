using Npgsql;

namespace Bookmarks.Data.Migrations;

public class Migration001 : IDatabaseMigration
{
    public async Task ApplyMigration(NpgsqlConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
                              create table if not exists version_info
                              (
                                  version     integer       not null
                                      constraint version_info_pk
                                          primary key,
                                  applied     timestamp     not null,
                                  description varchar(1000) not null
                              )
                              """;
        await command.ExecuteNonQueryAsync();

        command.CommandText = """
                              create table if not exists bookmark
                              (
                                  id integer generated always as identity
                                      constraint bookmark_pk
                                          primary key,
                                  url         varchar(5000) not null,
                                  title       varchar(500),
                                  description varchar(5000)
                              )
                              """;
        await command.ExecuteNonQueryAsync();

        command.CommandText = """
                              create table if not exists tag
                              (
                                  id   integer generated always as identity
                                      constraint tag_pk
                                          primary key,
                                  name varchar(50) not null
                              )
                              """;
        await command.ExecuteNonQueryAsync();

        command.CommandText = """
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
                              )
                              """;
        await command.ExecuteNonQueryAsync();
    }
}