using System.Data;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.RU
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU")]
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "MailRuSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Odnoklassniki")]
    public class OdnoklassnikiMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "OdnoklassnikiSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("ClientPublicId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.VKontakte")]
    public class VKontakteMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "VKontakteSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Yandex")]
    public class YandexMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "YandexSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }
}
