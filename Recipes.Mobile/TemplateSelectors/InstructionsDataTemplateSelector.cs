using Recipes.Client.Core.ViewModels;

namespace Recipes.Mobile.TemplateSelectors;

// DataTemplateSelector: step template or note template
public class InstructionsDataTemplateSelector : DataTemplateSelector
{
    public required DataTemplate InstructionTemplate { get; init; }
    public required DataTemplate NoteTemplate { get; init; }

    // Called once per item
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is NoteViewModel)
            return NoteTemplate;

        return InstructionTemplate;
    }
}
