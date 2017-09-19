using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Contracts
{
    public interface IOutput
    {
        void Set(LabeledValue labeledValue);
    }
}