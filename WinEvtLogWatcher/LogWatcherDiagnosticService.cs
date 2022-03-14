using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    class LogWatcherDiagnosticService : ILogWatcherDiagnosticService
    {
        private readonly PingService _pingService;
        private readonly IApplicationLogger _logger;
        public LogWatcherDiagnosticService(PingService pingService, IApplicationLogger logger)
        {
            _pingService = pingService ?? throw new ArgumentNullException(nameof(pingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private List<DiagnosticResult> _prevDiagnoseResults = null;
        
        /// <summary>
        /// Метод выполняющий диагностику связи с компьютерами
        /// </summary>
        /// <param name="computers">Компьютеры, с которыми необходимо продиагностировать связь</param>
        /// <returns>Возвращает лист кортежей вида - (компьютер - флаг о необходимости пересоздания вотчера)</returns>
        public async Task<List<DiagnosticResult>> Diagnose(IEnumerable<Computer> computers)
        {
            var taskList = new List<Task<bool>>();
            foreach (var computer in computers)
                taskList.Add(_pingService.TryPingHostAsync(computer.IpAddress));
            await Task.WhenAll(taskList);


            var diagResults = new List<DiagnosticResult>();

            // результаты проверки
            int index = 0;
            foreach (var computer in computers)
            {
                var currDiagResult = new DiagnosticResult(
                    computer,
                    _prevDiagnoseResults != null && !_prevDiagnoseResults[index].IsConnected && await taskList[index],
                    await taskList[index]
                    );
                diagResults.Add(currDiagResult);
                index++;
            }
            _prevDiagnoseResults = diagResults;

            return diagResults;
        }
    }

    class DiagnosticResult
    {
        public readonly Computer Computer;
        public readonly bool IsNeedResub;
        public readonly bool IsConnected;

        public DiagnosticResult(Computer computer, bool isNeedResub, bool isConnected)
        {
            Computer = computer;
            IsNeedResub = isNeedResub;
            IsConnected = isConnected;
        }
    }
}
