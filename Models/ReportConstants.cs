namespace CQCDMS.Models
{
    public static class ReportConstants
    {
        public static readonly Dictionary<string, string> FaxTypeNames = new()
        {
            { "planning_training_operations", "إتجاه التخطيط والتدريب والعمليات" },
            { "needs_technical_affairs", "إتجاه الإحتياجات والشئون الفنية" },
            { "intelligence_modern_systems", "إتجاه الذكاء والأنظمة الحديثة" },
            { "systems_research", "إتجاه النظم والبحوث" },
            { "command_control_mechanisms", "إتجاه ألية القيادة والسيطرة" },
            { "organization_management", "فرع التنظيم والإدارة" },
            { "military_secretariat", "فرع السكرتارية العسكرية" },
            { "officer_affairs", "فرع شئون ضباط" },
            { "information_warfare", "فرع حرب المعلومات" },
            { "development_technical_security", "إتجاه التطوير والتأمين الفني" }
        };

        public static readonly Dictionary<string, string> FaxTypeBadgeClasses = new()
        {
            { "planning_training_operations", "bg-primary" },
            { "needs_technical_affairs", "bg-secondary" },
            { "intelligence_modern_systems", "bg-success" },
            { "systems_research", "bg-warning" },
            { "command_control_mechanisms", "bg-danger" },
            { "organization_management", "bg-info" },
            { "military_secretariat", "bg-dark" },
            { "officer_affairs", "bg-light text-dark" },
            { "information_warfare", "bg-primary" },
            { "development_technical_security", "bg-secondary" }
        };

        public static readonly Dictionary<int, string> MonthNames = new()
        {
            { 1, "يناير" },
            { 2, "فبراير" },
            { 3, "مارس" },
            { 4, "أبريل" },
            { 5, "مايو" },
            { 6, "يونيو" },
            { 7, "يوليو" },
            { 8, "أغسطس" },
            { 9, "سبتمبر" },
            { 10, "أكتوبر" },
            { 11, "نوفمبر" },
            { 12, "ديسمبر" }
        };

        public static readonly Dictionary<string, string> StatusNames = new()
        {
            { "sent", "مُرسل" },
            { "received", "مُستقبل" },
            { "pending", "معلق" },
            { "draft", "مسودة" },
            { "failed", "فشل" }
        };

        public static readonly Dictionary<string, string> ReportTypes = new()
        {
            { "daily", "تقرير يومي" },
            { "monthly", "تقرير شهري" },
            { "status", "تقرير الحالة" },
            { "sender", "تقرير المرسلين" },
            { "type", "تقرير الأنواع" },
            { "custom", "تقرير مخصص" }
        };

        public static readonly Dictionary<string, string> ExportFormats = new()
        {
            { "pdf", "PDF" },
            { "excel", "Excel" },
            { "csv", "CSV" },
            { "json", "JSON" }
        };
    }
}
