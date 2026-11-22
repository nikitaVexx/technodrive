namespace AutoService.Data
{
    public class ZayavkaView
    {
        public int Идентификатор { get; set; }
        public string ИмяКлиента { get; set; } = "";
        public string Телефон { get; set; } = "";
        public string НазваниеУслуги { get; set; } = "";
        public string ОписаниеПроблемы { get; set; } = "";
        public string ДатаЗаявки { get; set; } = "";
        public string ДатаОбслуживания { get; set; } = "";
        public string ВремяОбслуживания { get; set; } = "";
        public string Статус { get; set; } = "Новая";
    }
}