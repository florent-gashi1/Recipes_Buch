namespace Recipes.Client.Core.ViewModels;

// DataTemplateSelector: common base type so steps and notes fit in one list
public abstract class InstructionBaseViewModel;

public class InstructionViewModel : InstructionBaseViewModel
{
    public int Index { get; }
    public string Description { get; }

    public InstructionViewModel(int index, string description)
    {
        Index = index;
        Description = description;
    }
}

public class NoteViewModel : InstructionBaseViewModel
{
    public string Note { get; }

    public NoteViewModel(string note) => Note = note;
}
