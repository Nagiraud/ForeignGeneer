public class FixedInputCraft : BaseCraft
{
    public FixedInputCraft(Recipe recipe, Inventory input)
        : base(recipe, input, null) // Pas de output
    {
    }

    public override bool compareRecipe()
    {
        if (recipe == null || recipe.input == null || input == null)
        {
            return false;
        }

        for (int i = 0; i < recipe.input.Count; i++)
        {
            var requiredItem = recipe.input[i];
            var slotItem = input.getItem(i);

            if (slotItem == null || slotItem.getResource() != requiredItem.getResource() || slotItem.getStack() < requiredItem.getStack())
            {
                return false;
            }
        }

        return true;
    }

    public override bool consumeResources()
    {
        for (int i = 0; i < recipe.input.Count; i++)
        {
            var requiredItem = recipe.input[i];
            var slotItem = input.getItem(i);

            if (slotItem != null && slotItem.getResource() == requiredItem.getResource())
            {
                slotItem.subtract(requiredItem.getStack());

                if (slotItem.isEmpty())
                {
                    input.deleteItem(i);
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public override bool canContinue()
    {
        if (isCrafting)
            return false;

        return true; // Pas de vérification d'output
    }
}