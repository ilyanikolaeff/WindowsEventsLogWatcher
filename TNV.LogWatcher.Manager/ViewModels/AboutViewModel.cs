using System.Text;

namespace TNV.LogWatcher.Manager.ViewModels
{
    class AboutViewModel
    {
        public string AboutText { get; private set; }
        public AboutViewModel()
        {
            var sbBuilder = new StringBuilder();
            sbBuilder.Append("Данное программное обеспечение принадлежит ООО Транснефть-Восток\n");
            sbBuilder.Append("Разработка: Николаев И.К. (сектор ИТС, ОАСУТП)\n");
            sbBuilder.Append("Бесплатно для использования в организациях системы Транснефть\n");

            AboutText = sbBuilder.ToString();
        }
    }
}
