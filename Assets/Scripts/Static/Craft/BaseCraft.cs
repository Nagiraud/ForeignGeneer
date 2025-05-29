using System;
using Godot;

public abstract class BaseCraft
{
    public Recipe recipe { get; private set; }
    public Inventory input { get; private set; }
    public Inventory output { get; private set; }
    public Timer craftTimer { get; set; }
    public float craftProgress { get; protected set; } = 0;
    public bool isCrafting { get; protected set; } = false;
    private Action _onCraftFinished;

    public BaseCraft(Recipe recipe, Inventory input, Inventory output = null)
    {
        this.recipe = recipe;
        this.input = input;
        this.output = output;
    }

    public bool startCraft(Action onCraftFinished)
    {
        if (!canContinue())
            return false;

        if (compareRecipe())
        {
            isCrafting = true;
            _onCraftFinished = onCraftFinished;
            craftTimer.OneShot = true;
            craftTimer.WaitTime = recipe.duration;
            craftTimer.Timeout += onCraftFinished;
            craftTimer.Start();
            resetCraftProgress();
            consumeResources();
            return true;
        }

        return false;
    }

    public void stopCraft()
    {
        isCrafting = false;
        if (craftTimer != null)
        {
            craftTimer.Stop();
            if (_onCraftFinished != null)
            {
                craftTimer.Timeout -= _onCraftFinished;
                _onCraftFinished = null;
            }

            resetCraftProgress();
        }
    }

    public abstract bool compareRecipe();

    public abstract bool consumeResources();

    public bool addOutput()
    {
        if (recipe == null || recipe.output == null || output == null)
        {
            return true;
        }

        var recipeItem = recipe.output;
        var outputSlotItem = output.getItem(0);

        if (outputSlotItem == null)
        {
            output.addItemToSlot(new StackItem(recipeItem.getResource(), recipeItem.getStack()), 0);
            return true;
        }
        else if (outputSlotItem.getResource() == recipeItem.getResource())
        {
            int remainingSpace = outputSlotItem.getResource().getMaxStack - outputSlotItem.getStack();
            if (remainingSpace >= recipeItem.getStack())
            {
                outputSlotItem.add(recipeItem.getStack());
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void resetCraftProgress()
    {
        craftProgress = 0;
    }

    public void updateCraftProgress(double delta)
    {
        craftProgress += (float)delta / recipe.duration;
    }

    public virtual bool canContinue()
    {
        if (isCrafting)
            return false;

        if (output == null)
            return true;

        var outputSlotItem = output.getItem(0);
        return outputSlotItem == null ||(
               outputSlotItem.getStack() + recipe.output.getStack()
               < outputSlotItem.getResource().getMaxStack && recipe.output.getResource() == outputSlotItem.getResource());
    }

    public override string ToString()
    {
        return $"\nrecipe: {recipe}\noutput: {output}\ninput: {input}";
    }
}