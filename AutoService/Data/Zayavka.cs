namespace AutoService.Data
{
    public class Zayavka
    {
        public int Идентификатор { get; set; }

        public string ИмяКлиента { get; set; } = "";

        public string Телефон { get; set; } = "";

        public string НазваниеУслуги { get; set; } = "";

        public string ОписаниеПроблемы { get; set; } = "";

        public string ДатаЗаявки { get; set; } = "";
    }
}
