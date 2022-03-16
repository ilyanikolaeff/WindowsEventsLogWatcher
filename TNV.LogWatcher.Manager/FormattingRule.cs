using DevExpress.Mvvm;
using DevExpress.Xpf.Core.ConditionalFormatting;

namespace TNV.LogWatcher.Manager.ViewModels
{
    public class FormattingRule : BindableBase
    {
        public FormattingRule(string fieldName, ConditionRule conditionRule, int value, bool applyToRow, Format format)
        {
            FieldName = fieldName;
            ValueRule = conditionRule;
            Value = value;
            ApplyToRow = applyToRow;
            Format = format;
        }

        public string FieldName { get; }
        public ConditionRule ValueRule { get; }
        public int Value { get; }
        public bool ApplyToRow { get; }
        public Format Format { get; }
    }
}
