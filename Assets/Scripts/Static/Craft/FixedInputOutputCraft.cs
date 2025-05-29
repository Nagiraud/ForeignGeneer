public class FixedInputOutputCraft : BaseCraft
{
    public FixedInputOutputCraft(Recipe recipe, Inventory input, Inventory output = null)
        : base(recipe, input, output)
    {
    }

    public override bool compareRecipe()
    {
        if (recipe == null || recipe.input == null || input == null)
        {
            return false;
        }

        if (output == null || output.getItem(0) == null || output.getItem(0).canAdd(recipe.output))
        {
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

        return false;
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
}