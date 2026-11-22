using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
namespace AutoService.Data
{
    public class Database
    {
        private readonly string _connectionString =
            "Data Source=Data/autoservice.db";

        public void Initialize()
        {

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Услуги (
                    Идентификатор INTEGER PRIMARY KEY AUTOINCREMENT,
                    Название TEXT NOT NULL,
                    Описание TEXT,
                    Стоимость REAL NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Заявки (
                    Идентификатор INTEGER PRIMARY KEY AUTOINCREMENT,
                    ИмяКлиента TEXT NOT NULL,
                    Телефон TEXT NOT NULL,
                    ИдентификаторУслуги INTEGER,
                    ОписаниеПроблемы TEXT,
                    ДатаЗаявки TEXT NOT NULL,
                    ДатаОбслуживания TEXT,
                    ВремяОбслуживания TEXT,
                    Статус TEXT NOT NULL DEFAULT 'Новая',
                    FOREIGN KEY (ИдентификаторУслуги)
                        REFERENCES Услуги(Идентификатор)
                );
            ";

            command.ExecuteNonQuery();
            using var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
    INSERT INTO Услуги (Название, Описание, Стоимость)
    SELECT $название, $описание, $стоимость
    WHERE NOT EXISTS (
        SELECT 1 FROM Услуги WHERE Название = $название
    );

    INSERT INTO Услуги (Название, Описание, Стоимость)
    SELECT $название2, $описание2, $стоимость2
    WHERE NOT EXISTS (
        SELECT 1 FROM Услуги WHERE Название = $название2
    );

    INSERT INTO Услуги (Название, Описание, Стоимость)
    SELECT $название3, $описание3, $стоимость3
    WHERE NOT EXISTS (
        SELECT 1 FROM Услуги WHERE Название = $название3
    );

    INSERT INTO Услуги (Название, Описание, Стоимость)
    SELECT $название4, $описание4, $стоимость4
    WHERE NOT EXISTS (
        SELECT 1 FROM Услуги WHERE Название = $название4
    );

    INSERT INTO Услуги (Название, Описание, Стоимость)
    SELECT $название5, $описание5, $стоимость5
    WHERE NOT EXISTS (
        SELECT 1 FROM Услуги WHERE Название = $название5
    );

    INSERT INTO Услуги (Название, Описание, Стоимость)
    SELECT $название6, $описание6, $стоимость6
    WHERE NOT EXISTS (
        SELECT 1 FROM Услуги WHERE Название = $название6
    );
";

            insertCommand.Parameters.AddWithValue("$название", "Диагностика автомобиля");
            insertCommand.Parameters.AddWithValue("$описание", "Проверка основных систем автомобиля для выявления неисправностей.");
            insertCommand.Parameters.AddWithValue("$стоимость", 1500);

            insertCommand.Parameters.AddWithValue("$название2", "Замена масла");
            insertCommand.Parameters.AddWithValue("$описание2", "Замена моторного масла и масляного фильтра.");
            insertCommand.Parameters.AddWithValue("$стоимость2", 2000);

            insertCommand.Parameters.AddWithValue("$название3", "Ремонт тормозной системы");
            insertCommand.Parameters.AddWithValue("$описание3", "Осмотр и ремонт тормозов, замена колодок и дисков.");
            insertCommand.Parameters.AddWithValue("$стоимость3", 3000);

            insertCommand.Parameters.AddWithValue("$название4", "Ремонт подвески");
            insertCommand.Parameters.AddWithValue("$описание4", "Проверка и устранение неисправностей ходовой части автомобиля.");
            insertCommand.Parameters.AddWithValue("$стоимость4", 2500);

            insertCommand.Parameters.AddWithValue("$название5", "Шиномонтаж");
            insertCommand.Parameters.AddWithValue("$описание5", "Снятие и установка колёс, балансировка и проверка шин.");
            insertCommand.Parameters.AddWithValue("$стоимость5", 2000);

            insertCommand.Parameters.AddWithValue("$название6", "Компьютерная диагностика");
            insertCommand.Parameters.AddWithValue("$описание6", "Считывание кодов ошибок электронных систем автомобиля.");
            insertCommand.Parameters.AddWithValue("$стоимость6", 1000);

            insertCommand.ExecuteNonQuery();
            using var checkCommand = connection.CreateCommand();
            checkCommand.CommandText =
                "PRAGMA table_info('Заявки');";

            bool столбецЕсть = false;

            using (var reader = checkCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString() == "ОписаниеПроблемы")
                    {
                        столбецЕсть = true;
                        break;
                    }
                }
            }

            if (!столбецЕсть)
            {
                using var alterCommand = connection.CreateCommand();
                alterCommand.CommandText =
                    "ALTER TABLE Заявки ADD COLUMN ОписаниеПроблемы TEXT;";
                alterCommand.ExecuteNonQuery();
            }
            string[] новыеСтолбцы =
{
    "ДатаОбслуживания",
    "ВремяОбслуживания",
    "Статус"
};

            foreach (string столбец in новыеСтолбцы)
            {
                using var проверка = connection.CreateCommand();
                проверка.CommandText = "PRAGMA table_info('Заявки');";

                bool существует = false;

                using (var reader = проверка.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["name"].ToString() == столбец)
                        {
                            существует = true;
                            break;
                        }
                    }
                }

                if (!существует)
                {
                    using var изменение = connection.CreateCommand();

                    if (столбец == "Статус")
                    {
                        изменение.CommandText =
                            "ALTER TABLE Заявки ADD COLUMN Статус TEXT NOT NULL DEFAULT 'Новая';";
                    }
                    else
                    {
                        изменение.CommandText =
                            $"ALTER TABLE Заявки ADD COLUMN {столбец} TEXT;";
                    }

                    изменение.ExecuteNonQuery();
                }
            }

            using var индекс = connection.CreateCommand();
            индекс.CommandText = @"
                CREATE UNIQUE INDEX IF NOT EXISTS IX_Zayavki_Date_Time
                ON Заявки (ДатаОбслуживания, ВремяОбслуживания)
                WHERE ДатаОбслуживания IS NOT NULL
                  AND ВремяОбслуживания IS NOT NULL
                  AND ДатаОбслуживания <> ''
                  AND ВремяОбслуживания <> '';
            ";
            индекс.ExecuteNonQuery();


        }
        public List<Usluga> GetServices()
        {
            var услуги = new List<Usluga>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT Идентификатор, Название, Описание, Стоимость
        FROM Услуги
        ORDER BY Идентификатор;
    ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                услуги.Add(new Usluga
                {
                    Идентификатор = reader.GetInt32(0),
                    Название = reader.GetString(1),
                    Описание = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Стоимость = reader.GetDouble(3)
                });
            }

            return услуги;
        }

        public bool AddRequest(
    string имяКлиента,
    string телефон,
    string описаниеПроблемы,
    int идентификаторУслуги,
    string датаОбслуживания,
    string времяОбслуживания)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
        INSERT INTO Заявки
        (
            ИмяКлиента,
            Телефон,
            ИдентификаторУслуги,
            ОписаниеПроблемы,
            ДатаЗаявки,
            ДатаОбслуживания,
            ВремяОбслуживания,
            Статус
        )
        VALUES
        (
            $имя,
            $телефон,
            $услуга,
            $описание,
            $датаЗаявки,
            $датаОбслуживания,
            $времяОбслуживания,
            'Новая'
        );
    ";

            command.Parameters.AddWithValue("$имя", имяКлиента);
            command.Parameters.AddWithValue("$телефон", телефон);
            command.Parameters.AddWithValue("$услуга", идентификаторУслуги);
            command.Parameters.AddWithValue("$описание", описаниеПроблемы);
            command.Parameters.AddWithValue(
                "$датаЗаявки",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("$датаОбслуживания", датаОбслуживания);
            command.Parameters.AddWithValue("$времяОбслуживания", времяОбслуживания);

            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                return false;
            }
        }
        public List<ZayavkaView> GetRequests()
        {
            var заявки = new List<ZayavkaView>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT
                    Заявки.Идентификатор,
                    Заявки.ИмяКлиента,
                    Заявки.Телефон,
                    Услуги.Название AS НазваниеУслуги,
                    Заявки.ОписаниеПроблемы,
                    Заявки.ДатаЗаявки,
                    Заявки.ДатаОбслуживания,
                    Заявки.ВремяОбслуживания,
                    Заявки.Статус
                FROM Заявки
                LEFT JOIN Услуги
                    ON Заявки.ИдентификаторУслуги = Услуги.Идентификатор
                ORDER BY Заявки.Идентификатор DESC;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                заявки.Add(new ZayavkaView
                {
                    Идентификатор = reader.GetInt32(0),
                    ИмяКлиента = reader.GetString(1),
                    Телефон = reader.GetString(2),
                    НазваниеУслуги = reader.IsDBNull(3) ? "Услуга не найдена" : reader.GetString(3),
                    ОписаниеПроблемы = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    ДатаЗаявки = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    ДатаОбслуживания = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    ВремяОбслуживания = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Статус = reader.IsDBNull(8) ? "Новая" : reader.GetString(8)
                });
            }

            return заявки;
        }




        public bool CancelRequest(int идентификатор, string телефон)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Заявки
                SET Статус = 'Отменена'
                WHERE Идентификатор = $идентификатор
                  AND Телефон = $телефон
                  AND Статус NOT IN ('Отменена', 'Завершена');";
            command.Parameters.AddWithValue("$идентификатор", идентификатор);
            command.Parameters.AddWithValue("$телефон", телефон);

            return command.ExecuteNonQuery() > 0;
        }

        public void DeleteRequest(int идентификатор)
        {
            using var соединение = new SqliteConnection(
                "Data Source=Data/autoservice.db");

            соединение.Open();

            using var команда = соединение.CreateCommand();
            команда.CommandText = @"
        DELETE FROM Заявки
        WHERE Идентификатор = $идентификатор";

            команда.Parameters.AddWithValue(
                "$идентификатор", идентификатор);

            команда.ExecuteNonQuery();
        }

    }

}
