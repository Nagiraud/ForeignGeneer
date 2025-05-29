using System;
using System.Collections.Generic;

public class BulkCraftWithOutput : BaseCraft
{
    private List<Inventory> _inputs;

    public BulkCraftWithOutput(Recipe recipe, List<Inventory> inputs, Inventory output = null)
        : base(recipe, null, output) 
    {
        this._inputs = inputs;
    }

    public override bool compareRecipe()
    {
        if (recipe == null || recipe.input == null || _inputs == null || _inputs.Count == 0)
        {
            return false;
        }

        if (output == null || output.getItem(0) == null || output.getItem(0).canAdd(recipe.output))
        {
            foreach (var requiredItem in recipe.input)
            {
                bool found = false;

                foreach (var inventory in _inputs)
                {
                    foreach (var slotItem in inventory.slots)
                    {
                        if (slotItem != null && slotItem.getResource() == requiredItem.getResource() && slotItem.getStack() >= requiredItem.getStack())
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        break;
                }

                if (!found)
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
        foreach (var requiredItem in recipe.input)
        {
            bool consumed = false;

            foreach (var inventory in _inputs)
            {
                for (int i = 0; i < inventory.slots.Count; i++)
                {
                    var slotItem = inventory.getItem(i);

                    if (slotItem != null && slotItem.getResource() == requiredItem.getResource())
                    {
                        int toConsume = Math.Min(slotItem.getStack(), requiredItem.getStack());
                        slotItem.subtract(toConsume);

                        if (slotItem.isEmpty())
                        {
                            inventory.deleteItem(i);
                        }

                        consumed = true;
                        break;
                    }
                }

                if (consumed)
                    break;
            }

            if (!consumed)
            {
                return false;
            }
        }

        return true;
    }
}