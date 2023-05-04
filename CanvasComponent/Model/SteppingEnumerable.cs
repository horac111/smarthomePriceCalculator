using CanvasComponent.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace CanvasComponent.Model
{
    public class SteppingEnumerable<T> : IEnumerable<T>, IEnumerable
    {
        private IEnumerable<IEnumerable<T>> list = new IEnumerable<T>[0];
        private IEnumerable<(int Index, IEnumerable<T> Collection)> removed = new (int Index, IEnumerable<T> Collection)[0];
        private int index = 0;
        private bool hadSeed = false;
        public IEnumerator<T> GetEnumerator()
        {
            var except = removed.TakeWhile(x => x.Index < index).SelectMany(x => x.Collection);
            var items = list.Take(index).SelectMany(x => x).Except(except);
            foreach (T obj in items)
                yield return obj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void StepForward()
        {
            if (CanStepForward())
                index++;
        }

        public void StepBackward()
        {
            if(CanStepBackward())
                index--;
        }

        public void Step(bool forward)
        {
            if (forward)
                StepForward();
            else 
                StepBackward();
        }

        public bool CanStepForward()
            => index < list.Count();

        public bool CanStepBackward()
            => index > (hadSeed ? 1 : 0);

        public void AddRange(IEnumerable<T> values, OperationType operation = OperationType.Add)
        {
            switch (operation)
            {
                case OperationType.Add:
                    list = list.Take(index).Append(values).ToArray();
                    removed = removed.TakeWhile(x => x.Index < index).ToArray();
                    break;
                case OperationType.Delete:
                    list = list.Take(index).Append(new T[0]).ToArray();
                    removed = removed.TakeWhile(x => x.Index < index)
                        .Append(new(index, values)).ToArray();
                    break;
                default:
                    throw new NotImplementedException($"OperationType {operation} is not supported yet");
            }
            index++;
        }

        public SteppingEnumerable(IEnumerable<T> initial)
        {
            list = new[] { initial };
            index = 1;
            hadSeed = true;
        }
        public SteppingEnumerable()
        {
        }
    }
}
