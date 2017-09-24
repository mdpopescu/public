using WindowsFormsApp2.Models;

namespace WindowsFormsApp2.Contracts
{
    public interface IOutput
    {
        void Set(LabeledValue labeledValue);
    }
}