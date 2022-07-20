using System;

public interface iKeepWait
{
    bool IsMoveNext();
}

public class WaitForSeconds : iKeepWait
{
    private float _seconds;
    private DateTime after;

    public WaitForSeconds(float seconds)
    {
        _seconds = seconds;
        after = DateTime.Now.AddSeconds(_seconds);
    }

    public bool IsMoveNext()
    {
        return after <= DateTime.Now;
    }
}

public class WaitUntil : iKeepWait
{
    public delegate bool Condition();
    private Condition _condition;

    public WaitUntil(Condition condition)
    {
        _condition = condition;
    }

    public bool IsMoveNext()
    {
        return _condition.Invoke();
    }
}

public class WaitWhile : iKeepWait
{
    public delegate bool Condition();
    private Condition _condition;

    public WaitWhile(Condition condition)
    {
        _condition = condition;
    }

    public bool IsMoveNext()
    {
        return !_condition.Invoke();
    }
}
