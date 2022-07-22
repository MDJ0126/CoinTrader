using System;
using System.Collections.Generic;

public class WaterfallProcess
{
    public enum FailedProcessType
    {
        Stop,   //실패시 중단 (default)
        Next,   //다음으로 진행
    }

    public Stack<Process> processStack;

    public struct Process
    {
        public FailedProcessType processType;
        public Action<Action<bool>> proc;

        public Process(Action<Action<bool>> proc, FailedProcessType processType)
        {
            this.proc = proc;
            this.processType = processType;
        }
    }

    public WaterfallProcess()
    {
        processStack = new Stack<Process>();
    }


    public void Start(Action<bool> OnFinished)
    {
        var stack = new Stack<Process>(processStack);
        ProcessStack(stack, OnFinished);
    }

    public void ProcessStack(Stack<Process> stack, Action<bool> OnFinished)
    {
        if (stack.Count > 0)
        {
            stack.Peek().proc((result) =>
            {
                if (!result)
                {//실패면
                    switch (stack.Peek().processType)
                    {
                        case FailedProcessType.Next:
                            //continue
                            break;
                        case FailedProcessType.Stop:
                        default:
                            stack.Clear();
                            OnFinished(false);
                            return;
                    }
                }

                stack.Pop();     //꺼냄
                ProcessStack(stack, OnFinished);    //재귀
            });
        }
        else
        {
            OnFinished(true);
        }
    }

    public void Add(Action<Action<bool>> process, FailedProcessType processType = FailedProcessType.Stop)
    {
        processStack.Push(new Process(process, processType));
    }

}