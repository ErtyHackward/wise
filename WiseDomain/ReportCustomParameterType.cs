namespace WiseDomain
{
    public enum ReportCustomParameterType
    {
        Check,
        CustomString,
        Enum,
    }

    public static class EnumToLabel
    {
        public static string ToLabel(this ReportCustomParameterType par)
        {
            switch (par)
            {
                case ReportCustomParameterType.Check: return "Чек-бокс";
                case ReportCustomParameterType.CustomString: return "Строка";
                case ReportCustomParameterType.Enum: return "Выбор";
                default: return "(добавь описание в класс EnumToLabel)";
            }
        }
    }
}