using System;
using System.Collections;
using System.Collections.Generic;

namespace LimitedSizeStack;

public class LimitedSizeStack<T>
{
    T[] stack;
    int num;

    public LimitedSizeStack(int undoLimit)
    {
        stack = new T[undoLimit];
    }

    public void Push(T item)
    {
        if (stack.Length != 0)
        {
            if (num < stack.Length)
            {
                stack[num] = item;
                num++;
            }
            else
            {
                stack[num - 2] = stack[num - 1];
                stack[num - 1] = item;
            }
        }
    }

    public T Pop()
    {
        if (num == 0) throw new NotImplementedException();
        var result = stack[num - 1];
        num--;
        return result;
    }

    public int Count => num;
}